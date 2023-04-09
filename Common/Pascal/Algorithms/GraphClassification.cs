using System.Text.RegularExpressions;

namespace Pascal;


public class GraphClassification
{
    public static void Tests()
    {
        TestCliqueOf5andCliqueOf3();
        TestTwoInFour();
        TestTriangleWithLeg();
        TestSmallGraphOf4();
        TestSmallGraphOf5();
        TestBiggerGraphOf5();
        TestLargerGraphOf12();
        // TestAdamCrazyGraph();

    }

    abstract record class Signature : IComparable<Signature>
    {
        public int CompareTo(Signature? other)
        {
            int result;
            if (this is AdjSignature)
            {
                if (other is AdjSignature)
                {
                    var thisArr = ((AdjSignature)this).adj;
                    var otherArr = ((AdjSignature)other!).adj;
                    int min = Math.Min(thisArr.Length, otherArr.Length);
                    for (int i = 0; i < min; i++)
                    {
                        var thisEntry = thisArr[i];
                        var otherEntry = otherArr[i];
                        result = thisEntry.CompareTo(otherEntry);
                        if (result != 0) return result;
                    }
                    int result1 = this.ToString().CompareTo(other?.ToString());
                    int result2;
                    if (thisArr.Length > otherArr.Length) result2 = -1;
                    else if (thisArr.Length < otherArr.Length) result2 = 1;
                    else result2 = 0;
                    if (result1 == result2) return result = result1;
                    else
                    {
                        Console.WriteLine($"result1: {result1}, result2: {result2}");
                        return result = result1;
                    }
                }
                else
                { // other is LnkSignature
                    result = -1;
                }
            }
            else // this is LnkSignature
            {
                if (other is AdjSignature)
                {
                    result = 1;
                }
                else
                { // other is LnkSignature
                    var thisLnk = ((LnkSignature)this).lnk;
                    var otherLnk = ((LnkSignature)other!).lnk;
                    result = thisLnk.CompareTo(otherLnk);
                }

            }
            return result;
        }
    }

    record class AdjSignature(Signature[] adj) : Signature
    {
        override public string ToString()
        {
            return $"[{String.Join(',', adj.Select(a => a.ToString()))}]";
        }
    }

    record class LnkSignature(int lnk) : Signature
    {
        override public string ToString()
        {
            return lnk.ToString();
        }
    }


    static Regex codeComment = new Regex(@"\/\*[^*]*\*\/", RegexOptions.Multiline);

    public static string removeComments(string? signatureWithComments)
    {
        return codeComment.Replace(signatureWithComments ?? "", "");
    }

