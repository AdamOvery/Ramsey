using System.Linq;

namespace Pascal;


/// <summary>PT stands for parallel traversal</summary>
public class PTNodeComparer : IComparer<INode>
{

    internal static void Tests()
    {
        TestEngine.Test("NodeComparer", () =>
         {
             //NodeWithMoreEdgesArrivesFirst();
             //NodeWithBetterAdjacentsArrivesFirst();
             // N5_and_N12_are_the_same();
             // N5_and_N12_are_not_the_same();
             //TheBigClVsCr();
             //CaVsCo();
             //CQVSC_();
             // DK_D__();
             //Cw();
             // Grz_bc();
             TestAdamCrazyGraph();
         });
    }
    class Batch
    {
        public readonly int level;
        public readonly int order;
        public readonly List<INode> a;
        public readonly List<INode> b;

        public Batch(int level, int order, IEnumerable<INode> a, IEnumerable<INode> b)
        {
            this.level = level;
            this.order = order;
            this.a = a.ToList();
            this.b = b.ToList();
        }
    }

    public int Compare(INode? a0, INode? b0)
    {
        if (a0 == null) return (b0 == null ? 0 : 1); // null is less than anything else
        else if (b0 == null) return -1; // anything is greater than null
        else if (a0 == b0) return 0; // same node is equal

        var visitingA = new Dictionary<INode, int>();
        var visitingB = new Dictionary<INode, int>();
        var visitedA = new Dictionary<INode, int>();
        var visitedB = new Dictionary<INode, int>();
        var queue = new Queue<Batch>();
        queue.Enqueue(new Batch(1, 1, new[] { a0 }, new[] { b0 }));

        int result = 0;
        // -1 makes a come first
        // 1 makes b come first

        int currentLevel = 0;

        while (queue.Count > 0)
        {
            var batch = queue.Dequeue();
            if (batch.level > currentLevel)
            {
                currentLevel = batch.level;
                foreach (var n in visitingA) if (!visitedA.ContainsKey(n.Key)) visitedA[n.Key] = n.Value;
                foreach (var n in visitingB) if (!visitedB.ContainsKey(n.Key)) visitedB[n.Key] = n.Value;

                visitingA.Clear();
                visitingB.Clear();
            }

            var a = batch.a.GroupBy(n => n.adjacentNodes.Count).ToDictionary(g => g.Key, g => g.ToList());
            var b = batch.b.GroupBy(n => n.adjacentNodes.Count).ToDictionary(g => g.Key, g => g.ToList());

            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var n in a) { if (n.Key < min) min = n.Key; if (n.Key > max) max = n.Key; }
            foreach (var n in b) { if (n.Key < min) min = n.Key; if (n.Key > max) max = n.Key; }

            for (int adjCount = max; adjCount >= min; adjCount--)
            {
                a.TryGetValue(adjCount, out var groupA);
                b.TryGetValue(adjCount, out var groupB);
                if (groupA != null && groupB != null)
                {
                    foreach (var n in groupA) if (!visitingA.ContainsKey(n)) visitingA[n] = batch.level;
                    foreach (var n in groupB) if (!visitingB.ContainsKey(n)) visitingB[n] = batch.level;

                    var groupACount = groupA.Count;
                    var groupBCount = groupB.Count;
                    if (groupACount == groupBCount)
                    {
                        var groupAAdj = groupA.SelectMany(n => n.adjacentNodes).Distinct().ToArray();
                        var groupBAdj = groupB.SelectMany(n => n.adjacentNodes).Distinct().ToArray();

                        var newBatch = new Batch(batch.level + 1, groupACount,
                            groupAAdj.Where(n => !visitedA.ContainsKey(n)),
                            groupBAdj.Where(n => !visitedB.ContainsKey(n)));
                        queue.Enqueue(newBatch);
                    }
                    else
                    {
                        result = groupBCount.CompareTo(groupACount); // we put the node with the most adjacent nodes first
                        break;
                    }
                }
                else
                {
                    if (groupA == null) return 1;
                    else if (groupB == null) return -1;
                    else continue;
                }
            }
            if (result != 0) break;

        }


