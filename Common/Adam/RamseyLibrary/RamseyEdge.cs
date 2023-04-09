namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyEdge
    {
        public int Id { get; }
        public int Node1Index { get; }
        public int Node2Index { get; }
        public int Count { get; set; }

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

            Id = (Node1Index * nodeCount) + Node2Index;
            Count = 1;
        }
    }
}
