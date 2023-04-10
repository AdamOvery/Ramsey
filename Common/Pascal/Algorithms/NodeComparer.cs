namespace Pascal;


/// <summary>Compares two node in their existing configuration.</summary>
/// <remarks>If the adjacent nodes are not sorted, it will favour the best sorted adjacent node.</remarks>
public class NodeComparer : IComparer<INode>
{
    public int Compare(INode? a, INode? b)
    {

        var visited = new HashSet<Tuple<int, int>>();
        var cmp = (INode a, INode b) => 0;

        cmp = (a, b) =>
        {
            var pair = new Tuple<int, int>(a.id, b.id);
            if (visited.Contains(pair)) return 0;
            visited.Add(pair);
            var adjA = a.adjacentNodes;
            var adjB = b.adjacentNodes;
            int na = adjA.Count;
            int nb = adjB.Count;
            if (na != nb) return na.CompareTo(nb);
            for (int i = 0; i < na; i++)
            {
                var result = cmp(adjA[i], adjB[i]);
                if (result != 0) return result;
            }
            return 0;
        };
        if (a == null) return (b == null ? 0 : -1);
        else if (b == null) return 1;

        int result = cmp(a, b);
        return result;
    }

    public static readonly NodeComparer instance = new NodeComparer();
}
