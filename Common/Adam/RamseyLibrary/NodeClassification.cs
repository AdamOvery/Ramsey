using System.Collections.Generic;

namespace Ramsey.Adam.RamseyLibrary
{
    public class NodeClassification
    {
        private List<int>[] NodeLists { get; }
        public Dictionary<int, List<int>> DistancesDictionary { get; }
        public int?[] NodeDistances { get; }
        public int NodeCount { get; private set; }
        public int GraphNodeCount { get; }

        public NodeClassification(int nodeCount, bool[,] edges, List<int>[]? nodeLists)
        {
            GraphNodeCount = nodeCount;

            if (nodeLists is not null)
            {
                NodeLists = nodeLists;
            }
            else
            {
                // Build up node lists (These are the same as edges but each node has an list of "on" edges rather than a straight array)
                NodeLists = new List<int>[edges.Length];
                for (var node1Index = 0; node1Index < nodeCount; node1Index++)
                {
                    NodeLists[node1Index] = new List<int>();
                    for (var node2Index = 0; node2Index < nodeCount; node2Index++)
                    {
                        if (edges[node1Index, node2Index])
                        {
                            NodeLists[node1Index].Add(node2Index);
                        }
                    }
                }
            }

            NodeDistances = new int?[nodeCount];
            DistancesDictionary = new Dictionary<int, List<int>>();
        }

        public string ClassifyNode(int nodeStartIndex)
        {
            Array.Clear(NodeDistances);
            DistancesDictionary.Clear();
            NodeCount = 1; // Include self

            NodeDistances[nodeStartIndex] = 0;
            DistancesDictionary.Add(0, new List<int>() { nodeStartIndex });
            var distance = 0;
            while (SetDistances(distance++));

            return GetId(nodeStartIndex, true);
        }

        public string ClassifyGraph()
        {
            var nodeIds = new Dictionary<string, List<int>>();

            for (var nodeIndex = 0; nodeIndex < GraphNodeCount; nodeIndex++)
            {
                var nodeId = ClassifyNode(nodeIndex);

                // Add to generic dictionary
                if (!nodeIds.ContainsKey(nodeId))
                {
                    nodeIds.Add(nodeId, new List<int>());
                }
                nodeIds[nodeId].Add(nodeIndex);
            }

            var fullGraphId = string.Join(",", nodeIds.Values.Select(v => v.Count).OrderBy(v => v).Select(v => v.ToString()).ToList()) +
                "x" + string.Join(",", nodeIds.Keys.OrderBy(k => k).ToList());

            return fullGraphId;
        }

        private bool SetDistances(int distance)
        {
            DistancesDictionary[distance + 1] = new List<int>();

            var isFound = false;

            foreach (var nodeIndex in DistancesDictionary[distance])
            {
                foreach(var nodeToIndex in NodeLists[nodeIndex])
                {
                    if (NodeDistances[nodeToIndex] is null)
                    {
                        NodeDistances[nodeToIndex] = distance + 1;
                        DistancesDictionary[distance + 1].Add(nodeToIndex);
                        isFound = true;
                        NodeCount++;
                    }
                }
            }

            return isFound;
        }

        private string GetId(int nodeIndex, bool childrenOnly)
        {
            var childIds = new List<string>();
            foreach (var nodeToIndex in NodeLists[nodeIndex])
            {
                var nextDistanceBase = NodeDistances[nodeToIndex];
                if (nextDistanceBase is not null)
                {
                    var nextDistance = (int)nextDistanceBase;

                    if (nextDistance == NodeDistances[nodeIndex] + 1)
                    {
                        childIds.Add(GetId(nodeToIndex, false));
                    }
                    else if ((nextDistance == NodeDistances[nodeIndex]) && !childrenOnly)
                    {
                        childIds.Add(GetId(nodeToIndex, true));
                    }
                    else
                    {
                        childIds.Add(nextDistance.ToString());
                    }
                }
            }
            var id = NodeDistances[nodeIndex].ToString() +
                "." + string.Join(":", childIds.OrderBy(id => id).Select(id => id));         

            return id;
        }
    }
}