    class NoCommentsComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return removeComments(x).CompareTo(removeComments(y));
        }

        public static NoCommentsComparer instance = new NoCommentsComparer();
    }

    private static string GetSignature(IGraph g, INode n1, bool withComments = false)
    {

        int[] visited = new int[g.order];

        var getSignature = new Func<INode, int, Signature>((start, depth) => { return new LnkSignature(0); });

        getSignature = (INode start, int depth) =>
        {
            if (visited[start.id] > 0) return new LnkSignature(depth - visited[start.id] - 1);
            else
            {
                visited[start.id] = depth;
                var adjacentNodes = start.adjacentNodes.Select(n =>
                {
                    if (n == n1) return new LnkSignature(0);
                    else return getSignature(n, depth + 1);
                }).ToArray();
                Array.Sort(adjacentNodes);
                visited[start.id] = 0;
                return new AdjSignature(adjacentNodes);
            }
        };
        var signature = getSignature(n1, 1);
        return signature.ToString();
    }



    private static string GetSignatures(ISubGraph subgraph, bool withComments = false)
    /*
Cm [
    [[[-2,0],0],[[-2,0],0]],
    [[[-2,0],0],[[-2,0],0]],
    [[[-2,0],0],[[-2,0],0]],
    [[0]],
    [[0]]
]

[
    (N0:) [(N1:)[(N0:)0,(N2:)[(N1:)1],(N3:)[(N0:)0,(N1:)1]],(N3:)[(N0:)0,(N1:)[(N0:)0,(N2:)[(N1:)1],(N3:)1]]],\n
    (N1:) [(N0:)[(N1:)0,(N3:)[(N0:)1,(N1:)0]],(N2:)[(N1:)0],(N3:)[(N0:)[(N1:)0,(N3:)1],(N1:)0]],\n
    (N2:) [(N1:)[(N0:)[(N1:)1,(N3:)[(N0:)1,(N1:)2]],(N2:)0,(N3:)[(N0:)[(N1:)2,(N3:)1],(N1:)1]]],\n
    (N3:) [(N0:)[(N1:)[(N0:)1,(N2:)[(N1:)1],(N3:)0],(N3:)0],(N1:)[(N0:)[(N1:)1,(N3:)0],(N2:)[(N1:)1],(N3:)0]]
]

A = [-2,0]
B = [0]
C = [A,0]
D = [B]
E = [C,C]

// [E,E,E,D,D]
     */
    {
        // var signatures = new List<string>();
        var signatures = new List<string>();
        foreach (var n1 in subgraph.nodes)
        {
            var sig = GetSignature(subgraph.graph, n1, withComments);
            if (withComments) sig = $"/*#{n1.id}*/{sig}";
            signatures.Add(sig);
        }
        string result;
        if (!withComments) signatures.Sort();

        var delim = withComments ? ",\n" : ",";
        result = "[" + String.Join(delim, signatures) + "]";
        return result;


    }



    static void TestSignature(string g6, string expectedSignature, bool withComments = false)
    {
        var subgraph = (G6.parse(g6).AsSubGraph())!;

        var newSignature = GetSignatures(subgraph, withComments);
        TestEngine.AssertEquals($"Graph {g6} signature", () => newSignature, expectedSignature);
    }

    static string GetSignature(string g6, bool withComments = false)
    {
        var subgraph = (G6.parse(g6).AsSubGraph())!;
        var newSignature = GetSignatures(subgraph, withComments);
        return newSignature;
    }

    public static void TestTwoInFour()
    {
        TestSignature("CC", @"[/*#0*/[[0]],
/*#1*/[],
/*#2*/[],
/*#3*/[[0]]]", true);
    }

    public static void TestSmallGraphOf4()
    {
        TestSignature("Cm", "[[[[[1,2],1],[[1,2],1],0]],[[[[1],0,1],0],[[0,1],[1],0]],[[[[1],0,1],0],[[0,1],[1],0]],[[[0,1],0],[[0,1],0],[0]]]", false);
        TestSignature("Cy", "[[[[[1,2],1],[[1,2],1],0]],[[[[1],0,1],0],[[0,1],[1],0]],[[[[1],0,1],0],[[0,1],[1],0]],[[[0,1],0],[[0,1],0],[0]]]", false);

    }


    public static void TestSmallGraphOf5()
    {
        string sig5 = GetSignature("DKW");
        TestSignature("DKW", sig5);
        TestSignature("DeG", sig5);
        TestSignature("DKc", sig5);
    }

    public static void TestBiggerGraphOf5()
    {
        string sig5 = GetSignature("DK{");
        TestSignature("DyS", sig5);
        TestSignature("DxK", sig5);
    }

    public static void TestLargerGraphOf12()
    {
        string sig12 = GetSignature("K]W@WC@?G?oD");
        TestSignature("KC@C?[K{C_?P", sig12);
        TestSignature("KGsA@?dCHEQ?", sig12);
    }

    public static void TestAdamCrazyGraph()
    {
        string sig41 = GetSignature(@"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw");
        TestSignature(@"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41);
    }

    public static void TestTriangleWithLeg()
    {
        TestSignature("Cm", @"[/*#0*/[[[[1],0,1],0],[[0,1],[1],0]],
/*#1*/[[[0,1],0],[[0,1],0],[0]],
/*#2*/[[[[1,2],1],[[1,2],1],0]],
/*#3*/[[[[1],0,1],0],[[0,1],[1],0]]]", true);
    }



    public static void TestCliqueOf5andCliqueOf3()
    {
        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        // var g = (G6.parse("Fg???").AsSubGraph())!;
        var sigClique5AndClique3 = GetSignature("K~{???A????S");
        TestSignature("K~{???A????S", sigClique5AndClique3);
        TestSignature("K@Kw?A?_Hw??", sigClique5AndClique3);
        TestSignature("K?O?AOCQQQ_c", sigClique5AndClique3);
        TestSignature("K@@@_?qKg?G_", sigClique5AndClique3);

    }
}

