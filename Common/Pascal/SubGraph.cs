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

        public Node(SubGraph nodes, int id)
        {
            this.nodes = nodes;
            this.id = id;
        }
    }

    private ISet<INode> getAdjacentNodes(Node n1)
    {
        return _nodes.Where((n2) => n2 != n1 && graph.GetEdgeValue(n1.id, n2.id)).ToHashSet<INode>();
    }

    public SubGraph(ISubGraph subGraph, IEnumerable<INode> nodes) : this(subGraph.graph, nodes.Select(n => n.id))
    {
    }

    public SubGraph(IGraph graph, IEnumerable<int>? nodeIds = null)
    {
        this._graph = graph;
        if (nodeIds == null) nodeIds = Enumerable.Range(0, graph.order);
        this._nodes = nodeIds.Select(i => new Node(this, i) as INode).ToHashSet();

        // graph.GraphChanged += (a) => Array.ForEach(_nodes, (n) => n._adjacentNodes = null);
        // graph.EdgeChanged += (sender, n1, n2, value) =>
        // {
        //     if (value)
        //     {
        //         (_nodes[n1]._adjacentNodes ??= new HashSet<INode>()).Add(_nodes[n2]);
        //         (_nodes[n2]._adjacentNodes ??= new HashSet<INode>()).Add(_nodes[n1]);
        //     }
        //     else
        //     {
        //         _nodes[n1]._adjacentNodes!.Remove(_nodes[n2]);
        //         _nodes[n2]._adjacentNodes!.Remove(_nodes[n1]);
        //     }
        // };
    }

    public override string ToString()
    {
        var sortedNodeIds = nodes.Select(n => n.id).OrderBy(i => i);
        return string.Join("-", sortedNodeIds);
    }

}

