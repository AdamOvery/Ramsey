using Pascal;
using static Pascal.TestEngine;

static class SortedGraph
{
    public static IGraph Sorted(this IGraph graph)
    {
        var subGraph = graph.AsSubGraph();
        var sortedNodes = subGraph.nodes.OrderByDescending(n => n, NodeComparer.instance).ToList();
        while (!sortedNodes.IsOrdered())
        {
            // resequence the node Ids
            sortedNodes.ForEachIndexed((n, i) => { n.id = i; });
            // reorder nodes and adjacent nodes
            subGraph.nodes = sortedNodes;
            subGraph.nodes.ForEach(n => n.adjacentNodes = n.adjacentNodes.OrderBy(n => n.id).ToList());
            // then sort again to see if it has changed again
            sortedNodes = subGraph.nodes.OrderByDescending(n => n, NodeComparer.instance).ToList();
        }
        return subGraph.ToGraph();
    }



    internal static void Tests()
    {
        Test("Clique of 5 and 3", () =>
        {
            TestSorted("ex1", "K~{???A????S", "K~{?GK??????");
            TestSorted("ex2", "KG?O?gT??jK?", "K~{?GK??????");
            TestSorted("ex3", "K_o?_BGCeP??", "K~{?GK??????");
        });
        Test("Larger graph of 15", () =>
        {
            TestSorted("ex4", "NG?O?gT??jK???~~_@G", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex5", "NTBwGGC@?gU_l@@G?cG", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex6", "N??NxCMbhE?OBk?_?O?", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex7", "NQVw\\GC@Aw?_@?@?J`?", "N~~{CEBoC?O?_?_?O??");

        });
        TestAdamCrazyGraph();
    }

    public static void TestAdamCrazyGraph()
    {
        var sig41 = @"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw";
        TestSorted("ex8", @"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw", sig41);
        var sig41b = @"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_";
        TestSorted("ex9", @"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41b);
    }

    internal static void TestSorted(string message, string originalG6, string expectedG6)
    {
        IGraph graph = G6.parse(originalG6);
        var sortedGraph = graph.Sorted();
        AssertEquals("[Sorted] " + message, () => G6.fromGraph(sortedGraph), expectedG6);
    }
}
