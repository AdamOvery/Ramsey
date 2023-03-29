namespace Pascal;
class MatrixGraph : IGraph
{
    // I am not clear why or whether I need this delegate {} but the compiler wants it.
    private bool[,] edges;

    public event EdgeChangedHandler EdgeChanged = delegate { };
    public event GraphChangedHandler GraphChanged = delegate { };

    public MatrixGraph(int order)
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
        edges = new bool[order, order];
        GraphChanged.Invoke(this);
    }

    public static IGraphFactory factory = new MatrixGraphFactory();

    private class MatrixGraphFactory : IGraphFactory
    {
        public IGraph newGraph(int order)
        {
            return new MatrixGraph(order);
        }
    }

}

