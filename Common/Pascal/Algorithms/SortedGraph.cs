using Pascal;
using static Pascal.TestEngine;

static class SortedGraph
{
    public static bool Verbose = false;

    internal static void Tests(IComparer<INode> comparer)
    {
        // TriangleWithLeg();
        // CliqueOf5and3();
        // LargerGraphOf15();
        // TestTweenGraphOfThree();
        // not working yet TestAdamCrazyGraphI();
        // TestCaVsCo();
        //TestD_oVsDzc(comparer);
        // TestSorted("DUG should get you DpG and not Dpo", @"DUG", @"Dpg", comparer, displaySort: true);
        TestRandomGraphs(7, nbGraphs: 100_000, nbShuffle: 10_000, comparer: comparer);

    }

    public static IGraph Sorted(this IGraph graph, IComparer<INode> comparer, IGraphFactory? factory = null)
    {
        int pass = 0;
        while (true)
        {
            var subGraph = graph.AsSubGraph();
            if (Verbose)
            {
                Console.WriteLine(graph.ToString($"Pass {pass}..."));
            }
            var unsortedNode = FindFirstUnsortedNode(subGraph, comparer);
            if (unsortedNode < 0) break;
            graph = SwapTwoNodes(subGraph, unsortedNode, unsortedNode + 1, factory);
            pass += 1;
            if (pass == 1000) throw new Exception("Infinite loop here");
        }
        return graph;
    }

    public static int FindFirstUnsortedNode(this ISubGraph subgraph, IComparer<INode> comparer)
    {
        for (int i = 0; i < subgraph.graph.order - 1; i++)
        {
            var na = subgraph.nodes[i];
            var nb = subgraph.nodes[i + 1];
            var cmp = comparer.Compare(na, nb);
            if (cmp > 0) return i;
        }
        return -1;
    }

    public static IGraph SwapTwoNodes(this ISubGraph subgraph, int a, int b, IGraphFactory? factory)
    {
        var na = subgraph.nodes[a];
        var nb = subgraph.nodes[b];

        subgraph.nodes[a] = nb;
        subgraph.nodes[b] = na;

        return subgraph.ToGraph(factory);
    }

    private static void LargerGraphOf15(IComparer<INode> comparer)
    {
        Test("Larger graph of 15", () =>
        {
            TestSorted("ex4", "NG?O?gT??jK???~~_@G", "N~~{CEBoC?O?_?_?O??", comparer);
            TestSorted("ex5", "NTBwGGC@?gU_l@@G?cG", "N~~{CEBoC?O?_?_?O??", comparer);
            TestSorted("ex6", "N??NxCMbhE?OBk?_?O?", "N~~{CEBoC?O?_?_?O??", comparer);
            TestSorted("ex7", "NQVw\\GC@Aw?_@?@?J`?", "N~~{CEBoC?O?_?_?O??", comparer);

        });
    }

    private static void TriangleWithLeg(IComparer<INode> comparer)
    {
        Test("Triangle with leg", () =>
        {
            var sig = GetSorted(@"ECe?", comparer);
            TestSorted("v2", "Ea@O", sig, comparer);
            TestSorted("v3", "E@K_", sig, comparer);
            TestSorted("v4", "E?RG", sig, comparer);
        });
    }

    private static void CliqueOf5and3(IComparer<INode> comparer)
    {
        Test("Clique of 5 and 3", () =>
        {
            TestSorted("ex1", "K~{???A????S", "K~{?GK??????", comparer);
            TestSorted("ex2", "KG?O?gT??jK?", "K~{?GK??????", comparer);
            TestSorted("ex3", "K_o?_BGCeP??", "K~{?GK??????", comparer);
        });
    }

    public static void TestTweenGraphOfThree(IComparer<INode> comparer)
    {
        Test("TestTweenGraphOfThree", () =>
        {
            var sig = GetSorted(@"Cl", comparer);
            TestSorted("ex9", @"Cr", sig, comparer);
            TestSorted("ex8", @"C]", sig, comparer);
            TestSorted("ex9", @"Cl", sig, comparer);
        });
    }

