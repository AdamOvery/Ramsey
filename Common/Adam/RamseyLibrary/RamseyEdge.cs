namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyEdge
    {
        public int Id { get; }
        public int Node1Index { get; }
        public int Node2Index { get; }
        public int Count { get; set; }
        public bool IsInvalid { get; set; }

        public RamseyEdge(int node1Index, int node2Index, int nodeCount)
        {
            if (node1Index < node2Index)
            {
                Node1Index = node1Index;
                Node2Index = node2Index;
            }
            else
            {
                Node1Index = node2Index;
                Node2Index = node1Index;
            }

            Id = GetId(Node1Index, Node2Index, nodeCount);
            Count = 1;
        }

        public static int GetId(int node1Index, int node2Index, int nodeCount)
        {
            if (node1Index > node2Index)
            {
                var dummy = node1Index;
                node1Index = node2Index;
                node2Index = dummy;
            }

            return (node1Index * nodeCount) + node2Index;
        }
    }
}
