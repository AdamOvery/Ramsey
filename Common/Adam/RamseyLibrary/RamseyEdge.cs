namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyEdge
    {
        public int Node1Index { get; }
        public int Node2Index { get; }

        public RamseyEdge(int node1Index, int node2Index)
        {
            Node1Index = node1Index;
            Node2Index = node2Index;
        }
    }
}
