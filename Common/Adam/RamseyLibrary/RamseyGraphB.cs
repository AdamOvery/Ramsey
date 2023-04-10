namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyGraphB : IRamseyGraph
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RamseyGraphB(RamseyConfig ramseyConfig)
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

        private GraphEdges CreateNewGraphEdges()
        {
            var edges = new bool[Config.NodeCount, Config.NodeCount];
            var nodeLists = new List<int>[Config.NodeCount];
            for (var nodeIndex = 0; nodeIndex < Config.NodeCount; nodeIndex++)
            {
                nodeLists[nodeIndex] = new List<int>();
            }

            return new GraphEdges(edges, nodeLists);
        }

        public void InitializeGraph()
        {
            Iterations = 0;
            IsSuccess = false;
            Solutions = new List<Solution>();
            var startTime = DateTime.UtcNow;

            var graphEdges = CreateNewGraphEdges();

            if (Config.NodeEdgeCount is null)
            {
                throw new InvalidOperationException("NodeEdgeCount is mandatory for RamseyGraphB");
            }

            var edgeLinkCount = (int)((Config.NodeEdgeCount % 2 == 0) ? Config.NodeEdgeCount / 2 : (Config.NodeEdgeCount + 1) / 2);
            var halfNodeCount = (Config.NodeCount % 2 == 0) ? (Config.NodeCount - 2) / 2 : (Config.NodeCount - 1) / 2;

            // The edgeLinks are the links to "other" nodes that every node will have.
            // e.g. If this has 3 entries of 1, 2 & 4, It would mean that node number x needs to be linked to nodes x+1, x+2 & x+4
            var edgeLinks = new int[edgeLinkCount];

            // Initialize Edge Links
            for (var loop = 0; loop < edgeLinkCount; loop++)
            {
                edgeLinks[loop] = loop + 1;
            }

            if (!GetNextValidEdgeLinkArray(edgeLinks, halfNodeCount))
            {
                TimeTaken = DateTime.UtcNow - startTime;
                return;
            }

            // Clear down graph
            Array.Clear(graphEdges.Edges);

            for (var node1Index = 0; node1Index < Config.NodeCount; node1Index++)
            {
                for (var edgeIndex = 0; edgeIndex < edgeLinkCount; edgeIndex++)
                {
                    var node2Index = (node1Index + edgeLinks[edgeIndex]) % Config.NodeCount;

                    if (!graphEdges.Edges[node1Index, node2Index])
                    {
                        ToggleEdge(graphEdges.Edges, graphEdges.NodeLists, node1Index, node2Index);
                    }
                }
            }

            var fullGraphIds = new Dictionary<string, bool>();
            var graphQueueUp = new Queue<GraphEdges>();
            var graphQueueDown = new Queue<GraphEdges>();
            var distances = new int[Config.NodeCount, Config.NodeCount];
            graphEdges.IsDown = (Config.NodeEdgeCount % 2 != 0);

            do
            {
                var nodeClassification = new NodeClassification(Config, graphEdges.NodeLists);

                var nodeIds = new Dictionary<string, List<int>>();
                var minIds = new Dictionary<string, List<int>>();
                var maxIds = new Dictionary<string, List<int>>();

                Array.Clear(distances);
                var isSingleGroup = true;
                for (var nodeIndex = 0; nodeIndex < Config.NodeCount; nodeIndex++)
                {
                    var nodeId = nodeClassification.ClassifyNode(nodeIndex);

                    // Find the distance between all nodes
                    for (var nodeToIndex = 0; nodeToIndex < Config.NodeCount; nodeToIndex++)
                    {
                        distances[nodeIndex, nodeToIndex] = nodeClassification.NodeDistances[nodeToIndex] ?? 0;
                    }

                    if (nodeClassification.NodeCount < Config.NodeCount)
                    {
                        isSingleGroup = false;
                        break;
                    }

                    // Add to generic dictionary
                    if (!nodeIds.ContainsKey(nodeId))
                    {
                        nodeIds.Add(nodeId, new List<int>());
                    }
                    nodeIds[nodeId].Add(nodeIndex);

                    // Add to min and max dictionaries
                    var sizeIds = (graphEdges.NodeLists[nodeIndex].Count == Config.NodeEdgeCount) ? minIds : maxIds;

                    if (!sizeIds.ContainsKey(nodeId))
                    {
                        sizeIds.Add(nodeId, new List<int>());
                    }
                    sizeIds[nodeId].Add(nodeIndex);
                }

                if (isSingleGroup)
                {
                    var fullGraphId = string.Join(",", nodeIds.Values.Select(v => v.Count).OrderBy(v => v).Select(v => v.ToString()).ToList()) +
                        "x" + string.Join(",", nodeIds.Keys.OrderBy(k => k).ToList());

                    if (!fullGraphIds.ContainsKey(fullGraphId))
                    {
                        fullGraphIds.Add(fullGraphId, true);

                        Iterations++;

                        if (!IdentifyInvalidCliques(graphEdges.Edges))
                        {
                            IsSuccess = true;
                            TimeTaken = DateTime.UtcNow - startTime;
                            var solution = new Solution(edgeLinks, graphEdges.Edges);
                            Solutions.Add(solution);

                            if (!Config.FindAllSolutions)
                            {
                                return;
                            }
                        }

                        if (graphEdges.IsDown)
                        {
                            if (!FindEdgesToRemove(graphQueueDown, graphEdges, maxIds))
                            {
                                FindEdgesToAdd(graphQueueUp, graphEdges, minIds, distances);
                            }
                        }
                        else
                        {
                            if (!FindEdgesToAdd(graphQueueUp, graphEdges, minIds, distances))
                            {
                                FindEdgesToRemove(graphQueueDown, graphEdges, maxIds);
                            }
                        }

                        // I think we could create a hash of the fullGraphId above
                        // Add to a dictionary of these, so we can ignore those graphs if we find them again
                        // Loop through each entry and do the above for each one.
                    }
                }

                graphEdges = GetNextGraphEdges(graphQueueUp, graphQueueDown);

            } while (graphEdges is not null);

            TimeTaken = DateTime.UtcNow - startTime;
        }

        private GraphEdges? GetNextGraphEdges(Queue<GraphEdges> graphQueueUp, Queue<GraphEdges> graphQueueDown)
        {
            GraphEdges? graphEdges;
            if (graphQueueUp.TryDequeue(out graphEdges))
            {
                graphEdges.IsDown = false;
                return graphEdges;
            }

            if (graphQueueDown.TryDequeue(out graphEdges))
            {
                graphEdges.IsDown = true;
                return graphEdges;
            }

            return null;
        }

        private bool FindEdgesToAdd(Queue<GraphEdges> graphQueue, GraphEdges graphEdges, Dictionary<string, List<int>> minIds, int[,] distances)
        {
            var result = false;
            var groupFromIndex = 0;
            foreach (var minNodeListsFrom in minIds.Values)
            {
                var groupToIndex = 0;
                foreach (var minNodeListsTo in minIds.Values)
                {
                    // We only need each group pair once, if groupToIndex > groupFromIndex, we'll do that when the two values are swapped over
                    if (groupToIndex > groupFromIndex)
                    {
                        break;
                    }

                    // We need a quick way of doing this. This is probably far too slow.
                    var ramseyEdges = FindFirstLinkToAddAtEachDistance(graphEdges, minNodeListsFrom, minNodeListsTo, distances);

                    foreach (var ramseyEdge in ramseyEdges.Values)
                    {
                        var newGraphEdges = GetNewGraph(graphEdges, ramseyEdge);
                        graphQueue.Enqueue(newGraphEdges);
                        result = true;
                    }

                    groupToIndex++;
                }

                groupFromIndex++;
            }

            return result;
        }

        private bool FindEdgesToRemove(Queue<GraphEdges> graphQueue, GraphEdges graphEdges, Dictionary<string, List<int>> maxIds)
        {
            var result = false;
            var groupFromIndex = 0;
            foreach (var maxNodeListsFrom in maxIds.Values)
            {
                var groupToIndex = 0;
                foreach (var maxNodeListsTo in maxIds.Values)
                {
                    // We only need each group pair once, if groupToIndex > groupFromIndex, we'll do that when the two values are swapped over
                    if (groupToIndex > groupFromIndex)
                    {
                        break;
                    }

                    // We need a quick way of doing this. This is probably far too slow.
                    var ramseyEdge = FindFirstLinkToRemove(graphEdges, maxNodeListsFrom, maxNodeListsTo);

                    if (ramseyEdge is not null)
                    {
                        var newGraphEdges = GetNewGraph(graphEdges, ramseyEdge);
                        graphQueue.Enqueue(newGraphEdges);
                        result = true;
                    }

                    groupToIndex++;
                }

                groupFromIndex++;
            }

            return result;
        }

        private GraphEdges GetNewGraph(GraphEdges graphEdges, RamseyEdge ramseyEdge)
        {
            var newGraphEdges = graphEdges.Clone();
            ToggleEdge(newGraphEdges.Edges, newGraphEdges.NodeLists, ramseyEdge.Node1Index, ramseyEdge.Node2Index);

            return newGraphEdges;
        }

        private static RamseyEdge? FindFirstLinkToRemove(GraphEdges graphEdges, List<int> nodeListsFrom, List<int> nodeListsTo)
        {
            foreach (var nodeIdFrom in nodeListsFrom)
            {
                foreach (var nodeIdTo in nodeListsTo)
                {
                    if (graphEdges.Edges[nodeIdFrom, nodeIdTo])
                    {
                        return new RamseyEdge(nodeIdFrom, nodeIdTo, graphEdges.Edges.GetLength(0));
                    }
                }
            }

            return null;
        }

        private static Dictionary<int, RamseyEdge> FindFirstLinkToAddAtEachDistance(GraphEdges graphEdges, List<int> nodeListsFrom, List<int> nodeListsTo, int[,] distances)
        {
            var ramseyEdges = new Dictionary<int, RamseyEdge>();

            foreach (var nodeIdFrom in nodeListsFrom)
            {
                foreach (var nodeIdTo in nodeListsTo)
                {
                    if (!graphEdges.Edges[nodeIdFrom, nodeIdTo])
                    {
                        var distance = distances[nodeIdFrom, nodeIdTo];
                        if (!ramseyEdges.ContainsKey(distance) && (distance > 0)) // Don't add a link to itself!
                        {
                            ramseyEdges[distance] = new RamseyEdge(nodeIdFrom, nodeIdTo, graphEdges.Edges.GetLength(0));
                        }
                    }
                }
            }

            return ramseyEdges;
        }

        private bool GetNextValidEdgeLinkArray(int[] edgeLinks, int halfNodeCount)
        {
            var index = 0;

            while (index < edgeLinks.Length)
            {
                var previousNode = (index == 0) ? 0 : edgeLinks[index - 1];

                // We can't have a gap of more than Config.MaxCliqueOff, this will result in an invalid "Off" clique
                if (edgeLinks[index] - previousNode > Config.MaxCliqueOff)
                {
                    if (index <= 0)
                    {
                        return false;
                    }

                    // We need to increment the previous node
                    edgeLinks[index - 1]++;

                    // And set the rest to be the ones after
                    for (var nextIndex = index; nextIndex < edgeLinks.Length; nextIndex++)
                    {
                        edgeLinks[nextIndex] = edgeLinks[nextIndex - 1] + 1;
                    }

                    // And go back one index
                    index--;
                    continue;
                }

                // Look "behind" by Config.MaxCliqueOn - 1. If that edgelink value is the original value + Config.MaxCliqueOn
                // Then we will result in an invalid "On" clique
                if (index >= Config.MaxCliqueOn - 1)
                {
                    if (edgeLinks[index] - edgeLinks[index - (Config.MaxCliqueOn - 1)] < Config.MaxCliqueOn)
                    {
                        // We need increment this node
                        edgeLinks[index]++;

                        // And set the rest to be the ones after
                        for (var nextIndex = index + 1; nextIndex < edgeLinks.Length; nextIndex++)
                        {
                            edgeLinks[nextIndex] = edgeLinks[nextIndex - 1] + 1;
                        }

                        // And try again
                        continue;
                    }
                }

                // If the highest edge link is less than or equal to "Half Node Count" - MaxCliqueOff, then we have an "end-gap" too big
                // Which will result in an invalid "Off" clique
                if (index == edgeLinks.Length - 1)
                {
                    if (edgeLinks[edgeLinks.Length - 1] <= halfNodeCount - Config.MaxCliqueOff)
                    {
                        edgeLinks[edgeLinks.Length - 1] = halfNodeCount - Config.MaxCliqueOff + 1;
                        // And try again
                        continue;
                    }
                }

                index++;
            }

            return true;
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

        private void ToggleEdge(bool[,] edges, List<int>[] nodeLists, int node1, int node2)
        {
            if (edges[node1, node2])
            {
                edges[node1, node2] = false;
                edges[node2, node1] = false;

                nodeLists[node1].Remove(node2);
                nodeLists[node2].Remove(node1);
            }
            else
            {
                edges[node1, node2] = true;
                edges[node2, node1] = true;

                nodeLists[node1].Add(node2);
                nodeLists[node2].Add(node1);
            }
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
