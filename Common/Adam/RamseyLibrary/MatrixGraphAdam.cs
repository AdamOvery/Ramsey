namespace Ramsey.Adam.RamseyLibrary
{
    public class MatrixGraphAdam : IGraph
    {
        public bool[,] edges { get; }
        public int[] nodeCounts { get; private set; }

        public event EdgeChangedHandler EdgeChanged = delegate { };
        public event GraphChangedHandler GraphChanged = delegate { };

        public MatrixGraphAdam(int order)
        {
            this.order = order;
            edges = new bool[order, order];
            nodeCounts = new int[order];
        }

        public int order { get; private set; }

        public bool GetEdgeValue(int n1, int n2)
        {
            return edges[n1, n2];
        }

        public void SetEdgeValue(int n1, int n2, bool value)
        {
            var current = edges[n1, n2];
            if (value != current)
            {
                edges[n1, n2] = value;
                edges[n2, n1] = value;
                if (value)
                {
                    nodeCounts[n1]++;
                    nodeCounts[n2]++;
                }
                else
                {
                    nodeCounts[n1]--;
                    nodeCounts[n2]--;
                }
                EdgeChanged.Invoke(this, n1, n2, value);
            }
        }

        public void Clear()
        {
            Array.Clear(edges);
            Array.Clear(nodeCounts);
            GraphChanged.Invoke(this);
        }

        public static IGraphFactory factory = new MatrixGraphFactory();

        private class MatrixGraphFactory : IGraphFactory
        {
            public IGraph newGraph(int order)
            {
                return new MatrixGraphAdam(order);
            }
        }
    }
}
