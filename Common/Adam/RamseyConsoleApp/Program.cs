using Pascal;
using Ramsey.Adam.RamseyLibrary;

class AdamProgram
{
    public static void AdamMain()
    {
        Console.WriteLine("Example 1: A,4,4,17,Y");
        Console.WriteLine("Example 2: A,5,5,41,Y");
        Console.WriteLine("Example 3: B,3,4,8,Y,2");
        Console.WriteLine("Example 4: C,3,4,8,Y,3");
        Console.WriteLine("Example 5: C,3,5,12,Y,4");
        Console.WriteLine("Example 6: D,3,4,8,Y,3,N");
        Console.WriteLine("Example 7: D,3,5,12,Y,4,N (This takes about 2 mins)");
        Console.WriteLine("");

        // Test Code only
        ////var graphG6 = "GsOiho";
        ////var graphG6 = "Ls`?XGRQR@B`Kc";
        ////var graphG6 = "UsaCB@AQAGI?HGIABGOKJCcoa[``o_U[?LcGD[O?";
        //var graphG6 = "UsaCAHAQA_K?GcC`@`XHaDH_Xa@QQOKwGJKOK[G?";
        //var graphG6 = "bsaCCA?O?O_aC??`c@O@b?RcHQ?DcA@H?PC_@QO@@Q?DA@MP?RgKADoH?aR?KCbOC_JE?xO@AALC@AJPG?__V?_K@ROO_A@dOG@?_";
        //MatrixGraphAdam testGraph = (MatrixGraphAdam)G6.parse(graphG6, MatrixGraphAdam.factory);
        //var nodeLoops = NodeLoopUtil.FindAllNodeLoops(testGraph.edges);
        //var ramseyEdges = NodeLoopUtil.FindAllRamseyEdges(nodeLoops, testGraph.edges.GetLength(0));
        //var edgeCounts = new Dictionary<int, int>();
        //foreach (var edge in ramseyEdges)
        //{
        //    if (!edgeCounts.ContainsKey(edge.Count))
        //    {
        //        edgeCounts.Add(edge.Count, 1);
        //    }
        //    else
        //    {
        //        edgeCounts[edge.Count]++;
        //    }
        //}

        //var graphG6 = "Ls`?XGRQR@B`Kc";
        //MatrixGraphAdam testGraph = (MatrixGraphAdam)G6.parse(graphG6, MatrixGraphAdam.factory);
        //var nodeCountTest = testGraph.edges.GetLength(0);

        //while (true)
        //{
        //    Console.WriteLine(graphG6);
        //    Console.WriteLine("Enter node1");
        //    var node1String = Console.ReadLine();
        //    if (!int.TryParse(node1String, out var swap1))
        //    {
        //        return;
        //    }
        //    Console.WriteLine("Enter node2");
        //    var node2String = Console.ReadLine();
        //    if (!int.TryParse(node2String, out var swap2))
        //    {
        //        return;
        //    }

        //    var newEdges = testGraph.edges.Clone() as bool[,] ?? new bool[nodeCountTest, nodeCountTest];
        //    for (var loop = 0; loop < nodeCountTest; loop++)
        //    {
        //        newEdges[loop, swap1] = testGraph.edges[loop, swap2];
        //        newEdges[loop, swap2] = testGraph.edges[loop, swap1];
        //        newEdges[swap1, loop] = testGraph.edges[swap2, loop];
        //        newEdges[swap2, loop] = testGraph.edges[swap1, loop];
        //    }

        //    testGraph.edges = newEdges;
        //    graphG6 = G6.fromGraph(testGraph);
        //}


        Console.WriteLine($"Enter Ramsey Type:{Environment.NewLine}'A' -> Quick Full Symmetrical{Environment.NewLine}'B' -> Node Identification{Environment.NewLine}'C' - R(3,x) Type 1{Environment.NewLine}'D' - R(3,x) Generic");
        var ramseyGraphType = Console.ReadLine()?.ToUpperInvariant();
        if (!new string[] { "A", "B", "C", "D" }.Contains(ramseyGraphType))
        {
            Console.WriteLine("Invalid Response");
            return;
        }

        Console.WriteLine("Enter Max 'On' clique");
        var maxCliqueOnString = Console.ReadLine();

        if (!int.TryParse(maxCliqueOnString, out var maxCliqueOn))
        {
            Console.WriteLine("Invalid Response");
            return;
        }

        Console.WriteLine("Enter Max 'Off' clique");
        var maxCliqueOffString = Console.ReadLine();
        if (!int.TryParse(maxCliqueOffString, out var maxCliqueOff))
        {
            Console.WriteLine("Invalid Response");
            return;
        }

        Console.WriteLine("Enter node count");
        var nodeCountString = Console.ReadLine();
        if (!int.TryParse(nodeCountString, out var nodeCount))
        {
            Console.WriteLine("Invalid Response");
            return;
        }

        Console.WriteLine("Find all solutions? (Y/N)");

        var findAllSolutionsString = Console.ReadLine()?.ToUpperInvariant();
        bool? findAllSolutions = (findAllSolutionsString == "N") ? false : (findAllSolutionsString == "Y") ? true : null;
        if (findAllSolutions is null)
        {
            Console.WriteLine("Invalid Response");
            return;
        }

        int? nodeEdgeCount = null;
        int? minNodeEdgeCount = null;

        if (new string[] { "B", "C" }.Contains(ramseyGraphType))
        {
            Console.WriteLine("Enter minimum 'On' edge count per node");
            var nodeEdgeCountString = Console.ReadLine();

            if (!int.TryParse(nodeEdgeCountString, out var nodeEdgeCountOut))
            {
                Console.WriteLine("Invalid Response");
                return;
            }

            nodeEdgeCount = nodeEdgeCountOut;
        }

        if (new string[] { "D" }.Contains(ramseyGraphType))
        {
            Console.WriteLine("Enter 'On' edge count per node");
            var nodeEdgeCountString = Console.ReadLine();

            if (!int.TryParse(nodeEdgeCountString, out var nodeEdgeCountOut))
            {
                Console.WriteLine("Invalid Response");
                return;
            }

            nodeEdgeCount = nodeEdgeCountOut;

            Console.WriteLine("Check for solutions with one fewer edge per node? (Y/N)");

            var checkForSolutionsWithOneFewerEdgeString = Console.ReadLine()?.ToUpperInvariant();
            bool? checkForSolutionsWithOneFewerEdge = (checkForSolutionsWithOneFewerEdgeString == "N") ? false : (checkForSolutionsWithOneFewerEdgeString == "Y") ? true : null;
            if (checkForSolutionsWithOneFewerEdge is null)
            {
                Console.WriteLine("Invalid Response");
                return;
            }

            if (checkForSolutionsWithOneFewerEdge == true)
            {
                minNodeEdgeCount = nodeEdgeCount - 1;
            }
            else
            {
                minNodeEdgeCount = nodeEdgeCount;
            }
        }

        var ramseyConfig = new RamseyConfig(nodeCount, maxCliqueOn, maxCliqueOff, findAllSolutions ?? false, nodeEdgeCount, minNodeEdgeCount);

        IRamseyGraph ramsey;
        if (ramseyGraphType == "A")
        {
            ramsey = new RamseyGraphA2(ramseyConfig);
        }
        else if (ramseyGraphType == "B")
        {
            ramsey = new RamseyGraphB(ramseyConfig);
        }
        else if (ramseyGraphType == "C")
        {
            ramsey = new RamseyGraphC(ramseyConfig);
        }
        else if (ramseyGraphType == "D")
        {
            ramsey = new RamseyGraphD(ramseyConfig);
        }
        else
        {
            throw new InvalidOperationException("Invalid graph type");
        }

        ramsey.InitializeGraph();

        var description = $"R({ramseyConfig.MaxCliqueOn},{ramseyConfig.MaxCliqueOff}). Iterations: {ramsey.Iterations}. Node Count: {ramseyConfig.NodeCount}";
        var timeTaken = string.Format("{0:0.000}s", ramsey.TimeTaken.TotalMilliseconds / 1000);

        if (ramsey.IsSuccess)
        {
            foreach (var solution in ramsey.Solutions)
            {
                Console.WriteLine($"{description}. G6={solution.G6Code}. Solution = {Environment.NewLine}{solution.EdgeDescription}. Found in {timeTaken}");
            }
            Console.WriteLine($"Solution count: {ramsey.Solutions.Count}");
        }
        else
        {
            Console.WriteLine($"{description}. No solution Found in {timeTaken}");
        }

        Console.WriteLine("Press 'return' to end");
        Console.ReadLine();
    }
}