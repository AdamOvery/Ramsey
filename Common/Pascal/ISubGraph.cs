namespace Pascal;

public interface ISubGraph
{
    IGraph graph { get; }

    ISet<INode> nodes { get; }

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
}

public delegate void OnNodeVisited(INode visitedNode, INode? parentNode);
