using System.Net;

namespace RamseyLibrary
{
    public class Ramsey
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Ramsey(RamseyConfig ramseyConfig)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Config = ramseyConfig;

            InitializeGraph();
        }

        public RamseyConfig Config { get; }

        // Results
        public bool IsSuccess { get; private set; }
        public TimeSpan TimeTaken { get; private set; }
        public int Iterations { get; private set; }
        public IList<Solution> Solutions { get; } = new List<Solution>();

        public void InitializeGraph()
        {
            Iterations = 0;
            IsSuccess = false;
            var startTime = DateTime.UtcNow;

            // Currently this will only work for node counts of 5, 9, 13, 17, etc.
            var otherNodeCount = Config.VertexCount - 1;

            if (otherNodeCount % 4 != 0)
            {
                TimeTaken = DateTime.UtcNow - startTime;
                Solutions.Clear();
                return;
            }

            var halfVertexCount = otherNodeCount / 2;
            var edgeLinkCount = otherNodeCount / 4;

            // The edgeLinks are the links to "other" nodes that every node will have.
            // e.g. If this has 3 entries of 1, 2 & 4, It would mean that node number x needs to be linked to nodes x+1, x+2 & x+4
            var edgeLinks = new int[edgeLinkCount];

            // Initialize Edge Links
            for (var loop = 0; loop < edgeLinkCount; loop++)
            {
                edgeLinks[loop] = loop + 1;
            }

            var edges = new bool[Config.VertexCount, Config.VertexCount];

            while (true)
            {
                if (GetNextValidEdgeLinkArray(edgeLinks, halfVertexCount))
                {
                    Iterations++;
                    // Clear down graph
                    Array.Clear(edges);

                    for (var vertex1Index = 0; vertex1Index < Config.VertexCount; vertex1Index++)
                    {
                        for (var edgeIndex = 0; edgeIndex < edgeLinkCount; edgeIndex++)
                        {
                            var vertex2Index = (vertex1Index + edgeLinks[edgeIndex]) % Config.VertexCount;

                            if (!edges[vertex1Index, vertex2Index])
                            {
                                ToggleEdge(edges, vertex1Index, vertex2Index);
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
                }

                // Try the next edgeLink setting
                var maxGoodEdgeIndex = edgeLinkCount - 1;
                var maxAllowedEdgeLinkValue = halfVertexCount;

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

        // This is the older version of GetNextValidEdgeLinkArray. There's very little difference in speed
        // Let's keep this code for reference        
        private bool AreConsecutiveEdgeLinkCountsValid(int[] edgeLinks, int halfVertexCount)
        {
            for (var index = 0; index < edgeLinks.Length - 1; index++)
            {
                // We can't have a gap of more than Config.MaxCliqueOff, this will result in an invalid "Off" clique
                if (edgeLinks[index + 1] - edgeLinks[index] > Config.MaxCliqueOff)
                {
                    return false;
                }

                // Look "ahead" by Config.MaxCliqueOn - 1. If that edgelink value is the original value + Config.MaxCliqueOn
                // Then we will result in an invalid "On" clique
                if (index + Config.MaxCliqueOn - 1 < edgeLinks.Length)
                {
                    if (edgeLinks[index + Config.MaxCliqueOn - 1] - edgeLinks[index] < Config.MaxCliqueOn)
                    {
                        return false;
                    }
                }
            }

            // If the highest edge link is less than or equal to "Half Vertex Count" - MaxCliqueOff, then we have an "end-gap" too big
            // Which will result in an invalid "Off" clique
            if (edgeLinks[edgeLinks.Length - 1] <= halfVertexCount - Config.MaxCliqueOff)
            {
                return false;
            }

            return true;
        }

        private bool GetNextValidEdgeLinkArray(int[] edgeLinks, int halfVertexCount)
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

                // If the highest edge link is less than or equal to "Half Vertex Count" - MaxCliqueOff, then we have an "end-gap" too big
                // Which will result in an invalid "Off" clique
                if (index == edgeLinks.Length - 1)
                {
                    if (edgeLinks[edgeLinks.Length - 1] <= halfVertexCount - Config.MaxCliqueOff)
                    {
                        edgeLinks[edgeLinks.Length - 1] = halfVertexCount - Config.MaxCliqueOff + 1;
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
            var clique = new int[Config.VertexCount];
            if (FindCliques(edges, clique, 0, 0, Config.MaxCliqueOn, true))
            {
                return true;
            }
            clique = new int[Config.VertexCount];
            return FindCliques(edges, clique, 0, 0, Config.MaxCliqueOff, false);
        }

        private void ToggleEdge(bool[,] edges, int vertex1, int vertex2)
        {
            if (edges[vertex1, vertex2])
            {
                edges[vertex1, vertex2] = false;
                edges[vertex2, vertex1] = false;
            }
            else
            {
                edges[vertex1, vertex2] = true;
                edges[vertex2, vertex1] = true;
            }
        }

        // Function to check if the given set of vertices
        // in store array is a clique or not
        private bool IsClique(bool[,] edges, int[] clique, int b, bool onOffCheck)
        {
            // Run a loop for all the set of edges
            // for the select vertex
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
            //            for (int j = i; j < Config.VertexCount - (s - l); j++)

            for (int j = i; j < Config.VertexCount; j++)
            {
                // Add the vertex to clique
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
