using System.Net;

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
        public IList<Solution> Solutions { get; private set; }

        // Ramsey Graph B specifics
        public int MinEdgeCount { get; set; }
        public int MaxEdgeCount { get; private set; }

        public void InitializeGraph()
        {
            Iterations = 0;
            IsSuccess = false;
            Solutions = new List<Solution>();
            var startTime = DateTime.UtcNow;

            MaxEdgeCount = MinEdgeCount + 1;

            var edgeLinkCount = (MinEdgeCount % 2 == 0) ? MinEdgeCount / 2 : MaxEdgeCount / 2;
            var halfNodeCount = (Config.NodeCount % 2 == 0) ? (Config.NodeCount - 2) / 2 : (Config.NodeCount - 1) / 2;

            // The edgeLinks are the links to "other" nodes that every node will have.
            // e.g. If this has 3 entries of 1, 2 & 4, It would mean that node number x needs to be linked to nodes x+1, x+2 & x+4
            var edgeLinks = new int[edgeLinkCount];

            // Initialize Edge Links
            for (var loop = 0; loop < edgeLinkCount; loop++)
            {
                edgeLinks[loop] = loop + 1;
            }

            var edges = new bool[Config.NodeCount, Config.NodeCount];
            var nodeLists = new List<int>[Config.NodeCount];
            for (var nodeIndex = 0; nodeIndex < Config.NodeCount; nodeIndex++)
            {
                nodeLists[nodeIndex] = new List<int>();
            }

            if (!GetNextValidEdgeLinkArray(edgeLinks, halfNodeCount))
            {
                TimeTaken = DateTime.UtcNow - startTime;
                return;
            }

            Iterations++;
            // Clear down graph
            Array.Clear(edges);

            for (var node1Index = 0; node1Index < Config.NodeCount; node1Index++)
            {
                for (var edgeIndex = 0; edgeIndex < edgeLinkCount; edgeIndex++)
                {
                    var node2Index = (node1Index + edgeLinks[edgeIndex]) % Config.NodeCount;

                    if (!edges[node1Index, node2Index])
                    {
                        ToggleEdge(edges, nodeLists, node1Index, node2Index);
                    }
                }
            }

            if (!IdentifyInvalidCliques(edges))
            {
                IsSuccess = true;
                TimeTaken = DateTime.UtcNow - startTime;
                Solutions.Add(new Solution(edgeLinks, edges));

                if (!Config.FindAllSolutions)
                {
                    return;
                }
            }


            // Test Code
            //ToggleEdge(edges, nodeLists, 0, 1);
            //ToggleEdge(edges, nodeLists, 1, 2);
            //ToggleEdge(edges, nodeLists, 2, 3);
            //ToggleEdge(edges, nodeLists, 3, 4);
            //ToggleEdge(edges, nodeLists, 4, 0);

            var nodeClassification = new NodeClassification(Config, nodeLists);

            var nodeIds = new List<string>();

            for(var nodeIndex = 0; nodeIndex < Config.NodeCount; nodeIndex++)
            {
                nodeIds.Add(nodeClassification.ClassifyNode(nodeIndex));
            }

            var fullGraphId = string.Join(",", nodeIds.OrderBy(i => i).ToList());

            //return fullGraphId;

            // TODO:

            // Create dictionary of node Ids to classify how many "types" of node there are. 

            // Categorize all nodes into ones with the "Min Edge Count" and those with "Max Edge Count"
            // Go through all with "Max Edge Count" and look for edges we can remove
            // Go through all with "Min Edge Count" and look for edges we can add.
            // NB: For the latter, we will need to nodeClassificiation Node Distances too because the distance matters as well as the node "type"
            // e.g. If you have 6 identical nodes, it could well be that 2 are at distance 1, 2 are at distance 2 and 1 is at distance 3. 
            // Each of the 3 options result in a different graph. 
            // NB: Obviously this doesn't apply to removing edges, because they are always as distance 1

            // I think we should create a hash of the fullGraphId above
            // Add to a dictionary of these, so we can ignore those graphs if we find them again
            // Loop through each entry and do the above for each one.


            TimeTaken = DateTime.UtcNow - startTime;
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
