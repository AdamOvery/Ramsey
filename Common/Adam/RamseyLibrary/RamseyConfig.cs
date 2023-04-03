namespace Ramsey.Adam.RamseyLibrary
{
    public class RamseyConfig
    {
        public int NodeCount { get; set; }
        public int MaxCliqueOn { get; set; }
        public int MaxCliqueOff { get; set; }
        public bool FindAllSolutions { get; set; }

        public RamseyConfig(int nodeCount, int maxCliqueOn, int maxCliqueOff, bool findAllSolutions)
        {
            NodeCount = nodeCount;
            MaxCliqueOn = maxCliqueOn;
            MaxCliqueOff = maxCliqueOff;
            FindAllSolutions = findAllSolutions;
        }
    }
}
