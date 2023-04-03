using System.Collections.Generic;

namespace Ramsey.Adam.RamseyLibrary
{
    public class NodeClassification
    {
        private List<int>[] NodeLists { get; }
        public Dictionary<int, List<int>> DistancesDictionary { get; }
        public int?[] NodeDistances { get; }
        public int NodeCount { get; private set; }

        public NodeClassification(RamseyConfig config, List<int>[] nodeLists)
        {
            // Don't need to build up lists - We're going to keep those up-to-date in Ramsey Graph
            //// Build up node lists (These are the same as edges but each node has an list of "on" edges rather than a straight array)
            //NodeLists = new List<int>[edges.Length];
            //for (var node1Index = 0; node1Index < config.NodeCount; node1Index++)
            //{
            //    NodeLists[node1Index] = new List<int>();
            //    for (var node2Index = 0; node2Index < config.NodeCount; node2Index++)
            //    {
            //        if (edges[node1Index, node2Index])
            //        {
            //            NodeLists[node1Index].Add(node2Index);
            //        }
            //    }
            //}

            NodeLists = nodeLists;
            NodeDistances = new int?[config.NodeCount];
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

        public bool SetDistances(int distance)
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

        public string GetId(int nodeIndex, bool childrenOnly)
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
