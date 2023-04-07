namespace Ramsey.Adam.RamseyLibrary
{
    public class MatrixGraphAdam : IGraph
    {
        public bool[,] edges { get; }

        public event EdgeChangedHandler EdgeChanged = delegate { };
        public event GraphChangedHandler GraphChanged = delegate { };

        public MatrixGraphAdam(int order)
        {
            this.order = order;
            edges = new bool[order, order];
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
                EdgeChanged.Invoke(this, n1, n2, value);
            }
        }

        public void Clear()
        {
            Array.Clear(edges);
            GraphChanged.Invoke(this);
        }
    }
}
