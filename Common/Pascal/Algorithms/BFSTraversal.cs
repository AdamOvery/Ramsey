namespace Pascal;

static class BFSTraversal
{

    public static void BFS(this IGraph g, OnNodeIdVisited onNodeIdVisited)
    {
        int order = g.order;
        bool[] visited = new bool[order];
        var traverseFrom = new Action<int, int?>((a, b) => { });

        traverseFrom = (int start, int? parentNode) =>
        {
            Queue<int> queue = new Queue<int>();
            visited[start] = true;
            queue.Enqueue(start);
            bool first = true;
            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                for (var i = 0; i < order; i++)
                {
                    if (g.GetEdgeValue(start, i))
                    {
                        if (!visited[i])
                        {
                            if (first)
                            {
                                first = false;
                                onNodeIdVisited(start, parentNode);
                            }
                            visited[i] = true;
                            onNodeIdVisited(i, current);
                            queue.Enqueue(i);
                        }
                    }
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

    public static void BFS(this ISubGraph subGraph, OnNodeVisited onNodeVisited)
    {
        int graphOrder = subGraph.graphOrder;
        bool[] visited = new bool[graphOrder];
        var traverseFrom = new Action<INode, INode?>((a, b) => { });

        traverseFrom = (INode start, INode? parentNode) =>
        {
            var queue = new Queue<INode>();
            visited[start.id] = true;
            queue.Enqueue(start);
            bool first = true;
            while (queue.Count > 0)
            {
                INode current = queue.Dequeue();

                foreach (var n in current.adjacentNodes)
                {
                    if (!visited[n.id])
                    {
                        if (first)
                        {
                            first = false;
                            onNodeVisited(start, parentNode);
                        }
                        visited[n.id] = true;
                        onNodeVisited(n, current);
                        queue.Enqueue(n);
                    }
                }
            }
        };
        foreach (var n in subGraph.nodes)
        {
            if (!visited[n.id]) traverseFrom(n, null);
        }
    }

    internal static void Tests()
    {
        TestEngine.Test("BFS Using IGraph", () =>
        {
            IGraph g = G6.parse("K~{???A????S");
            g.BFS((int node, int? parentNode) =>
            {
                Console.WriteLine($"BFS node:{node} parent:{parentNode}");
            });
        });
        TestEngine.Test("BFS Using ISubGraph", () =>
        {
            ISubGraph g = G6.parse("K~{???A????S").AsSubGraph();
            g.BFS((INode visitedNode, INode? parentNode) =>
            {
                Console.WriteLine($"BFS node:{visitedNode.id} parent:{parentNode?.id}");
            });
        });
    }
}

