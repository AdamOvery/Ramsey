using Pascal;
using static Pascal.TestEngine;

static class IsCliqueAlgorithm
{
    public static bool IsClique(this ISubGraph subGraph)
    {
        int nodeCount = subGraph.nodes.Count;

        if (nodeCount <= 1) return true;
        foreach (var n in subGraph.nodes)
        {
            if (n.adjacentNodes.Count != nodeCount - 1) return false;
        }
        return true;
    }

    internal static void Tests()
    {
        Test("IsCliqueAlgorithm", () =>
        {
            ISubGraph completePentagon = G6.parse("D~{").AsSubGraph();
            ISubGraph incompletePentagon = G6.parse("D^{").AsSubGraph();
            ISubGraph completeHexagon = G6.parse("E~~w").AsSubGraph();
            ISubGraph incompleteHexagon = G6.parse("E~nw").AsSubGraph();

            Assert("completePentagon is a clique", () => completePentagon.IsClique());
            Assert("incompletePentagon is not a clique", () => !incompletePentagon.IsClique());
            Assert("completeHexagon is a clique", () => completeHexagon.IsClique());
            Assert("incompleteHexagon is not a clique", () => !incompleteHexagon.IsClique());
        });
    }
}
