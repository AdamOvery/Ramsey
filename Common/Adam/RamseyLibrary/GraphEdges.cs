namespace Ramsey.Adam.RamseyLibrary
{
    public class GraphEdges
    {
        public bool[,] Edges { get; }
        public List<int>[] NodeLists { get; }

        public GraphEdges(bool[,] edges, List<int>[] nodeLists)
        {
            Edges = edges;
            NodeLists = nodeLists;
        }

        public GraphEdges Clone()
        {
            // Since NodeLists is a reference type, we can't simply use clone
            var newNodeLists = new List<int>[NodeLists.Length];
            for (var index = 0; index < NodeLists.Length; index++)
            {
                newNodeLists[index] = new List<int>(NodeLists[index]);
            }

            return new GraphEdges(Edges.Clone() as bool[,] ?? new bool[0, 0], newNodeLists);
        }
    }
}
