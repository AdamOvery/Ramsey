using Pascal;
using static Pascal.TestEngine;

static class SortedGraph
{
    public static IGraph Sorted(this IGraph graph)
    {
        var subGraph = graph.AsSubGraph();
        return subGraph.ToGraph();
    }

    internal static void Tests()
    {
        Test("SortedGraph", () =>
        {
            IGraph graph = G6.parse("K~{???A????S");
            var sortedGraph = graph.Sorted();
            AssertEquals("Graph is unchanged", () => G6.fromGraph(sortedGraph), "K~{???A????S");
        });
    }
}
