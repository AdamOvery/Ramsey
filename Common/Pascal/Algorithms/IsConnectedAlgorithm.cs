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
        Test("IsConnected", () =>
        {

            Assert("completePentagon is a connected", () => G6.parse("D~{").AsSubGraph().IsConnected());
            Assert("incompletePentagon is connected", () => G6.parse("Dhc").AsSubGraph().IsConnected());
            Assert("square with empty node is not connected", () => !G6.parse("Dl?").AsSubGraph().IsConnected());
            Assert("triangle + segment is not connected", () => !G6.parse("DJ_").AsSubGraph().IsConnected());
        });
    }
}
