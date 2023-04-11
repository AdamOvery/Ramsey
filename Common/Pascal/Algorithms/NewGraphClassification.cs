using System.Text.RegularExpressions;

namespace Pascal;


public partial class NewGraphClassification
{

   public static void Tests()
    {
        TestTwoInFour();
        TestSmallGraphOf4();
        TestSmallGraphOf5();
        TestBiggerGraphOf5();
        TestCliqueOf5andCliqueOf3();
        // broken TestLargerGraphOf12();
        // not this one TestAdamCrazyGraph();
    }
    static string GetNewSignature(string g6, bool withComments = false)
    {
        var subgraph = G6.parse(g6).AsSubGraph();
        
        var newSignature = GetNewSignature(subgraph, withComments);
        return newSignature;
    }


    static void TestNewSignature(string g6, string expectedSignature, bool withComments = false)
    {
        var subgraph = G6.parse(g6).AsSubGraph();

        var newSignature = GetNewSignature(subgraph, withComments);
        TestEngine.AssertEquals($"Graph {g6} signature", () => newSignature, expectedSignature);
    }


    private static string GetNewSignature(ISubGraph subgraph, bool withComments = false)
    {
        var order = subgraph.graph.order;
        bool[] visited = new bool[order];
        int?[] oldIdToNewId = new int?[order];
        int?[] newIdToOldId = new int?[order];

        var findBestNode = new Func<IEnumerable<INode>, Func<INode, INode, int>, INode>((nodes, finalCompare) =>
        {
            INode? bestNode = null;
            foreach (var n in nodes)
            {
                if (!visited[n.id])
                {
                    if
                      (bestNode == null)
                    {
                        bestNode = n;
                    }
                    else
                    {
                        int cmp2 = DFSNodeComparer.instance.Compare(n, bestNode);
                        if (cmp2 == 0)
                        {
                            cmp2 = finalCompare(n, bestNode);
                        }
                        if (cmp2 > 0) bestNode = n;
                    }
                }
            }
            return bestNode!;
        });

        for (int nodeNo = 0; nodeNo < order; nodeNo++)
        {
            var bestNode = findBestNode(subgraph.nodes, (a, b) =>
            {
                int? na = oldIdToNewId[a.id];
                int? nb = oldIdToNewId[b.id];
                if (na == null && nb == null) return 0;
                else if (na == null) return -1;
                else if (nb == null) return +1;
                return nb.Value.CompareTo(na.Value);
            });
            visited[bestNode.id] = true;
            newIdToOldId[nodeNo] = bestNode.id;
            oldIdToNewId[bestNode.id] = nodeNo;
        }
        var delim = withComments ? ",\n" : ",";
        List<string> signatures = new List<string>();

        for (int nodeNo = 0; nodeNo < order; nodeNo++)
        {
            int oldId = (int)newIdToOldId[nodeNo]!;
            var n = subgraph.nodes[oldId];
            signatures.Add("[" + String.Join(",", n.adjacentNodes.Select(n2 => oldIdToNewId[n2.id]).OrderBy(n2 => n2)) + "]");
        }
        return "[" + String.Join(delim, signatures) + "]";
    }

    public static void TestTwoInFour()
    {
        string sig2 = GetNewSignature("CC");
        TestNewSignature("CG", sig2);
        TestNewSignature("CA", sig2);
        TestNewSignature("C_", sig2);
    }

    public static void TestSmallGraphOf4()
    {
        string sig4 = GetNewSignature("Cm");
        TestNewSignature("Cy", sig4);

    }


    public static void TestSmallGraphOf5()
    {
        string sig5 = GetNewSignature("DKW");
        TestNewSignature("DKW", sig5);
        TestNewSignature("DeG", sig5);
        TestNewSignature("DKc", sig5);
    }

    public static void TestBiggerGraphOf5()
    {
        string sig5 = GetNewSignature("DK{");
        //TestNewSignature("DyS", sig5);
        //TestNewSignature("DxK", sig5);
    }

    public static void TestLargerGraphOf12()
    {
        string sig12 = GetNewSignature("K]W@WC@?G?oD");
        TestNewSignature("KC@C?[K{C_?P", sig12);
        TestNewSignature("KGsA@?dCHEQ?", sig12);
    }

    public static void TestAdamCrazyGraph()
    {
        string sig41 = GetNewSignature(@"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw");
        TestNewSignature(@"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41);
    }

    public static void TestCliqueOf5andCliqueOf3()
    {
        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        // var g = G6.parse("Fg???").AsSubGraph();
        var sigClique5AndClique3 = GetNewSignature("K~{???A????S");
        TestNewSignature("K~{???A????S", sigClique5AndClique3);
        TestNewSignature("K@Kw?A?_Hw??", sigClique5AndClique3);
        TestNewSignature("K?O?AOCQQQ_c", sigClique5AndClique3);
        TestNewSignature("K@@@_?qKg?G_", sigClique5AndClique3);

    } 

}

