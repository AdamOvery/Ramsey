

public interface INode
{
    int id { get; }
    ISet<INode> adjacentNodes { get; }
}
