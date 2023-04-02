using Pascal;
using static Pascal.TestEngine;

static class IsConnectedAlgorithm
{
    public static bool IsConnected(this ISubGraph subGraph)
    {
        if (subGraph.nodes.Any(n => n.adjacentNodes.Count < 1)) return false;
        var connectedGroups = subGraph.ConnectedGroups();
        return (connectedGroups.Count == 1);
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
