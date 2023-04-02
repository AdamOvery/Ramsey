namespace Pascal;

public interface ISubGraph
{
    IGraph graph { get; }

    ISet<INode> nodes { get; }

}


public static class ISubGraphExtension
{
    public static SubGraph AsSubGraph(this IGraph g)
    {
        return new SubGraph(g);
    }

}

public delegate void OnNodeVisited(INode visitedNode, INode? parentNode);
