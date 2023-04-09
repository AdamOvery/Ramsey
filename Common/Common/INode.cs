

public interface INode
{
    int id { get; }
    IList<INode> adjacentNodes { get; }

    String Label { get; set; }
}
