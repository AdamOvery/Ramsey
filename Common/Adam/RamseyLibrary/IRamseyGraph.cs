namespace Ramsey.Adam.RamseyLibrary
{
    public interface IRamseyGraph
    {
        public void InitializeGraph();

        public RamseyConfig Config { get; }
        // Results
        public bool IsSuccess { get; }
        public TimeSpan TimeTaken { get; }
        public int Iterations { get; }
        public IList<Solution> Solutions { get; }
    }
}
