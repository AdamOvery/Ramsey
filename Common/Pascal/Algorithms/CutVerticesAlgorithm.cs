using Pascal;
using static Pascal.TestEngine;

static class CutVerticesAlgorithm
{

    public class CutVerticesResult
    {
        public readonly List<INode> articulationNodes;
        public readonly List<ISubGraph> subGraphs;

        public CutVerticesResult()
        {
            articulationNodes = new List<INode>();
            subGraphs = new List<ISubGraph>();
        }

        public override string ToString()
        {
            return $"[{string.Join(",", articulationNodes)}] {string.Join(" ", subGraphs)}";
        }
    }


    public static CutVerticesResult CutVertices(this ISubGraph subGraph)
    {
#if DEBUG
        Assert("CutVertices only works on Connected Graphs", () => subGraph.IsConnected());
#endif

        var traverseAvoidingCutNode = new Action<INode>((start) => { });

        var result = new CutVerticesResult();
        var foundOne = false;
        foreach (var cutNode in subGraph.nodes)
        {
            if (cutNode.adjacentNodes.Count <= 1) continue;

            bool[] visited = new bool[subGraph.graph.order];
            List<INode>? subGraphNodes = null;

            traverseAvoidingCutNode = (INode start) =>
            {
                visited[start.id] = true;
                subGraphNodes!.Add(start);
                foreach (var n in start.adjacentNodes)
                {
                    if (n == cutNode) continue;
                    if (!visited[n.id]) traverseAvoidingCutNode(n);
                }
            };


            foreach (var n2 in cutNode.adjacentNodes)
            {
                if (!visited[n2.id])
                {
                    subGraphNodes = new List<INode>() { cutNode };

                    traverseAvoidingCutNode(n2);

                    if (subGraphNodes.Count < subGraph.nodes.Count)
                    {
                        if (!foundOne)
                        {
                            foundOne = true;
                            result.articulationNodes.Add(cutNode);
                        }
                        var localSubGraph = subGraph.CreateSubGraph(subGraphNodes);
                        var localSubGraphsCutVertices = CutVertices(localSubGraph);
                        if (localSubGraphsCutVertices.subGraphs.Count > 0)
                        {
                            result.articulationNodes.AddRange(localSubGraphsCutVertices.articulationNodes);
                            result.subGraphs.AddRange(localSubGraphsCutVertices.subGraphs);
                        }
                        else result.subGraphs.Add(localSubGraph);
                    }
                }
            }
            if (foundOne) break;
        }
        return result;
    }

    internal static void Tests()
    {
        Test("Minimal CutVertices", () =>
        {
            //        {*} N0
            //          \
            //           \
            //  {*}------{*}
            //  N2        N1
            ISubGraph graph = G6.parse("Bg").AsSubGraph();
            var cutVertices = CutVertices(graph);
            Console.WriteLine($" - cutVertices:{cutVertices}");
            AssertEquals("articulationNodes.Count", () => cutVertices.articulationNodes.Count, 1);
            AssertEquals("subGraphs.Count", () => cutVertices.subGraphs.Count, 2);
            AssertEquals("CutVertex 0 is", () => cutVertices.subGraphs[0].ToString(), "0-1");
            AssertEquals("CutVertex 1 is", () => cutVertices.subGraphs[1].ToString(), "1-2");
        });
        Test("Medium CutVertices", () =>
        {
            //                                  NO                    
            //                               --/--                    
            //                           ---/ / \ \---                
            //                        --/    /   \    \--             
            //             N4     ---/      /     \      \---         
            //                ---/         /   -   \         \---   N1
            //              |/------------/----------------------\|   
            //              | \--        /           \        /-- /   
            //              \    \---   /             \    /--   |    
            //               |       \-/               /---      |    
            //               |        / \--         /-- \        |    
            //               |       /     \--   /--     \       /    
            //               |      /         /---        \     |     
            //               |     /       /--    \--      \    |     
            //               \    /    /---          \--    \   |     
            //                |  /  /--                 \--- \  /     
            //                | //--                        \--|      
            //            N3  |--------------------------------|- N2  

            ISubGraph graph = G6.parse("D{c").AsSubGraph();
            var cutVertices = CutVertices(graph);
            Console.WriteLine($" - cutVertices:{cutVertices}");
            AssertEquals("cutVertices", () => cutVertices.articulationNodes.Count, 1);
            AssertEquals("subGraphs.Count", () => cutVertices.subGraphs.Count, 2);
            AssertEquals("CutVertex 0 is", () => cutVertices.subGraphs[0].ToString(), "0-1-2");
            AssertEquals("CutVertex 1 is", () => cutVertices.subGraphs[1].ToString(), "0-3-4");
        });
        Test("Bigger CutVertices", () =>
        {
            ISubGraph graph = G6.parse("FhMCG").AsSubGraph();
            var cutVertices = CutVertices(graph);
            Console.WriteLine($" - cutVertices:{cutVertices}");
            AssertEquals("cutVertices", () => String.Join("-", cutVertices.articulationNodes.Select(n => n.id.ToString())), "0-1-2");
            AssertEquals("subGraphs.Count", () => cutVertices.subGraphs.Count, 4);
            AssertEquals("CutVertex 0 is", () => cutVertices.subGraphs[0].ToString(), "0-1");
            AssertEquals("CutVertex 1 is", () => cutVertices.subGraphs[1].ToString(), "1-2");
            AssertEquals("CutVertex 2 is", () => cutVertices.subGraphs[2].ToString(), "2-3-4");
            AssertEquals("CutVertex 3 is", () => cutVertices.subGraphs[3].ToString(), "0-5-6");
        });
        Test("No CutVertices", () =>
        {
            ISubGraph graph = G6.parse("FhMKG").AsSubGraph();
            AssertEquals("no cutVertices", () => CutVertices(graph).articulationNodes.Count, 0);
        });
    }

}
