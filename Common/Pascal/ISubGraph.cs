namespace Pascal;

public interface ISubGraph
{
    IGraph graph { get; }

    IList<INode> nodes { get; }

    String Label { get; set; }

    //    INode CreateNode(int id);
    ISubGraph CreateSubGraph(IEnumerable<INode> nodes);
}

public interface ISubGraphFactory
{
    ISubGraph CreateSubGraph(IGraph graph);
}


public static class ISubGraphExtension
{
    public static ISubGraph AsSubGraph(this IGraph g, ISubGraphFactory? factory = null)
    {
        return (factory ?? SubGraph.factory).CreateSubGraph(g);
    }

    public static IGraph ToGraph(this ISubGraph g, IGraphFactory? factory = null)
    {
        var graph = (factory ?? MatrixGraph.factory).newGraph(g.graph.order);
        foreach (var n1 in g.nodes)
        {
            foreach (var n2 in n1.adjacentNodes)
            {
                if (n1.id < n2.id) graph.SetEdgeValue(n1.id, n2.id, true);
            }
        }
        return graph;
    }
}

public delegate void OnNodeVisited(INode visitedNode, INode? parentNode);
