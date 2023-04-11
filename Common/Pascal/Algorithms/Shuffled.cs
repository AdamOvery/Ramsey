using Pascal;

static class ShuffledGraph
{
    static Random rnd = new Random(123456);

    internal static void Tests()
    {
        IGraph g = new MatrixGraph(3);
        g.SetEdgeValue(0, 1, true);
        while (g.GetEdgeValue(0, 1))
        {
            g = g.Shuffled();
        }
        Console.WriteLine(g.ToString("Shuffled N0 to N1"));
    }

    public static IGraph Shuffled(this IGraph graph, IGraphFactory? factory = null)
    {
        var subGraph = graph.AsSubGraph();
        var randomNodeOrder = subGraph.nodes.OrderBy(n => rnd.Next()).ToList();
        // resequence the node Ids
        randomNodeOrder.ForEachIndexed((n, i) => { n.id = i; });
        subGraph.nodes = randomNodeOrder;
        return subGraph.ToGraph(factory);
    }

}
