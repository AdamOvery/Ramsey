namespace Ramsey.Adam.RamseyLibrary
{
    public static class CliqueFinderAll
    {
        // Pass onOffCheck as true to find "On" cliques
        public static List<int[]> IdentifyCliques(bool[,] edges, int nodeCount, int cliqueSize, bool onOffCheck)
        {
            var cliques = new List<int[]>();
            var clique = new int[cliqueSize];
            FindCliques(edges, clique, 0, 0, cliqueSize, onOffCheck, nodeCount, cliques);

            return cliques;
        }

        // Function to check if the given set of vertices
        // in store array is a clique or not
        private static bool IsClique(bool[,] edges, int[] clique, int b, bool onOffCheck)
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
        private static void FindCliques(bool[,] edges, int[] clique, int i, int l, int s, bool onOffCheck, int nodeCount, List<int[]> cliques)
        {
            // Check if any vertices from i+1 can be inserted
            //            for (int j = i; j < Config.NodeCount - (s - l); j++)

            for (int j = i; j < nodeCount; j++)
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
                        FindCliques(edges, clique, j + 1, l + 1, s, onOffCheck, nodeCount, cliques);
                    }
                    // Size is met
                    else
                    {
                        int[] newClique = new int[s];
                        clique.CopyTo(newClique, 0);
                        cliques.Add(newClique);
                    }
                }
            }
        }
    }
}
