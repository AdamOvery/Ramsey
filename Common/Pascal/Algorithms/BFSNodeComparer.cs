namespace Pascal;


/// <summary>Compares two node in their existing configuration.</summary>
/// <remarks>If the adjacent nodes are not sorted, it will favour the best sorted adjacent node.</remarks>
public class BFSNodeComparer : IComparer<INode>
{
    internal static void Tests()
    {
        TestEngine.Test("NodeComparer", () =>
         {
             // NodeWithMoreEdgesArrivesFirst();
             // NodeWithBetterAdjacentsArrivesFirst();
             //TheBigClVsCr();
             //CaVsCo();
             CQVSC_();
         });
    }

    public int Compare(INode? a0, INode? b0)
    {
        if (a0 == null) return (b0 == null ? 0 : 1); // null is less than anything else
        else if (b0 == null) return -1; // anything is greater than null
        else if (a0 == b0) return 0; // same node is equal

        var visited = new Dictionary<Tuple<INode, INode>, int>();
        var queue = new Queue<Tuple<INode, INode>>();

        // -1 makes a come first
        // 1 makes b come first
        int result = 0;
        var pair0 = new Tuple<INode, INode>(a0, b0);
        queue.Enqueue(pair0);

        while (queue.Count > 0)
        {
            var pair = queue.Dequeue();
            if (visited.TryGetValue(pair, out result)) continue;
            // visited[pair] = 0;
            var a = pair.Item1;
            var b = pair.Item2;
            if (a != b)
            {
                var adjA = a.adjacentNodes;
                var adjB = b.adjacentNodes;
                int adjACount = adjA.Count;
                int adjBCount = adjB.Count;
                if (adjACount == adjBCount)
                {
                    for (int i = 0; i < adjACount; i++)
                    {
                        queue.Enqueue(new Tuple<INode, INode>(adjA[i], adjB[i]));
                    }
                }
                else
                {
                    result = adjBCount.CompareTo(adjACount); // we put the node with the most adjacent nodes first
                    break;
                }
            }
            visited[pair] = result;
        }


        if (result == 0)
        {
            if (a0.id == 0) return -1;
            else if (b0.id == 0) return 1;

            queue.Enqueue(pair0);
            visited.Clear();
            while (queue.Count > 0)
            {
                var pair = queue.Dequeue();
                if (visited.TryGetValue(pair, out result)) continue;
                // visited[pair] = 0;
                var a = pair.Item1;
                var b = pair.Item2;
                if (a != b)
                {
                    var adjA = a.adjacentNodes;
                    var adjB = b.adjacentNodes;
                    int na = adjA.Count;
                    int nb = adjB.Count;
                    if (na != nb) throw new Exception("Internal Error na!=nb");
                    for (int i = 0; i < na; i++)
                    {
                        var aa = adjA[i];
                        var bb = adjB[i];
                        if (aa != bb)
                        {
                            result = aa.id.CompareTo(bb.id);
                            break;
                        }
                    }
                }
                visited[pair] = result;
            }
            if (result == 0)
            {
                result = a0.id.CompareTo(b0.id);
            }
        }
        return result;
    }

    public static readonly BFSNodeComparer instance = new BFSNodeComparer();




    private static void NodeWithMoreEdgesArrivesFirst()
    {
        TestEngine.Test("The node with more edges arrives first.", () =>
        {
            //
            var g = G6.parse("IS`A?????").AsSubGraph();
            TestEngine.Assert("1 arrives before 2", () => (1).CompareTo(2) < 0); // sanity check 1 arrives before 2
            TestEngine.Assert("n0 arrives before n1", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[1]) < 0);
            TestEngine.Assert("n0 arrives before n9", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[9]) < 0);
        });
    }

    private static void NodeWithBetterAdjacentsArrivesFirst()
    {
        TestEngine.Test("When two nodes have same edge count, the node with better adjacents arrives first", () =>
        {
            var g = G6.parse("Is??GGC?G").AsSubGraph();
            TestEngine.Assert("n5 arrives before n0", () => BFSNodeComparer.instance.Compare(g.nodes[5], g.nodes[0]) < 0);
        });
    }

    // private static void When_all_is_equal_the_node_with_the_lower_id_wins_which_is_wrong()
    // {
    //     TestEngine.Test("When two nodes are identical, the node with the lower edge ID arrives first", () =>
    //     {
    //         var g = G6.parse("IA???@O??").AsSubGraph();
    //         TestEngine.Assert("n1 arrives before n3", () => BFSNodeComparer.instance.Compare(g.nodes[1], g.nodes[3]) < 0);
    //         TestEngine.Assert("n3 arrives before n8", () => BFSNodeComparer.instance.Compare(g.nodes[3], g.nodes[8]) < 0);
    //         TestEngine.Assert("n8 arrives before n0", () => BFSNodeComparer.instance.Compare(g.nodes[8], g.nodes[0]) < 0);
    //     });
    // }
    // // EAS?

    // // Cr
    // private static void TheBigClVsCr()
    // {
    //     // C] Cl and Cr are equivalent. It is a square
    //     //  
    //     TestEngine.Test("TheBigClVsCr", () =>
    //     {

    //         var g = G6.parse("Cl").AsSubGraph();
    //         TestEngine.Assert("n0 arrives before n1", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[1]) < 0);
    //         TestEngine.Assert("n1 arrives before n3", () => BFSNodeComparer.instance.Compare(g.nodes[1], g.nodes[3]) < 0);
    //         TestEngine.Assert("n3 arrives before n2", () => BFSNodeComparer.instance.Compare(g.nodes[3], g.nodes[2]) < 0);
    //     });
    // }

    // //CaVsCo

    // private static void CaVsCo()
    // {
    //     // C] Cl and Cr are equivalent. It is a square
    //     //  
    //     TestEngine.Test("CaVsCo", () =>
    //     {

    //         var g = G6.parse("Ca").AsSubGraph();
    //         TestEngine.Assert("n1 arrives before n0", () => BFSNodeComparer.instance.Compare(g.nodes[1], g.nodes[0]) < 0);
    //     });
    // }

    // public static void TestD_oVsDzc()
    // {
    //     TestEngine.Test("TestD^oVsDzc", () =>
    //     {
    //         var g = G6.parse("D^o").AsSubGraph();
    //         TestEngine.Assert("n3 arrives before n1", () => BFSNodeComparer.instance.Compare(g.nodes[3], g.nodes[1]) < 0);
    //     });
    // }

    // CQVSC_
    private static void CQVSC_()
    {
        // CQ and C' are equivalent. but C' is a tad better

        TestEngine.Test("CQVSC'", () =>
        {
            var g = G6.parse("CQ").AsSubGraph();
            TestEngine.Assert("n0 arrives before n2", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[2]) < 0);
            TestEngine.Assert("n0 arrives before n1", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[1]) < 0);
            TestEngine.Assert("n0 arrives before n3", () => BFSNodeComparer.instance.Compare(g.nodes[0], g.nodes[3]) < 0);
            TestEngine.Assert("n2 arrives before n1", () => BFSNodeComparer.instance.Compare(g.nodes[2], g.nodes[1]) < 0);
        });
    }

}
