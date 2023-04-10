namespace Pascal;


/// <summary>Compares two node in their existing configuration.</summary>
/// <remarks>If the adjacent nodes are not sorted, it will favour the best sorted adjacent node.</remarks>
public class NodeComparer : IComparer<INode>
{
    public int Compare(INode? a, INode? b)
    {

        var visited = new HashSet<Tuple<int, int>>();
        var cmpByAdjCount = (INode a, INode b) => 0;
        var cmpByEdgeNo = (INode a, INode b) => 0;
        var cmpByAdjNodeId = (INode a, INode b) => 0;

        cmpByAdjCount = (a, b) =>
        {
            var pair = new Tuple<int, int>(a.id, b.id);
            if (visited.Contains(pair)) return 0;
            visited.Add(pair);
            var adjA = a.adjacentNodes;
            var adjB = b.adjacentNodes;
            int na = adjA.Count;
            int nb = adjB.Count;
            if (na != nb) return nb.CompareTo(na); // we put the node with the most adjacent nodes first
            for (int i = 0; i < na; i++)
            {
                var result = cmpByAdjCount(adjA[i], adjB[i]);
                if (result != 0) return result;
            }
            return 0;
        };

        cmpByEdgeNo = (a, b) =>
        {
            var pair = new Tuple<int, int>(a.id, b.id);
            if (visited.Contains(pair)) return 0;
            visited.Add(pair);
            var adjA = a.adjacentNodes;
            var adjB = b.adjacentNodes;
            int na = adjA.Count;
            int nb = adjB.Count;
            if (na != nb) throw new Exception("Internal error: the two nodes are not the same");
            for (int i = 0; i < na; i++)
            {
                var edgeA = Math.Min(a.id, adjA[i].id);
                var edgeB = Math.Min(b.id, adjB[i].id);
                var result = edgeA.CompareTo(edgeB);
                if (result != 0) return result;
            }

            for (int i = 0; i < na; i++)
            {
                var result = cmpByEdgeNo(adjA[i], adjB[i]);
                if (result != 0) return result;
            }
            return 0;
        };

        cmpByAdjNodeId = (a, b) =>
        {
            return a.id.CompareTo(b.id);
        };
        if (a == null) return (b == null ? 0 : 1); // null is less than anything else
        else if (b == null) return -1; // anything is greater than null

        int result = cmpByAdjCount(a, b);
        if (result == 0)
        {
            visited.Clear();
            result = cmpByEdgeNo(a, b);
            if (result == 0)
            {
                // the two nodes are the same we forget all our commitment and compare node ids
                visited.Clear();
                result = cmpByAdjNodeId(a, b);
            }
        }
        return result;
    }

    public static readonly NodeComparer instance = new NodeComparer();

    internal static void Tests()
    {
        TestEngine.Test("NodeComparer", () =>
         {
             // NodeWithMoreEdgesArrivesFirst();
             // NodeWithBetterAdjacentsArrivesFirst();
             // // // When_all_is_equal_the_node_with_the_lower_id_wins_which_is_wrong();
             TheBigClVsCr();
         });
    }

    private static void NodeWithMoreEdgesArrivesFirst()
    {
        TestEngine.Test("The node with more edges arrives first.", () =>
        {
            //
            var g = G6.parse("IS`A?????").AsSubGraph();
            TestEngine.Assert("1 arrives before 2", () => (1).CompareTo(2) < 0); // sanity check 1 arrives before 2
            TestEngine.Assert("n0 arrives before n1", () => NodeComparer.instance.Compare(g.nodes[0], g.nodes[1]) < 0);
            TestEngine.Assert("n0 arrives before n9", () => NodeComparer.instance.Compare(g.nodes[0], g.nodes[9]) < 0);
        });
    }

    private static void NodeWithBetterAdjacentsArrivesFirst()
    {
        TestEngine.Test("When two nodes have same edge count, the node with better adjacents arrives first", () =>
        {
            var g = G6.parse("Is??GGC?G").AsSubGraph();
            TestEngine.Assert("n5 arrives before n0", () => NodeComparer.instance.Compare(g.nodes[5], g.nodes[0]) < 0);
        });
    }

    private static void When_all_is_equal_the_node_with_the_lower_id_wins_which_is_wrong()
    {
        TestEngine.Test("When two nodes are identical, the node with the lower edge ID arrives first", () =>
        {
            var g = G6.parse("IA???@O??").AsSubGraph();
            TestEngine.Assert("n1 arrives before n3", () => NodeComparer.instance.Compare(g.nodes[1], g.nodes[3]) < 0);
            TestEngine.Assert("n3 arrives before n8", () => NodeComparer.instance.Compare(g.nodes[3], g.nodes[8]) < 0);
            TestEngine.Assert("n8 arrives before n0", () => NodeComparer.instance.Compare(g.nodes[8], g.nodes[0]) < 0);
        });
    }
    // EAS?

    // Cr
    private static void TheBigClVsCr()
    {
        // C] Cl and Cr are equivalent. It is a square
        //  
        TestEngine.Test("TheBigClVsCr", () =>
        {

            var g = G6.parse("Cl").AsSubGraph();
            TestEngine.Assert("n0 arrives before n1", () => NodeComparer.instance.Compare(g.nodes[0], g.nodes[1]) < 0);
            TestEngine.Assert("n1 arrives before n3", () => NodeComparer.instance.Compare(g.nodes[1], g.nodes[3]) < 0);
            TestEngine.Assert("n3 arrives before n2", () => NodeComparer.instance.Compare(g.nodes[3], g.nodes[2]) < 0);
        });
    }

    static int getEdgeNo(int a, int b)
    {
        return a + b * (b - 1) / 2;
    }
}
