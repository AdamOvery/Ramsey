using Pascal;
using static Pascal.TestEngine;

static class LongestCycleSearch
{
    // perhaps inspired from  Johnson's Algorithm - All simple cycles in directed graph
    // https://www.youtube.com/watch?v=johyrWospv0&t=1184s

    public static ISubGraph LongestCycle(this ISubGraph subGraph)
    {
        // bool[] visited = new bool[subGraph.graph.order];
        // var traverseFrom = new Action<INode, INode?>((start, parentNode) => { });

        // traverseFrom = (INode start, INode? parentNode) =>
        // {
        //     visited[start.id] = true;
        //     bool first = true;

        //     foreach (var n in start.adjacentNodes)
        //     {
        //         if (first)
        //         {
        //             first = false;
        //         }
        //         if (!visited[n.id]) traverseFrom(n, start);
        //     }
        // };
        // foreach (var n in subGraph.nodes)
        // {
        //     if (!visited[n.id]) traverseFrom(n, null);
        // }
        return subGraph; // new SubGraph(subGraph, new List<INode>());
    }

    internal static void Tests()
    {
        Test("Longest Subgraph", () =>
        {
            AssertEquals("square", () => G6.parse("Cl").AsSubGraph().LongestCycle().ToString(), "0-1-2-3");
            AssertEquals("square with diagonnal", () => G6.parse("Cn").AsSubGraph().LongestCycle().ToString(), "0-1-2-3");
            AssertEquals("shuffled square", () => G6.parse("Cr").AsSubGraph().LongestCycle().ToString(), "0-1-2-3");
            AssertEquals("shuffled square with diagonnal", () => G6.parse("Cr").AsSubGraph().LongestCycle().ToString(), "0-1-2-3");
        });
    }

}
