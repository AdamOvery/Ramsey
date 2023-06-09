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
             //CQVSC_();
             // DK_D__();
             //Cw();
             Grz_bc();
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
            // then we favour the node with the lowest id
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
                        var a2 = adjA[i];
                        var b2 = adjB[i];

                        if (a == b2 && b == a2) continue;
                        var aToA2 = getEdgeNo(a, a2);
                        var bToB2 = getEdgeNo(b, b2);

                        var aToB2 = getEdgeNo(a, b2 == a ? b : b2);
                        var bToA2 = getEdgeNo(b, a2 == b ? a : a2);
                        if (b2 == a || a2 == b)
                        {
                            Console.WriteLine($"a={a.id} b={b.id} a2={a2.id} b2={b2.id}");
                        }
                        var minKeep = Math.Min(aToA2, bToB2);
                        var minSwap = Math.Min(aToB2, bToA2);
                        if (minKeep < minSwap) { result = -1; break; }
                        else if (minSwap < minKeep) { result = 1; break; }

                    }
                }
                visited[pair] = result;
                if (result != 0) break;
            }
            if (result == 0)
            {
                result = a0.id.CompareTo(b0.id); // we have the same edges, we favour the node with the lowest id
            }
            if (SortedGraph.Verbose) Console.WriteLine($"BFSNodeComparer: {a0.id} {b0.id} = {result} (by EdgeNo)");
        }
        else
        {
            if (SortedGraph.Verbose) Console.WriteLine($"BFSNodeComparer: {a0.id} {b0.id} = {result}");
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

    public static readonly BFSNodeComparer instance = new BFSNodeComparer();




    private static void NodeWithMoreEdgesArrivesFirst()
    {
        TestEngine.Test("The node with more edges arrives first.", () =>
        {
            //
            var g = G6.parse("IS`A?????").AsSubGraph();
            TestEngine.Assert("1 arrives before 2", () => (1).CompareTo(2) < 0); // sanity check 1 arrives before 2
            TestNodeArrivesBefore(g, 0, 1);
            TestNodeArrivesBefore(g, 0, 9);
        });
    }

    private static void NodeWithBetterAdjacentsArrivesFirst()
    {
        TestEngine.Test("When two nodes have same edge count, the node with better adjacents arrives first", () =>
        {
            var g = G6.parse("Is??GGC?G").AsSubGraph();
            TestNodeArrivesBefore(g, 5, 0);
        });
    }

    // CQVSC_
    private static void CQVSC_()
    {
        // CQ and C` are equivalent. but C` is a tad better

        TestEngine.Test("CQVSC`", () =>
        {
            var g = G6.parse("CQ").AsSubGraph();
            TestNodeArrivesBefore(g, 0, 2);
            TestNodeArrivesBefore(g, 0, 1);
            TestNodeArrivesBefore(g, 0, 3);
            TestNodeArrivesBefore(g, 2, 1);
        });
    }

    private static void DK_D__()
    {
        // CQ and C` are equivalent. but C` is a tad better

        TestEngine.Test("DK? vs D`?`", () =>
        {
            var g = G6.parse("DK?").AsSubGraph();
            TestNodeArrivesBefore(g, 3, 1);
        });
    }

    private static void Cw()
    {

        TestEngine.Test("Cw", () =>
        {
            var g = G6.parse("Cw").AsSubGraph();
            TestNodeArrivesBefore(g, 0, 1);
            TestNodeArrivesBefore(g, 0, 2);
            TestNodeArrivesBefore(g, 1, 2);
        });
    }

    private static void Grz_bc()
    {
        //

        TestEngine.Test(@"Grz\bc", () =>
        {
            var g = G6.parse(@"Grz\bc").AsSubGraph();
            TestNodeArrivesBefore(g, 0, 1);
            TestNodeArrivesBefore(g, 1, 2);
            TestNodeArrivesBefore(g, 2, 3);
            TestNodeArrivesBefore(g, 3, 4);
            TestNodeArrivesBefore(g, 4, 5);
            TestNodeArrivesBefore(g, 5, 6);
            TestNodeArrivesBefore(g, 6, 7);

            TestNodeArrivesBefore(g, 0, 2);
            TestNodeArrivesBefore(g, 1, 3);
            TestNodeArrivesBefore(g, 2, 4);
            TestNodeArrivesBefore(g, 3, 5);
            TestNodeArrivesBefore(g, 4, 6);
            TestNodeArrivesBefore(g, 5, 7);

            TestNodeArrivesBefore(g, 0, 3);
            TestNodeArrivesBefore(g, 1, 4);
            TestNodeArrivesBefore(g, 2, 5);
            TestNodeArrivesBefore(g, 3, 6);
            TestNodeArrivesBefore(g, 4, 7);

            TestNodeArrivesBefore(g, 0, 4);
            TestNodeArrivesBefore(g, 1, 5);
            TestNodeArrivesBefore(g, 2, 6);
            TestNodeArrivesBefore(g, 3, 7);

            TestNodeArrivesBefore(g, 0, 5);
            TestNodeArrivesBefore(g, 1, 6);
            TestNodeArrivesBefore(g, 2, 7);

            TestNodeArrivesBefore(g, 0, 6);
            TestNodeArrivesBefore(g, 1, 7);

            TestNodeArrivesBefore(g, 0, 7);
        });
    }

    private static void TestNodeArrivesBefore(ISubGraph g, int n1, int n2)
    {
        TestEngine.Assert("n{n1} arrives before n{n2}", () => BFSNodeComparer.instance.Compare(g.nodes[n1], g.nodes[n2]) < 0);
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
            // TestNodeArrivesBefore(g, 1, 2);
        });
    }

}
