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
        private readonly SubGraph nodes;

        public int id { get; private set; }

        internal ISet<INode>? _adjacentNodes = null;


        public ISet<INode> adjacentNodes
        {
            get
            {
                return this._adjacentNodes ??= nodes.getAdjacentNodes(this);
            }
        }

        public string Label { get; set; }

        public Node(SubGraph nodes, int id)
        {
            this.nodes = nodes;
            this.id = id;
            this.Label = "";
        }

        override public string ToString() => id.ToString();
    }

    private ISet<INode> getAdjacentNodes(Node n1)
    {
        return _nodes.Where((n2) => n2 != n1 && graph.GetEdgeValue(n1.id, n2.id)).ToHashSet<INode>();
    }

    internal SubGraph(IGraph graph, IEnumerable<int>? nodeIds = null)
    {
        this._graph = graph;
        if (nodeIds == null) nodeIds = Enumerable.Range(0, graph.order);
        this._nodes = nodeIds.Select(i => new Node(this, i) as INode).ToHashSet();

        this.Label = "";

    }

    public override string ToString()
    {
        var sortedNodeIds = nodes.Select(n => n.id).OrderBy(i => i);
        return string.Join("-", sortedNodeIds);
    }

    public ISubGraph CreateSubGraph(IEnumerable<INode> nodes)
    {
        return new SubGraph(this.graph, nodes.Select(n => n.id));
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

