

static class LongestCycleSearch
{
    public static IEnumerable<int>? LongestCycle(this IGraph g, int startingNode)
    {
        int order = g.order;
        int[] visited = new int[order];
        int[] path = new int[order];
        var traverseFrom = new Action<int, int>((start, level) => { });
        int length = 0;
        int bestLength = 2; // a cycle of length 2 is not a cycle so we'll start at 3 
        int[]? bestPath = null;

        traverseFrom = (int start, int level) =>
        {

            visited[start] = level;
            path[level - 1] = start;

            for (var i = 0; i < order; i++)
            {
                if (g.GetEdgeValue(start, i))
                {
                    if (visited[i] > 0)
                    {
                        length = level - visited[i] + 1;
                        if (length > bestLength)
                        {
                            bestLength = length;
                            bestPath = new int[length];
                            Array.Copy(path, visited[i] - 1, bestPath, 0, length);
                        }
                    }
                    else
                    {
                        traverseFrom(i, level + 1);
                    }
                }
            }
            visited[start] = 0;
        };
        traverseFrom(startingNode, 1);
        return bestPath;

    }

}
