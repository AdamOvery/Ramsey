namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyConfig
    {
        public int NodeCount { get; set; }
        public int MaxCliqueOn { get; set; }
        public int MaxCliqueOff { get; set; }
        public bool FindAllSolutions { get; set; }

        // This is the number of edges that each node has.
        public int? NodeEdgeCount { get; set; }

        // Don't even check if any node has less then these edges.
        public int? MinNodeEdgeCount { get; set; }

        public RamseyConfig(int nodeCount, int maxCliqueOn, int maxCliqueOff, bool findAllSolutions, int? nodeEdgeCount = null, int? minNodeEdgeCount = null)
        {
            NodeCount = nodeCount;
            MaxCliqueOn = maxCliqueOn;
            MaxCliqueOff = maxCliqueOff;
            FindAllSolutions = findAllSolutions;
            NodeEdgeCount = nodeEdgeCount;
            MinNodeEdgeCount = minNodeEdgeCount;
            if (MinNodeEdgeCount is null)
            {
                MinNodeEdgeCount = nodeEdgeCount;
            }
        }
    }
}
