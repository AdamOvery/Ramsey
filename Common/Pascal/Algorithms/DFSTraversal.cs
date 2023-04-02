

static class DFSTraversal
{
    public static void DFS(this IGraph g, OnNodeVisited onNodeVisited)
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
                        onNodeVisited(start, parentNode);
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

}
