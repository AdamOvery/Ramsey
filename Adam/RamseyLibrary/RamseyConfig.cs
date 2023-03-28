namespace RamseyLibrary
{
    public class RamseyConfig
    {
        public int VertexCount { get; set; }
        public int MaxCliqueOn { get; set; }
        public int MaxCliqueOff { get; set; }
        public bool FindAllSolutions { get; set; }

        public RamseyConfig(int vertexCount, int maxCliqueOn, int maxCliqueOff, bool findAllSolutions)
        {
            VertexCount = vertexCount;
            MaxCliqueOn = maxCliqueOn;
            MaxCliqueOff = maxCliqueOff;
            FindAllSolutions = findAllSolutions;
        }
    }
}
