using Pascal;

static class RandomGraph
{
    internal static void Tests()
    {
        IGraph g = Random(5);
        Console.WriteLine(g.ToString("Random Graph(5)"));
    }

    public static IGraph Random(int order, double probability = 0.5, IGraphFactory? factory = null)
    {
        if (factory == null) factory = MatrixGraph.factory;
        var g = factory.newGraph(order);
        var rnd = new Random();
        for (var b = 1; b < order; b++)
        {
            for (var a = 0; a < b; a++)
            {
                if (rnd.NextDouble() < probability)
                {
                    g.SetEdgeValue(a, b, true);
                }
            }
        }
        return g;
    }

}
