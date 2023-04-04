using Pascal;
using static Pascal.TestEngine;

static class ConnectedGroup
{
    public static List<ISubGraph> ConnectedGroups(this ISubGraph subGraph)
    {
        var result = new List<List<INode>>();
        List<INode>? currentSubGraph = null;
        subGraph.BFS((visitedNode, parentNode) =>
        {
            if (parentNode == null)
            {
                currentSubGraph = new List<INode>();
                result.Add(currentSubGraph);
            }
            currentSubGraph!.Add(visitedNode);
        });
        return result.Select(g => subGraph.CreateSubGraph(g)).ToList();
    }

    internal static void Tests()
    {
        Test("Connected groups", () =>
        {
            ISubGraph graph = G6.parse("K~{???A????S").AsSubGraph();
            var connectedGroups = graph.ConnectedGroups();
            foreach (var group in connectedGroups) Console.WriteLine($" - group:{group}");
            AssertEquals("We have two groups", () => connectedGroups.Count, 2);
            AssertEquals("Group 0 is", () => connectedGroups[0].ToString(), "0-1-2-3-4");
            AssertEquals("Group 1 is", () => connectedGroups[1].ToString(), "6-8-11");
        });
    }
}
