

using Pascal;

static class DFSTraversal
{
    public static void DFS(this ISubGraph subGraph, OnNodeVisited onNoteVisited)
    {
        bool[] visited = new bool[subGraph.graph.order];
        var traverseFrom = new Action<INode, INode?>((start, parentNode) => { });

        traverseFrom = (INode start, INode? parentNode) =>
        {
            visited[start.id] = true;
            bool first = true;

            foreach (var n in start.adjacentNodes)
            {
                if (first)
                {
                    first = false;
                    onNoteVisited(start, parentNode);
                }
                if (!visited[n.id]) traverseFrom(n, start);
            }
        };
        foreach (var n in subGraph.nodes)
        {
            if (!visited[n.id]) traverseFrom(n, null);
        }
    }

    internal static void Tests()
    {
        TestEngine.Test("Depth-first search (DFS) traversal", () =>
        {
            ISubGraph g = G6.parse("K~{???A????S").AsSubGraph();
            g.DFS((INode visitedNode, INode? parentNode) =>
            {
                Console.WriteLine($" - node:{visitedNode.id} parent:{parentNode?.id}");
            });
        });
    }
}
