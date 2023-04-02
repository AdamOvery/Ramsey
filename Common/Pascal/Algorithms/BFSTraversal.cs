namespace Pascal;

static class BFSTraversal
{
    public static void BFS(this ISubGraph subGraph, OnNodeVisited onNodeVisited)
    {
        bool[] visited = new bool[subGraph.graph.order];
        var traverseFrom = new Action<INode, INode?>((start, parentNode) => { });

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
        TestEngine.Test("Breadth-first search (BFS) traversal", () =>
        {
            ISubGraph g = G6.parse("K~{???A????S").AsSubGraph();
            g.BFS((INode visitedNode, INode? parentNode) =>
            {
                Console.WriteLine($" - node:{visitedNode.id} parent:{parentNode?.id}");
            });
        });
    }
}

