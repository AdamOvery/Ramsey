using Pascal;
using static Pascal.TestEngine;

static class SortedGraph
{
    internal static void Tests()
    {
        TriangleWithLeg();
        CliqueOf5and3();
        LargerGraphOf15();
        TestTweenGraphOfThree();
        TestAdamCrazyGraph();
    }

    public static IGraph Sorted(this IGraph graph)
    {
        var subGraph = graph.AsSubGraph();
        var sortedNodes = subGraph.nodes.OrderBy(n => n, NodeComparer.instance).ToList();
        int pass = 0;
        while (!sortedNodes.IsOrdered() && pass++ < 100)
        {
            Console.WriteLine("Not sorted:" + String.Join(", ", sortedNodes.Select(n => n.id)));
            // resequence the node Ids
            sortedNodes.ForEachIndexed((n, i) => { n.id = i; });
            // reorder nodes and adjacent nodes
            subGraph.nodes = sortedNodes;
            subGraph.nodes.ForEach(n => n.adjacentNodes = n.adjacentNodes.OrderBy(n => n.id).ToList());

            DisplayNodes("Sorting...", subGraph.ToGraph());
            // then sort again to see if it has changed again
            sortedNodes = subGraph.nodes.OrderBy(n => n, NodeComparer.instance).ToList();
        }
        return subGraph.ToGraph();
    }

    private static void LargerGraphOf15()
    {
        Test("Larger graph of 15", () =>
        {
            TestSorted("ex4", "NG?O?gT??jK???~~_@G", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex5", "NTBwGGC@?gU_l@@G?cG", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex6", "N??NxCMbhE?OBk?_?O?", "N~~{CEBoC?O?_?_?O??");
            TestSorted("ex7", "NQVw\\GC@Aw?_@?@?J`?", "N~~{CEBoC?O?_?_?O??");

        });
    }

    private static void TriangleWithLeg()
    {
        Test("Triangle with leg", () =>
        {
            var sig = GetSorted(@"ECe?");
            TestSorted("v2", "Ea@O", sig);
            TestSorted("v3", "E@K_", sig);
            TestSorted("v4", "E?RG", sig);
        });
    }

    private static void CliqueOf5and3()
    {
        Test("Clique of 5 and 3", () =>
        {
            TestSorted("ex1", "K~{???A????S", "K~{?GK??????");
            TestSorted("ex2", "KG?O?gT??jK?", "K~{?GK??????");
            TestSorted("ex3", "K_o?_BGCeP??", "K~{?GK??????");
        });
    }

    public static void TestTweenGraphOfThree()
    {
        Test("TestTweenGraphOfThree", () =>
        {
            var sig = GetSorted(@"Cl");
            TestSorted("ex9", @"Cr", sig);
            TestSorted("ex8", @"C]", sig);
            TestSorted("ex9", @"Cl", sig);
        });
    }

    public static void TestAdamCrazyGraph()
    {
        Test("TestAdamCrazyGraph", () =>
        {
            var sig41 = @"h~zeubOr]QWpownevRtHLevePzc`asItECjJ^{PbfC}MS_sZ}?BY~EIKCyYiH]lcOof\[}FfN`x_o|`wQLLOtOzoxIRGyYpHhRImbOuhX]CrElmfB^XZr@@mKpqnOU\z_fP@bsTg~S";
            TestSorted("ex8", @"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw", sig41);
            var sig41b = @"h~zeubOrURWpoLmwTmtL?gOmZu[tXcZ}qHlc]UQzmw?M\FOZfghYe?zKvxIJBEosPg]ak}DTnXwA]Z`YdDz_kSUEZFSnkI_maRQtaptRN[CpfHqzC]vZtgAD]fpboL^lbBo?NuUD]k";
            TestSorted("ex9", @"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41b);
        });
    }

    internal static void TestSorted(string message, string originalG6, string expectedG6, bool displayNodes = true)
    {
        IGraph graph = G6.parse(originalG6);
        var sortedGraph = graph.Sorted();
        AssertEquals("[Sorted] " + message, () => G6.fromGraph(sortedGraph), expectedG6);
        if (displayNodes)
        {
            DisplayNodes("Sorted:", sortedGraph);
        }
    }

    internal static string GetSorted(string originalG6, bool displayNodes = true)
    {
        IGraph graph = G6.parse(originalG6);
        if (displayNodes)
        {
            DisplayNodes("Original:", graph);
        }
        var sortedGraph = graph.Sorted();
        if (displayNodes)
        {
            DisplayNodes("Sorted:", sortedGraph);
        }
        return G6.fromGraph(sortedGraph);
    }




    private static void DisplayNodes(string title, IGraph graph)
    {
        Console.WriteLine(title + " " + G6.fromGraph(graph));
        for (int i = 0; i < graph.order; i++)
        {
            var adj = new List<int>();
            for (int j = 0; j < graph.order; j++)
            {
                if (i != j && graph.GetEdgeValue(i, j)) adj.Add(j);
            }
            var s = "[" + String.Join(",", adj.Select(n => "N" + n)) + "]";
            Console.WriteLine($"N{i}: {adj.Count} {s}");
        }
    }
}