    public static void TestAdamCrazyGraphI(IComparer<INode> comparer)
    {
        Test("TestAdamCrazyGraph", () =>
        {
            var sig41 = @"h~zeubOr]QWpownevRtHLevePzc`asItECjJ^{PbfC}MS_sZ}?BY~EIKCyYiH]lcOof\[}FfN`x_o|`wQLLOtOzoxIRGyYpHhRImbOuhX]CrElmfB^XZr@@mKpqnOU\z_fP@bsTg~S";
            TestSorted("ex8", @"hUxtuxmluv\m\mMvBls\mpuxblpblopuwK\m@bloEMv?K\m?K\m_EMvW@blr?K\mK?puwW@blsW@bluK?puzb?K\m[W@bltp_EMvZb?K\mZb?K\mlp_EMvZ[W@blvZb?K\m\mK?puw", sig41, comparer);
            var sig41b = @"h~zeubOrURWpoLmwTmtL?gOmZu[tXcZ}qHlc]UQzmw?M\FOZfghYe?zKvxIJBEosPg]ak}DTnXwA]Z`YdDz_kSUEZFSnkI_maRQtaptRN[CpfHqzC]vZtgAD]fpboL^lbBo?NuUD]k";
            TestSorted("ex9", @"h?`DAagtBSUgugZSUtAugJY_UtCUtEJYbaug{UtFpZS^aug^augNpZSB{UtC^augP}JYab{UtEb{UtBP}JYas^augub{UtBYNpZSUs^augUs^augjYNpZSIub{Ut@Us^augDZP}JY_", sig41b, comparer);
            Console.WriteLine("Adam the two graphs you gave me doesn't seem to match");
        });
    }

    public static void TestCaVsCo(IComparer<INode> comparer)
    {
        Test("TestCaVsCo", () =>
        {
            TestSorted("ex8", @"Ca", @"Co", comparer);
        });
    }

    public static void TestD_oVsDzc(IComparer<INode> comparer)
    {
        Test("TestD^oVsDzc", () =>
        {
            var sig = GetSorted(@"D^o", comparer);
            TestSorted("ex9", @"Dzc", sig, comparer);
            TestSorted("ex8", @"D}K", sig, comparer);
            TestSorted("ex9", @"Dls", sig, comparer);

        });
    }

    public static void TestRandomGraphs(int order, IComparer<INode> comparer, int nbGraphs = 10_000, int nbShuffle = 10)
    {
        // here I test that all shuffled graphs are sorted the same way on many graphs
        for (int i = 0; i < nbGraphs; i++)
        {
            TestRandomGraph(order, comparer, nbShuffle);
        }

    }

    public static void TestRandomGraph(int order, IComparer<INode> comparer, int nbShuffle = 10)
    {
        // here I test that many shuffled graphs are sorted the same way
        Test("TestRandomGraph", () =>
        {
            var original = RandomGraph.Random(order);
            var originalSorted = G6.fromGraph(original.Sorted(comparer));
            for (int i = 0; i < nbShuffle; i++)
            {
                var shuffled = original.Shuffled();
                var shuffledSorted = shuffled.Sorted(comparer);
                AssertEquals($"[Shuffled] #{i} {G6.fromGraph(shuffled)}", () => G6.fromGraph(shuffledSorted), originalSorted);
            }
        });
    }


    internal static void TestSorted(string message, string originalG6, string expectedG6, IComparer<INode> comparer, bool displayNodes = true)
    {
        IGraph graph = G6.parse(originalG6);
        var sortedGraph = graph.Sorted(comparer);
        AssertEquals("[Sorted] " + message, () => G6.fromGraph(sortedGraph), expectedG6);
        if (displayNodes)
        {
            DisplayNodes("Sorted:", sortedGraph);
        }
    }

    internal static string GetSorted(string originalG6, IComparer<INode> comparer, bool displayNodes = true)
    {
        IGraph graph = G6.parse(originalG6);
        if (displayNodes)
        {
            DisplayNodes("Original:", graph);
        }
        var sortedGraph = graph.Sorted(comparer);
        if (displayNodes)
        {
            DisplayNodes("Sorted:", sortedGraph);
        }
        return G6.fromGraph(sortedGraph);
    }
    private static void DisplayNodes(string title, IGraph graph)
    {
        Console.WriteLine(graph.ToString(title));
    }



}
