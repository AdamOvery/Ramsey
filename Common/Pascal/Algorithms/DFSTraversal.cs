

using Pascal;

static class DFSTraversal
{
    public static void DFS(this IGraph g, OnNodeIdVisited onNodeIdVisited)
    {
        int order = g.order;
        bool[] visited = new bool[order];
        var traverseFrom = new Action<int, int?>((a, b) => { });

        traverseFrom = (int start, int? parentNode) =>
        {
            visited[start] = true;
            bool first = true;

            for (var i = 0; i < order; i++)
            {
                if (g.GetEdgeValue(start, i))
                {
                    if (first)
                    {
                        first = false;
                        onNodeIdVisited(start, parentNode);
                    }
                    if (!visited[i]) traverseFrom(i, start);
                }
            }
        };

        for (int i = 0; i < order; i++)
        {
            if (!visited[i])
            {
                traverseFrom(i, null);
            }
        }
    }

    public static void DFS(this ISubGraph subGraph, OnNodeVisited onNoteVisited)
    {
        int graphOrder = subGraph.graphOrder;
        bool[] visited = new bool[graphOrder];
        var traverseFrom = new Action<INode, INode?>((a, b) => { });

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
        TestEngine.Test("DFS Using IGraph", () =>
        {
            IGraph g = G6.parse("K~{???A????S");
            g.DFS((int node, int? parentNode) =>
            {
                Console.WriteLine($"DFS node:{node} parent:{parentNode}");
            });
        });
        TestEngine.Test("DFS Using ISubGraph", () =>
        {
            ISubGraph g = G6.parse("K~{???A????S").AsSubGraph();
            g.DFS((INode visitedNode, INode? parentNode) =>
            {
                Console.WriteLine($"DFS node:{visitedNode.id} parent:{parentNode?.id}");
            });
        });
    }
}
