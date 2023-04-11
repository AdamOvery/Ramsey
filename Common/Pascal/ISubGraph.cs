namespace Pascal;

public interface ISubGraph
{
    IGraph graph { get; }

    List<INode> nodes { get; set; }

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
        var order = g.nodes.Count;
        var graph = (factory ?? MatrixGraph.factory).newGraph(order);

        var indexOfNode = g.nodes.WithIndex<INode>().ToDictionary(x => x.item, x => x.index);
        foreach (var n1 in g.nodes)
        {
            var i1 = indexOfNode[n1];
            foreach (var n2 in n1.adjacentNodes)
            {

                if (n1.id < n2.id)
                {
                    var i2 = indexOfNode[n2];
                    graph.SetEdgeValue(i1, i2, true);
                }
            }
        }
        return graph;
    }
}

public delegate void OnNodeVisited(INode visitedNode, INode? parentNode);
