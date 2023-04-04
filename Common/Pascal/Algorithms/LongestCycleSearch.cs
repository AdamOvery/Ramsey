using Pascal;
using static Pascal.TestEngine;

static class LongestCycleSearch
{
    // inspired from  Johnson's Algorithm - All simple cycles in directed graph
    // https://www.youtube.com/watch?v=johyrWospv0&t=1184s

    private class NodeInfo
    {
        public INode? Parent;
        public int Depth;
    }

    public static ISubGraph? LongestCycle(this ISubGraph subGraph)
    {
        int maxLength = 0;
        List<INode>? longestCycle = null;
        var nodeInfos = new NodeInfo[subGraph.graph.order];

        Action<INode> traverse = (node) => { };

        traverse = (INode node) =>
               {
                   foreach (var n in node.adjacentNodes)
                   {
                       // If this node is already a parent of the current node, skip it
                       if (n == nodeInfos[node.id]?.Parent) continue;

                       if (nodeInfos[n.id] == null)
                       {
                           nodeInfos[n.id] = new NodeInfo { Parent = node, Depth = nodeInfos[node.id]?.Depth + 1 ?? 0 };
                           traverse(n);
                       }
                       else if (nodeInfos[node.id]?.Depth - nodeInfos[n.id].Depth + 1 > maxLength)
                       {
                           // We've found a cycle that's longer than any previous cycle
                           maxLength = nodeInfos[node.id]?.Depth - nodeInfos[n.id].Depth + 1 ?? 0;
                           longestCycle = new List<INode>();
                           var cycleNode = node;
                           while (cycleNode != n)
                           {
                               longestCycle.Add(cycleNode!);
                               cycleNode = nodeInfos[cycleNode!.id].Parent!;
                           }
                           longestCycle.Add(cycleNode);
                       }
                   }
               };

        // Perform depth-first search from each node in the graph
        foreach (var n in subGraph.nodes)
        {
            if (nodeInfos[n.id] == null)
            {
                nodeInfos[n.id] = new NodeInfo { Parent = null, Depth = 0 };
                traverse(n);
            }
        }
        return longestCycle == null ? null : subGraph.CreateSubGraph(longestCycle!);
    }

    internal static void Tests()
    {
        Test("Longest Cycle", () =>
        {
            AssertEquals("Square", () => G6.parse("Cl").AsSubGraph().LongestCycle(), "0-1-2-3");
            AssertEquals("Square with diagonnal", () => G6.parse("Cn").AsSubGraph().LongestCycle(), "0-1-2-3");
            AssertEquals("Shuffled square", () => G6.parse("Cr").AsSubGraph().LongestCycle(), "0-1-2-3");
            AssertEquals("Shuffled square with diagonnal", () => G6.parse("Cr").AsSubGraph().LongestCycle(), "0-1-2-3");

            AssertEquals("Pentagon", () => G6.parse("Dhc").AsSubGraph().LongestCycle(), "0-1-2-3-4");
            AssertEquals("Shuffled pentagon", () => G6.parse("DkK").AsSubGraph().LongestCycle(), "0-1-2-3-4");
            AssertEquals("Yogi bear basket", () => G6.parse("DHs").AsSubGraph().LongestCycle(), "1-2-3-4");
            AssertEquals("A Octogon-Clique with a leg", () => G6.parse("I~~~~{?OG").AsSubGraph().LongestCycle(), "0-1-2-3-4-5-6-7");
            AssertEquals("The same shuffled", () => G6.parse("Iev]}{_vw").AsSubGraph().LongestCycle(), "0-1-3-4-5-6-7-9");
        });
    }

}
