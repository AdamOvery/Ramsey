namespace Pascal;

public class SubGraph : ISubGraph
{
    private readonly IGraph _graph;

    // Node[] _graphNodes ;
    public int order { get; private set; }

    public List<INode> nodes { get; set; }

    public IGraph graph { get { return _graph; } }

    public class Node : INode
    {
        private readonly SubGraph subGraph;

        public int id { get; set; }

        public List<INode> adjacentNodes { get; set; }

        public Node(SubGraph subGraph, int id)
        {
            this.subGraph = subGraph;
            this.id = id;
            this.adjacentNodes = new List<INode>();
        }

        override public string ToString() => id.ToString();
    }

    // private List<INode> getAdjacentNodes(Node n1)
    // {
    //     return _nodes.Where((n2) => n2 != n1 && graph.GetEdgeValue(n1.id, n2.id)).ToList<INode>();
    // }

    protected SubGraph(IGraph graph, IEnumerable<INode>? nodes = null)
    {
        this._graph = graph;
        if (nodes == null)
        {
            nodes = Enumerable.Range(0, graph.order).Select(id => this.CreateNode(id));
        }
        this.nodes = nodes.ToList();

        var subGraphOrder = this.order = this.nodes.Count;

        for (int b = 0; b < subGraphOrder; b++)
        {
            var nb = this.nodes[b];
            for (int a = 0; a < b; a++)
            {
                var na = this.nodes[a];
                if (graph.GetEdgeValue(na.id, nb.id))
                {
                    na.adjacentNodes.Add(nb);
                    nb.adjacentNodes.Add(na);
                }
            }
        }
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

