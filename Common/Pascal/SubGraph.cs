namespace Pascal;

public class SubGraph : ISubGraph
{
    private readonly IGraph _graph;

    ISet<INode> _nodes;
    // Node[] _graphNodes ;
    public int order { get; private set; }

    public ISet<INode> nodes
    {
        get
        {
            return _nodes;
        }
    }

    public IGraph graph { get { return _graph; } }

    public string Label { get; set; }

    public class Node : INode
    {
        private readonly SubGraph subGraph;

        public int id { get; private set; }

        internal ISet<INode>? _adjacentNodes = null;


        public ISet<INode> adjacentNodes
        {
            get
            {
                return this._adjacentNodes ??= subGraph.getAdjacentNodes(this);
            }
        }

        public string Label { get; set; }

        public Node(SubGraph subGraph, int id)
        {
            this.subGraph = subGraph;
            this.id = id;
            this.Label = "";
        }

        override public string ToString() => id.ToString();
    }

    private ISet<INode> getAdjacentNodes(Node n1)
    {
        return _nodes.Where((n2) => n2 != n1 && graph.GetEdgeValue(n1.id, n2.id)).ToHashSet<INode>();
    }

    protected SubGraph(IGraph graph, IEnumerable<INode>? nodes = null)
    {
        this._graph = graph;
        if (nodes == null) nodes = Enumerable.Range(0, graph.order).Select(id => this.CreateNode(id));
        this._nodes = nodes.ToHashSet();
        this.order = _nodes.Count;
        this.Label = "";
    }

    virtual protected Node CreateNode(int id)
    {
        return new Node(this, id);
    }


    public override string ToString()
    {
        var sortedNodeIds = nodes.Select(n => n.id).OrderBy(i => i);
        return string.Join("-", sortedNodeIds);
    }

    public ISubGraph CreateSubGraph(IEnumerable<INode> nodes)
    {
        return new SubGraph(this.graph, nodes);
    }

    public static readonly ISubGraphFactory factory = new SubGraphFactory();

    class SubGraphFactory : ISubGraphFactory
    {

        public ISubGraph CreateSubGraph(IGraph graph)
        {
            return new SubGraph(graph);
        }
    }

}

