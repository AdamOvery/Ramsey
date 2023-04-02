using Pascal;
using static Pascal.TestEngine;

static class CutVerticesAlgorithm
{

    public class CutVertex
    {
        public readonly INode articulationNode;
        public readonly List<ISubGraph> subGraphs;

        public CutVertex(INode articulationNode)
        {
            this.articulationNode = articulationNode;
            subGraphs = new List<ISubGraph>();
        }

        public override string ToString()
        {
            return $"{articulationNode.id} {string.Join(" ", subGraphs)}";
        }
    }


    public static List<CutVertex> CutVertices(this ISubGraph subGraph)
    {
#if DEBUG
        Assert("CutVertices only works on Connected Graphs", () => subGraph.IsConnected());
#endif
        var result = new List<CutVertex>();

        var traverseFrom = new Action<INode>((start) => { });

        foreach (var cutNode in subGraph.nodes)
        {
            if (cutNode.adjacentNodes.Count <= 1) continue;

            bool[] visited = new bool[subGraph.graph.order];
            List<INode>? subGraphNodes = null;

            traverseFrom = (INode start) =>
            {
                visited[start.id] = true;
                subGraphNodes!.Add(start);
                foreach (var n in start.adjacentNodes)
                {
                    if (n == cutNode) continue;
                    if (!visited[n.id]) traverseFrom(n);
                }
            };

            CutVertex? cutVertex = null;

            foreach (var n2 in cutNode.adjacentNodes)
            {
                if (!visited[n2.id])
                {
                    subGraphNodes = new List<INode>() { cutNode };

                    traverseFrom(n2);

                    if (subGraphNodes.Count < subGraph.nodes.Count)
                    {
                        if (cutVertex == null)
                        {
                            cutVertex = new CutVertex(cutNode);
                            result.Add(cutVertex);
                        }
                        cutVertex.subGraphs.Add(new SubGraph(subGraph, subGraphNodes));
                    }

                }
            }
        }
        return result;
    }

    internal static void Tests()
    {
        Test("Minimal CutVertices", () =>
        {
            ISubGraph graph = G6.parse("Bg").AsSubGraph();
            var cutVertices = graph.CutVertices();
            foreach (var cutVertex in cutVertices) Console.WriteLine($" - CutVertex:{cutVertex}");
            AssertEquals("CutVertices.Count", () => cutVertices.Count, 1);
            AssertEquals("CutVertex 0 is", () => cutVertices[0].ToString(), "1 0-1 1-2");
        });
        Test("Medium CutVertices", () =>
        {
            ISubGraph graph = G6.parse("D{c").AsSubGraph();
            var cutVertices = graph.CutVertices();
            foreach (var cutVertex in cutVertices) Console.WriteLine($" - CutVertex:{cutVertex}");
            AssertEquals("We have one cutVertices", () => cutVertices.Count, 1);
            AssertEquals("CutVertex 0 is", () => cutVertices[0].ToString(), "0 0-1-2 0-3-4");
        });
        Test("Bigger CutVertices", () =>
        {
            ISubGraph graph = G6.parse("FhMCG").AsSubGraph();
            var cutVertices = graph.CutVertices();
            foreach (var cutVertex in cutVertices) Console.WriteLine($" - CutVertex:{cutVertex}");
            AssertEquals("We have three cutVertices", () => cutVertices.Count, 3);
            AssertEquals("CutVertex 0 is", () => cutVertices[0].ToString(), "0 0-1-2-3-4 0-5-6");
            AssertEquals("CutVertex 1 is", () => cutVertices[1].ToString(), "1 0-1-5-6 1-2-3-4");
            AssertEquals("CutVertex 2 is", () => cutVertices[2].ToString(), "2 0-1-2-5-6 2-3-4");
        });
        Test("No CutVertices", () =>
        {
            ISubGraph graph = G6.parse("FhMKG").AsSubGraph();
            AssertEquals("We have no cutVertices", () => graph.CutVertices().Count, 0);
        });
    }

}