        return result;
    }

    static int getEdgeNo(INode na, INode nb)
    {
        var a = na.id;
        var b = nb.id;

        return (a < b) ? a + b * (b - 1) / 2
        : (a > b) ? b + a * (a - 1) / 2
        : 0;
    }

    public static readonly PTNodeComparer instance = new PTNodeComparer();




    private static void NodeWithMoreEdgesArrivesFirst()
    {
        TestEngine.Test("The node with more edges arrives first.", () =>
        {
            //
            var g = G6.parse("IS`A?????").AsSubGraph();
            TestEngine.Assert("1 arrives before 2", () => (1).CompareTo(2) < 0); // sanity check 1 arrives before 2
            NodeArrivesBefore(g, 0, 1);
            NodeArrivesBefore(g, 0, 9);
        });
    }

    private static void NodeWithBetterAdjacentsArrivesFirst()
    {
        TestEngine.Test("When two nodes have same edge count, the node with better adjacents arrives first", () =>
        {
            var g = G6.parse("Is??GGC?G").AsSubGraph();
            NodeArrivesBefore(g, 5, 0);
        });
    }

    // CQVSC_
    private static void CQVSC_()
    {
        // CQ and C` are equivalent. but C` is a tad better

        TestEngine.Test("CQVSC`", () =>
        {
            var g = G6.parse("CQ").AsSubGraph();
            NodeArrivesBefore(g, 0, 2);
            NodeArrivesBefore(g, 0, 1);
            NodeArrivesBefore(g, 0, 3);
            NodeArrivesBefore(g, 2, 1);
        });
    }

    private static void DK_D__()
    {
        // CQ and C` are equivalent. but C` is a tad better

        TestEngine.Test("DK? vs D`?`", () =>
        {
            var g = G6.parse("DK?").AsSubGraph();
            NodeArrivesBefore(g, 3, 1);
        });
    }

    private static void Cw()
    {

        TestEngine.Test("Cw", () =>
        {
            var g = G6.parse("Cw").AsSubGraph();
            NodeArrivesBefore(g, 0, 1);
            NodeArrivesBefore(g, 0, 2);
            NodeArrivesBefore(g, 1, 2);
        });
    }

    private static void Grz_bc()
    {
        //

        TestEngine.Test(@"Grz\bc", () =>
        {
            var g = G6.parse(@"Grz\bc").AsSubGraph();
            NodeArrivesBefore(g, 0, 1);
            NodeArrivesBefore(g, 1, 2);
            NodeArrivesBefore(g, 2, 3);
            NodeArrivesBefore(g, 3, 4);
            NodeArrivesBefore(g, 4, 5);
            NodeArrivesBefore(g, 5, 6);
            NodeArrivesBefore(g, 6, 7);

            NodeArrivesBefore(g, 0, 2);
            NodeArrivesBefore(g, 1, 3);
            NodeArrivesBefore(g, 2, 4);
            NodeArrivesBefore(g, 3, 5);
            NodeArrivesBefore(g, 4, 6);
            NodeArrivesBefore(g, 5, 7);

            NodeArrivesBefore(g, 0, 3);
            NodeArrivesBefore(g, 1, 4);
            NodeArrivesBefore(g, 2, 5);
            NodeArrivesBefore(g, 3, 6);
            NodeArrivesBefore(g, 4, 7);

            NodeArrivesBefore(g, 0, 4);
            NodeArrivesBefore(g, 1, 5);
            NodeArrivesBefore(g, 2, 6);
            NodeArrivesBefore(g, 3, 7);

            NodeArrivesBefore(g, 0, 5);
            NodeArrivesBefore(g, 1, 6);
            NodeArrivesBefore(g, 2, 7);

            NodeArrivesBefore(g, 0, 6);
            NodeArrivesBefore(g, 1, 7);

            NodeArrivesBefore(g, 0, 7);
        });
    }

    private static void NodeArrivesBefore(ISubGraph g, int n1, int n2)
    {
        TestEngine.Assert("n{n1} arrives before n{n2}", () => PTNodeComparer.instance.Compare(g.nodes[n1], g.nodes[n2]) < 0);
    }

    private static void NodeAreIdentical(ISubGraph g, int n1, int n2)
    {
        TestEngine.Assert("n{n1} is identical to n{n2}", () => PTNodeComparer.instance.Compare(g.nodes[n1], g.nodes[n2]) == 0);
    }

    /*
    [Failed] [Shuffled] #0 original:Fr^cw shuffled:FrurW  expected F}oxw actual F{dzo
    [Failed] [Shuffled] #0 original:GUhZ~o shuffled:G~h[W{  expected GrzTrg actual Grz\bc
    */

    private static void GUhZ_o()
    {

        TestEngine.Test("GUhZ~o", () =>
        {
            var g = G6.parse("GUhZ~o").AsSubGraph();
            // NodeArrivesBefore(g, 1, 2);
        });
    }

    private static void N5_and_N12_are_the_same()
    {
        TestEngine.Test(@"MGC?GCC?GG?@S?_?_", () =>
        {
            var g = G6.parse(@"MGC?GCC?GG?@S?_?_").AsSubGraph();
            NodeAreIdentical(g, 5, 12);
        });
    }

    private static void N5_and_N12_are_not_the_same()
    {
        TestEngine.Test(@"NGC?GCC?GG?@S???o?G", () =>
        {
            var g = G6.parse(@"NGC?GCC?GG?@S???o?G").AsSubGraph();
            NodeArrivesBefore(g, 12, 5);
        });
    }

    public static void TestAdamCrazyGraph()
    {
        TestEngine.Test("TestAdamCrazyGraph", () =>
        {
            var g = G6.parse(@"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw").AsSubGraph();
            for (int n1 = 0; n1 < g.nodes.Count; n1++)
            {
                var a = g.nodes[n1];
                for (int n2 = 0; n2 < g.nodes.Count; n2++)
                {
                    var b = g.nodes[n2];
                    var r = PTNodeComparer.instance.Compare(a, b);
                    Console.Write((r < 0) ? "<" : (r > 0) ? ">" : "=");
                }
                Console.WriteLine();
            }
            //var sig41b = @"h~zeubOrURWpoLmwTmtL?gOmZu[tXcZ}qHlc]UQzmw?M\FOZfghYe?zKvxIJBEosPg]ak}DTnXwA]Z`YdDz_kSUEZFSnkI_maRQtaptRN[CpfHqzC]vZtgAD]fpboL^lbBo?NuUD]k";
            //TestSorted("ex9", @"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41b, comparer);
            //Console.WriteLine("Adam the two graphs you gave me doesn't seem to match");
        });
    }
}
