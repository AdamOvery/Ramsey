namespace Ramsey.Adam.RamseyLibrary
{
    public static class NodeLoopUtil
    {
        public static List<NodeLoop> FindAllNodeLoops(bool[,] edges)
        {
            var nodeLoops = new Dictionary<int, NodeLoop>();
            var nodeCount = edges.GetLength(0);

            // build up node lists (these are the same as edges but each node has an list of "on" edges rather than a straight array)
            var nodeLists = new List<int>[edges.GetLength(0)];
            for (var node1index = 0; node1index < nodeCount; node1index++)
            {
                nodeLists[node1index] = new List<int>();
                for (var node2index = 0; node2index < nodeCount; node2index++)
                {
                    if (edges[node1index, node2index])
                    {
                        nodeLists[node1index].Add(node2index);
                    }
                }
            }

            for (var nodeIndex1 = 0; nodeIndex1 < nodeCount; nodeIndex1++)
            {
                foreach (var nodeIndex2 in nodeLists[nodeIndex1])
                {
                    foreach (var nodeIndex3 in nodeLists[nodeIndex2])
                    {
                        if (nodeIndex3 != nodeIndex1)
                        {
                            foreach (var nodeIndex4 in nodeLists[nodeIndex3])
                            {
                                if (nodeLists[nodeIndex4].Contains(nodeIndex1))
                                {
                                    if (nodeIndex4 != nodeIndex2)
                                    {
                                        var nodeLoop = GetNodeLoop(nodeCount, nodeIndex1, nodeIndex2, nodeIndex3, nodeIndex4);

                                        if (!nodeLoops.ContainsKey(nodeLoop.Id))
                                        {
                                            nodeLoops.Add(nodeLoop.Id, nodeLoop);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return nodeLoops.Values.ToList();
        }

        public static List<RamseyEdge> FindAllRamseyEdges(List<NodeLoop> nodeLoops, int nodeCount)
        {
            var ramseyEdges = new Dictionary<int, RamseyEdge>();

            foreach (var nodeLoop in nodeLoops)
            {
                for (var loopIndex = 0; loopIndex < 4; loopIndex++)
                {
                    var ramseyEdge = new RamseyEdge(nodeLoop.Nodes[loopIndex], nodeLoop.Nodes[(loopIndex + 1) % 4], nodeCount);

                    if (!ramseyEdges.ContainsKey(ramseyEdge.Id))
                    {
                        ramseyEdges.Add(ramseyEdge.Id, ramseyEdge);
                    }
                    else
                    {
                        ramseyEdge = ramseyEdges[ramseyEdge.Id];
                        ramseyEdge.Count++;
                    }
                }
            }

            return ramseyEdges.Values.ToList();
        }

        private static int GetProperModulus(int value, int modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }

        private static NodeLoop GetNodeLoop(int nodeCount, int nodeIndex1, int nodeIndex2, int nodeIndex3, int nodeIndex4)
        {
            var tempDictionary = new Dictionary<int, int>
                                {
                                    { 0, nodeIndex1 },
                                    { 1, nodeIndex2 },
                                    { 2, nodeIndex3 },
                                    { 3, nodeIndex4 }
                                };

            // C# modulus operator doesn't return a modulus for a negative number.
            // I could use "3" for up instead of "-1", but that makes the code less readable, so I've created my own method
            var firstKey = tempDictionary.MinBy(k => k.Value).Key;
            var nextKey = GetProperModulus(firstKey + 1, 4);
            var prevKey = GetProperModulus(firstKey - 1, 4);

            var direction = tempDictionary[nextKey] < tempDictionary[prevKey] ? 1 : -1;

            var nodeLoop = new NodeLoop();

            var index = firstKey;
            for (var loopIndex = 0; loopIndex < 4; loopIndex++)
            {
                nodeLoop.Nodes.Add(loopIndex, tempDictionary[GetProperModulus(index, 4)]);
                index = index + direction;
            }

            nodeLoop.Id =
                (nodeLoop.Nodes[0] * nodeCount * nodeCount * nodeCount) +
                (nodeLoop.Nodes[1] * nodeCount * nodeCount) +
                (nodeLoop.Nodes[2] * nodeCount) +
                nodeLoop.Nodes[3];

            return nodeLoop;
        }
    }
}
