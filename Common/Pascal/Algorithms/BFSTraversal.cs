

using Pascal;

static class BFSTraversal
{

    public static void BFS(this IGraph g, OnNodeVisited onNodeVisited)
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
                                onNodeVisited(start, parentNode);
                            }
                            visited[i] = true;
                            onNodeVisited(i, current);
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

    public static void BFS(this SubGraph nodes, OnNodeVisited onNodeVisited)
    {

    }
}

