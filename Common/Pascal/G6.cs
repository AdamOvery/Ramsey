using System.Text;

namespace Pascal;

class G6
{

    public static IGraph parse(string g6, IGraphFactory? factory = null)
    {
        if (factory == null) factory = MatrixGraph.factory;
        var order = g6[0] - 63;
        if (order < 0) throw new Exception("Invalid G6 data");
        var graph = factory.newGraph(order);
        var a = 0;
        var b = 1;
        for (int i = 1; i < g6.Length; i++)
        {
            var currentByte = g6[i] - 63;
            if (currentByte < 0) throw new Exception("Invalid G6 data");

            var v = 32;
            while (v > 0)
            {
                if ((currentByte & v) != 0)
                {
                    graph.SetEdgeValue(a, b, true);
                }
                v = v >> 1;
                a += 1;
                if (a == b)
                {
                    a = 0;
                    b += 1;
                    if (b == order) break;
                }
            }
        }
        return graph;
    }


    public static String fromGraph(IGraph g)
    {
        var result = new StringBuilder();
        result.Append((char)(63 + g.order));
        int mask = 32;
        int currentByte = 0;

        for (var b = 1; b < g.order; b++)
        {
            for (var a = 0; a < b; a++)
            {
                if (g.GetEdgeValue(a, b))
                {
                    currentByte = (currentByte | mask);
                }
                mask >>= 1;
                if (mask == 0)
                {
                    result.Append((char)(63 + currentByte));
                    currentByte = 0;
                    mask = 32;
                }
            }
        }
        if (mask != 32) result.Append((char)(63 + currentByte));
        return result.ToString();
    }


    private static void TestG6(string g6String, int order, Action<IGraph> innerTests)
    {
        var g = parse(g6String, MatrixGraph.factory);
        innerTests.Invoke(g);
        var newG6String = G6.fromGraph(g);
        if (g6String != newG6String) throw new Exception($"Internal Error in G6 methods expected: {g6String} actual: {newG6String}");
    }
    private static void TestEdges(IGraph g, int node1, params int[] otherNodes)
    {
        for (int node2 = 0; node2 < g.order; node2++)
        {
            var expected = otherNodes.AsEnumerable().Contains(node2);
            var actual = g.GetEdgeValue(node1, node2);
            if (expected != actual)
            {
                throw new Exception($"Invalid edge value {node1}-{node2} expected {expected} actual {actual}");
            }
        }
    }

    public static void Tests()
    {
        TestG6("@", 1, (g) =>
        {
            TestEdges(g, 0);
        });
        TestG6("Es\\o", 6, (g) =>
        {
            TestEdges(g, 0, 1, 2, 3);
            TestEdges(g, 0, 1, 2, 3);
            TestEdges(g, 1, 0, 4, 5);
            TestEdges(g, 2, 0, 4, 5);
            TestEdges(g, 3, 0, 4, 5);
            TestEdges(g, 4, 1, 2, 3);
            TestEdges(g, 5, 1, 2, 3);
        });

        TestG6("FqOxo", 7, (g) =>
       {
           TestEdges(g, 0, 1, 2);
           TestEdges(g, 1, 0, 3, 4);
           TestEdges(g, 2, 0, 5, 6);
           TestEdges(g, 3, 1, 5, 6);
           TestEdges(g, 4, 1, 5, 6);
           TestEdges(g, 5, 2, 3, 4);
           TestEdges(g, 6, 2, 3, 4);
       });
        TestG6("U???GOOGIGPB`SQgg[cEoKaagg@iI@w@`ROCrGA?", 22, (g) =>
        {
            TestEdges(g, 0, 11, 12, 15, 17, 21);
            TestEdges(g, 1, 10, 13, 14, 20, 21);
            TestEdges(g, 2, 9, 16, 17, 18, 19);
            TestEdges(g, 3, 8, 16, 18, 19, 20);
            TestEdges(g, 4, 7, 11, 13, 17, 19, 21);
            TestEdges(g, 5, 6, 10, 12, 18, 19, 21);
            TestEdges(g, 6, 5, 13, 14, 15, 16, 20);
            TestEdges(g, 7, 4, 12, 14, 15, 18, 20);
            TestEdges(g, 8, 3, 9, 13, 14, 17, 21);
            TestEdges(g, 9, 2, 8, 11, 12, 15, 20);
            TestEdges(g, 10, 1, 5, 11, 15, 16, 17);
            TestEdges(g, 11, 0, 4, 9, 10, 14, 18);
            TestEdges(g, 12, 0, 5, 7, 9, 13, 16);
            TestEdges(g, 13, 1, 4, 6, 8, 12, 18);
            TestEdges(g, 14, 1, 6, 7, 8, 11, 19);
            TestEdges(g, 15, 0, 6, 7, 9, 10, 19);
            TestEdges(g, 16, 2, 3, 6, 10, 12, 21);
            TestEdges(g, 17, 0, 2, 4, 8, 10, 20);
            TestEdges(g, 18, 2, 3, 5, 7, 11, 13);
            TestEdges(g, 19, 2, 3, 4, 5, 14, 15);
            TestEdges(g, 20, 1, 3, 6, 7, 9, 17);
            TestEdges(g, 21, 0, 1, 4, 5, 8, 16);
        });
        Console.WriteLine("All G6 tests passed");
    }
}

