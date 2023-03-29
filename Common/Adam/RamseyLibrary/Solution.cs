namespace RamseyLibrary
{
    public class Solution
    {
        public int[] EdgeLinks { get; }

        public bool[,] Edges { get; }

        public Solution(int[] edgeLinks, bool[,] edges)
        {
            EdgeLinks = edgeLinks.Clone() as int[] ?? Array.Empty<int>();
            Edges = edges.Clone() as bool[,] ?? new bool[0, 0];
        }

        public string EdgeLinkDescription
        {
            get
            {
                return string.Join(",", EdgeLinks);
            }
        }

    }
}
