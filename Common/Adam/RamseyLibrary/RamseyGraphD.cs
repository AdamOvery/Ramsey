using Pascal;

namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyGraphD : IRamseyGraph
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RamseyGraphD(RamseyConfig ramseyConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (ramseyConfig.NodeEdgeCount is null)
            {
                throw new InvalidOperationException("NodeEdgeCount is mandatory");
            }
            Config = ramseyConfig;
        }

        public RamseyConfig Config { get; }
        public bool IsSuccess { get; private set; }
        public TimeSpan TimeTaken { get; private set; }
        public int Iterations { get; private set; }
        public List<Solution> Solutions { get; private set; }

        public void InitializeGraph()
        {
            Iterations = 0;
            IsSuccess = false;
            Solutions = new List<Solution>();
            var startTime = DateTime.UtcNow;

            // The first node will be linked to nodeEdgeCount other nodes. The next will be linked to nodeEdgeCounts - 1, etc.
            // These will all be symmetric, so no need to repeat those
            var fullySymmetricEdgeCount = ((Config.NodeEdgeCount + 1) * Config.NodeEdgeCount) / 2;
            var fullySymmetricEdgeIndex = 0;
            var matrixGraph = new MatrixGraphAdam(Config.NodeCount);
            var edgesAll = GetEdgeDictionary(Config.NodeCount);
            var edgesById = new Dictionary<int, RamseyEdge>();
            foreach (var ramseyEdge in edgesAll.Values)
            {
                if (ramseyEdge is not null)
                {
                    edgesById.Add(ramseyEdge.Id, ramseyEdge);
                }
            }
            var edgeCount = edgesAll.Count;
            var edgeIndex = 0;
            var edgeGroupStack = new Stack<List<RamseyEdge>>();

            while (true)
            {
                while (edgeIndex < edgeCount)
                {
                    Iterations++;
                    var edgeGroup = new List<RamseyEdge>();
                    var edge = edgesAll[edgeIndex];
                    if ((!edge.IsInvalid) && (!matrixGraph.edges[edge.Node1Index, edge.Node2Index]))
                    {
                        // Only attempt to add an edge if neither node is aleady "full"
                        if ((matrixGraph.nodeCounts[edge.Node1Index] < Config.NodeEdgeCount) && (matrixGraph.nodeCounts[edge.Node2Index] < Config.NodeEdgeCount))
                        {
                            if (!TryAddEdge(matrixGraph, edgeGroup, edge, Config, edgesById))
                            {
                                ReverseEdgeGroup(matrixGraph, edgeGroup);
                                edgeGroup.Clear();
                            }
                            else if (fullySymmetricEdgeCount > 0)
                            {
                                // For debugging purposes
                                var graphG6A = G6.fromGraph(matrixGraph);

                                fullySymmetricEdgeCount--;
                                fullySymmetricEdgeIndex = edgeIndex;
                            }
                        }
                    }

                    edgeGroupStack.Push(edgeGroup);
                    edgeIndex++;
                }

                // Let's only bother to check "Off" cliques if all nodes have at least the minimum number of edges
                if (matrixGraph.nodeCounts.Min() >= Config.MinNodeEdgeCount)
                {
                    if (!CliqueFinder.IdentifyInvalidCliques(matrixGraph.edges, Config))
                    {
                        IsSuccess = true;
                        TimeTaken = DateTime.UtcNow - startTime;
                        Solutions.Add(new Solution(matrixGraph));

                        if (!Config.FindAllSolutions)
                        {
                            return;
                        }
                    }
                }

                List<RamseyEdge> lastEdgeGroup;
                do
                {
                    // The first node edge count edges will be totally symmetric, so no need to change them
                    if (edgeGroupStack.Count <= fullySymmetricEdgeIndex)
                    {
                        TimeTaken = DateTime.UtcNow - startTime;
                        return;
                    }

                    edgeIndex--;
                    lastEdgeGroup = edgeGroupStack.Pop();
                } while (lastEdgeGroup.Count == 0);

                // Reverse that group, replace it with an empty group and increment the edgeIndex
                ReverseEdgeGroup(matrixGraph, lastEdgeGroup);
                // For debugging purposes
                //var graphG6B = G6.fromGraph(matrixGraph);
                var emptyEdgeGroup = new List<RamseyEdge>();
                edgeGroupStack.Push(emptyEdgeGroup);
                edgeIndex++;
            }
        }

        private static void ReverseEdgeGroup(MatrixGraphAdam matrixGraph, List<RamseyEdge> edgeGroup)
        {
            foreach (var edge in edgeGroup)
            {
                if (edge.IsInvalid)
                {
                    edge.IsInvalid = false;
                }
                else
                {
                    matrixGraph.SetEdgeValue(edge.Node1Index, edge.Node2Index, false);
                }             
            }
        }

        private static bool TryAddEdge(MatrixGraphAdam graph, List<RamseyEdge> edgeGroup, RamseyEdge edge, RamseyConfig config, Dictionary<int, RamseyEdge> edgesById)
        {
            var node1Connections = GetNodeConnections(graph, edge.Node1Index, edge.Node2Index, config);
            if (node1Connections is null)
            {
                return false;
            }
            var node2Connections = GetNodeConnections(graph, edge.Node2Index, edge.Node1Index, config);
            if (node2Connections is null)
            {
                return false;
            }

            // Damn it! We can't assume all 3-loops become 4-loops. That is a gross over-simplifcation
            // If we want to do that, we'll need to start with the loops, not the edges.

            //var loopEdges = new List<RamseyEdge>();
            //// Add loop edges where both nodes are connected to other nodes
            //foreach (var node1Connection in node1Connections)
            //{
            //    foreach (var node2Connection in node2Connections)
            //    {
            //        var edgeId = RamseyEdge.GetId(node1Connection, node2Connection, config.NodeCount);
            //        var ramseyEdge = edgesById[edgeId];

            //        // If a loop edge is invalid, we need to return false
            //        if (ramseyEdge.IsInvalid)
            //        {
            //            return false;
            //        }

            //        loopEdges.Add(ramseyEdge);
            //    }
            //}

            //// Add loop edges for all the secondary nodes of node 1
            //foreach (var node1Connection in node1Connections)
            //{
            //    if (!AddSecondaryNodeConnectionsToLoopEdges(graph, node1Connection, edge.Node2Index, config, edgesById, loopEdges))
            //    {
            //        return false;
            //    }
            //}

            //// Add loop edges for all the secondary nodes of node 2
            //foreach (var node2Connection in node2Connections)
            //{
            //    if (!AddSecondaryNodeConnectionsToLoopEdges(graph, node2Connection, edge.Node1Index, config, edgesById, loopEdges))
            //    {
            //        return false;
            //    }
            //}

            // Add main edge
            edgeGroup.Add(edge);
            graph.SetEdgeValue(edge.Node1Index, edge.Node2Index, true);

            // Mark up 3-cliques edges from node 1 connections to node 2 as invalid
            foreach (var node1Connection in node1Connections)
            {
                var edgeId = RamseyEdge.GetId(node1Connection, edge.Node2Index, config.NodeCount);
                var ramseyEdge = edgesById[edgeId];

                ramseyEdge.IsInvalid = true;
                edgeGroup.Add(ramseyEdge);
            }

            // Mark up 3-cliques edges from node 2 connections to node 1 as invalid
            foreach (var node2Connection in node2Connections)
            {
                var edgeId = RamseyEdge.GetId(node2Connection, edge.Node1Index, config.NodeCount);
                var ramseyEdge = edgesById[edgeId];

                ramseyEdge.IsInvalid = true;
                edgeGroup.Add(ramseyEdge);
            }

            //// Try to mark up 4-loop edges as "On"
            //foreach (var loopEdge in loopEdges)
            //{
            //    // Recheck the edge is not "on" because it could have been set in a previous recursive call to ReverseEdgeGroup
            //    if (!graph.edges[loopEdge.Node1Index, loopEdge.Node2Index])
            //    {
            //        if (!TryAddEdge(graph, edgeGroup, loopEdge, config, edgesById))
            //        {
            //            ReverseEdgeGroup(graph, edgeGroup);
            //            return false;
            //        }
            //    }
            //}

            return true;
        }

        private static bool AddSecondaryNodeConnectionsToLoopEdges(MatrixGraphAdam graph, int nodeIndex, int otherNodeIndex, RamseyConfig config,
            Dictionary<int, RamseyEdge> edgesById, List<RamseyEdge> loopEdges)
        {
            var connections = new List<int>();
            for (var connectedNodeIndex = 0; connectedNodeIndex < config.NodeCount; connectedNodeIndex++)
            {
                if (graph.GetEdgeValue(connectedNodeIndex, nodeIndex))
                {
                    // If it's already set, don't worry about it.
                    if (!graph.GetEdgeValue(connectedNodeIndex, otherNodeIndex))
                    {
                        // Find edgeId and see if it is invalid
                        var edgeId = RamseyEdge.GetId(connectedNodeIndex, otherNodeIndex, config.NodeCount);
                        var ramseyEdge = edgesById[edgeId];

                        if (ramseyEdge.IsInvalid)
                        {
                            return false;
                        }

                        loopEdges.Add(ramseyEdge);
                    }
                }
            }

            return true;
        }

        private static List<int>? GetNodeConnections(MatrixGraphAdam graph, int nodeIndex, int otherNodeIndex, RamseyConfig config)
        {
            var connections = new List<int>();
            for (var connectedNodeIndex = 0; connectedNodeIndex < config.NodeCount; connectedNodeIndex++)
            {
                if (graph.GetEdgeValue(connectedNodeIndex, nodeIndex))
                {
                    // If adding this connection will cause an "On" 3-clique, return null
                    if (graph.GetEdgeValue(connectedNodeIndex, otherNodeIndex))
                    {
                        return null;
                    }
                    connections.Add(connectedNodeIndex);
                }
            }

            // If the node is already full, return null
            if (connections.Count >= config.NodeEdgeCount)
            {
                return null;
            }

            return connections;
        }

        private static Dictionary<int, RamseyEdge> GetEdgeDictionary(int nodeCount)
        {
            var edges = new Dictionary<int, RamseyEdge>();

            var edgeIndex = 0;
            for (var node1 = 0; node1 < nodeCount; node1++)
            {
                for (var node2 = node1 + 1; node2 < nodeCount; node2++)
                {
                    var edge = new RamseyEdge(node1, node2, nodeCount);
                    edges.Add(edgeIndex, edge);
                    edgeIndex++;
                }
            }

            return edges;
        }
    }
}
