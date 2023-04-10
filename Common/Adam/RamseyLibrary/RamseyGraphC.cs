namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyGraphC : IRamseyGraph
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RamseyGraphC(RamseyConfig ramseyConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Config = ramseyConfig;
        }

        public RamseyConfig Config { get; }
        // Results
        public bool IsSuccess { get; private set; }
        public TimeSpan TimeTaken { get; private set; }
        public int Iterations { get; private set; }
        public List<Solution> Solutions { get; private set; }

        public void InitializeGraph()
        {
            if ((Config.NodeCount < 6) || (Config.NodeCount % 2 != 0))
            {
                throw new InvalidOperationException("Config.NodeCount must be an even number greater than or equal to 6.");
            }

            if ((Config.NodeEdgeCount is null) || (Config.NodeEdgeCount < 2))
            {
                throw new InvalidOperationException("Config.NodeEdgeCount is mandatory and needs to be greater then or equal to 2.");
            }

            // We are taking edges as our "base", not nodes. So, we have half as many of those
            var edgeSymmetryCount = Config.NodeCount / 2;

            // Each edge will be linked to 1 or more other edges symmetrically. We can't go more than half way round.
            var halfNodeCount = (edgeSymmetryCount % 2 == 0) ? edgeSymmetryCount / 2 : (edgeSymmetryCount - 1) / 2;

            var totalEdgeCount = Config.NodeCount * (int)Config.NodeEdgeCount / 2;
            if (totalEdgeCount % 12 != 0)
            {
                throw new InvalidOperationException("For my first iteration of this code, the total numbers of edges has to be divisible by 12.");
            }

            // The number of "forward" 4-loops from each edge will be the total edge count divided by 6.
            // This will result in one of the edges of a loop being in multiple loops.
            var edgeLinkCount = totalEdgeCount / 12;

            Iterations = 0;
            IsSuccess = false;
            Solutions = new List<Solution>();
            var startTime = DateTime.UtcNow;

            // The edgeLinks are the links to "other" nodes that every node will have.
            // e.g. If this has 3 entries of 1, 2 & 4, It would mean that node number x needs to be linked to nodes x+1, x+2 & x+4
            var edgeLinks = new int[edgeLinkCount];

            // Initialize Edge Links
            for (var loop = 0; loop < edgeLinkCount; loop++)
            {
                edgeLinks[loop] = loop + 1;
            }

            var matrixGraph = new MatrixGraphAdam(Config.NodeCount);

            while (true)
            {
                Iterations++;
                // Clear down graph
                matrixGraph.Clear();

                for (var node1Index = 0; node1Index < Config.NodeCount; node1Index += 2)
                {
                    // Set the "base" edges: 0-1, 2-3, 4-5, etc.
                    matrixGraph.SetEdgeValue(node1Index, node1Index + 1, true);

                    // Connect each edge to another "forward" edge for each edgeLink
                    for (var edgeIndex = 0; edgeIndex < edgeLinkCount; edgeIndex++)
                    {
                        var node2Index = (node1Index + edgeLinks[edgeIndex] * 2) % Config.NodeCount;
                        var node3Index = node2Index + 1 % Config.NodeCount;
                        matrixGraph.SetEdgeValue(node1Index, node2Index, true);
                        matrixGraph.SetEdgeValue(node1Index + 1, node3Index, true);
                    }
                }

                //var adamtest2 = 0;
                //var adamTest1 = G6.fromGraph(matrixGraph);
                //adamtest2++;

                if (TryAllCrossLoops(matrixGraph, edgeLinks, edgeLinkCount, edgeSymmetryCount))
                {
                    if (!Config.FindAllSolutions)
                    {
                        TimeTaken = DateTime.UtcNow - startTime;
                        return;
                    }
                }

                // Try the next edgeLink setting
                var maxGoodEdgeIndex = edgeLinkCount - 1;
                var maxAllowedEdgeLinkValue = halfNodeCount;

                while (true)
                {
                    if (edgeLinks[maxGoodEdgeIndex] < maxAllowedEdgeLinkValue)
                    {
                        edgeLinks[maxGoodEdgeIndex]++;

                        for (var edgeIndex2 = maxGoodEdgeIndex + 1; edgeIndex2 < edgeLinkCount; edgeIndex2++)
                        {
                            edgeLinks[edgeIndex2] = edgeLinks[edgeIndex2 - 1] + 1;
                        }

                        break;
                    }

                    maxGoodEdgeIndex--;
                    maxAllowedEdgeLinkValue--;
                    if (maxGoodEdgeIndex < 0)
                    {
                        TimeTaken = DateTime.UtcNow - startTime;
                        return;
                    }
                }
            }
        }

        private bool TryAllCrossLoops(MatrixGraphAdam matrixGraph, int[] edgeLinks, int edgeLinkCount, int edgeSymmetryCount)
        {
            var crossLoop = new CrossLoop();
            var maxStackDepth = edgeLinkCount; // Not really sure how deep to go? In the first two examples, it worked with edgeLinkCount;
            var crossLoopStack = new Stack<CrossLoop>();
            while (true)
            {
                //Iterations++;

                if (!IdentifyInvalidCliques(matrixGraph.edges))
                {
                    IsSuccess = true;
                    Solutions.Add(new Solution(matrixGraph));

                    if (!Config.FindAllSolutions)
                    {
                        return true;
                    }
                }

                // If the stack ain't full, push to it
                if (crossLoopStack.Count < maxStackDepth)
                {
                    crossLoopStack.Push(crossLoop);
                }
                else
                {
                    // Else recross the loop to revert to the original loop
                    CrossTheLoop(matrixGraph, crossLoop, edgeLinks);
                }

                var isIncremented = false;
                while (!isIncremented)
                {
                    if (crossLoop.EdgeLinkIndex < edgeLinkCount - 1)
                    {
                        crossLoop.EdgeLinkIndex++;
                        isIncremented = true;
                    }
                    else if (crossLoop.EdgeIndex < edgeSymmetryCount - 1)
                    {
                        crossLoop.EdgeIndex++;
                        crossLoop.EdgeLinkIndex = 0;
                        isIncremented = true;
                    }
                    else
                    {
                        crossLoop = crossLoopStack.Pop();

                        // Once we've popped the last item, there's no need to continue because the first push is totally symmetrical
                        if (crossLoopStack.Count == 0)
                        {
                            return false;
                        }
                    }
                }

                CrossTheLoop(matrixGraph, crossLoop, edgeLinks);
            }
        }

        private void CrossTheLoop(IGraph matrixGraph, CrossLoop crossLoop, int[] edgeLinks)
        {
            var nodeFromIndex1 = (crossLoop.EdgeIndex * 2) % Config.NodeCount;
            var nodeFromIndex2 = (nodeFromIndex1 + 1) % Config.NodeCount;

            var nodeToIndex1 = (nodeFromIndex1 + (edgeLinks[crossLoop.EdgeLinkIndex] * 2)) % Config.NodeCount;
            var nodeToIndex2 = (nodeToIndex1 + 1) % Config.NodeCount;

            // Cross the loop
            matrixGraph.SetEdgeValue(nodeFromIndex1, nodeToIndex1, false);
            matrixGraph.SetEdgeValue(nodeFromIndex2, nodeToIndex2, false);
            matrixGraph.SetEdgeValue(nodeFromIndex1, nodeToIndex2, true);
            matrixGraph.SetEdgeValue(nodeFromIndex2, nodeToIndex1, true);
        }

        public bool IdentifyInvalidCliques(bool[,] edges)
        {
            var clique = new int[Config.NodeCount];
            if (FindCliques(edges, clique, 0, 0, Config.MaxCliqueOn, true))
            {
                return true;
            }
            clique = new int[Config.NodeCount];
            return FindCliques(edges, clique, 0, 0, Config.MaxCliqueOff, false);
        }

        // Function to check if the given set of vertices
        // in store array is a clique or not
        private bool IsClique(bool[,] edges, int[] clique, int b, bool onOffCheck)
        {
            // Run a loop for all the set of edges
            // for the select node
            for (int i = 0; i < b; i++)
            {
                for (int j = i + 1; j < b; j++)
                {
                    // If any edge is missing
                    if (edges[clique[i], clique[j]] != onOffCheck)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Function to find all the cliques of size s
        private bool FindCliques(bool[,] edges, int[] clique, int i, int l, int s, bool onOffCheck)
        {
            // Check if any vertices from i+1 can be inserted
            //            for (int j = i; j < Config.NodeCount - (s - l); j++)

            for (int j = i; j < Config.NodeCount; j++)
            {
                // Add the node to clique
                clique[l] = j;

                // If the graph is not a clique of size k then it cannot be a clique by adding another edge
                if (IsClique(edges, clique, l + 1, onOffCheck))
                {
                    // If the length of the clique is still less than the desired size
                    if (l + 1 < s)
                    {
                        // Recursion to add vertices
                        if (FindCliques(edges, clique, j + 1, l + 1, s, onOffCheck))
                        {
                            return true;
                        }
                    }
                    // Size is met
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
