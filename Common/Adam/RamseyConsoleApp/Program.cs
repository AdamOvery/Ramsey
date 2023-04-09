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
        Console.WriteLine("");

        // Test Code only
        //var graphG6 = "Ls`?XGRQR@B`Kc";
        //MatrixGraphAdam testGraph = (MatrixGraphAdam)G6.parse(graphG6, MatrixGraphAdam.factory);
        //var nodeLoops = NodeLoopUtil.FindAllNodeLoops(testGraph.edges);

        Console.WriteLine("Enter Ramsey Type: 'A' -> Quick Full Symmetrical, 'B' -> Node Identification, 'C' - R(3,x) Type 1");
        var ramseyGraphType = Console.ReadLine()?.ToUpperInvariant();
        if (!new string[] { "A", "B", "C" }.Contains(ramseyGraphType))
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

        int? minEdgeCount = null;

        if (new string[] { "B", "C" }.Contains(ramseyGraphType))
        {
            Console.WriteLine("Enter minimum 'On' edge count per node");
            var minEdgeCountString = Console.ReadLine();

            if (!int.TryParse(minEdgeCountString, out var minEdgeCountOut))
            {
                Console.WriteLine("Invalid Response");
                return;
            }

            minEdgeCount = minEdgeCountOut;
        }

        var ramseyConfig = new RamseyConfig(nodeCount, maxCliqueOn, maxCliqueOff, findAllSolutions ?? false, minEdgeCount);

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
        }
        else
        {
            Console.WriteLine($"{description}. No solution Found in {timeTaken}");
        }

        Console.WriteLine("Press 'return' to end");
        Console.ReadLine();
    }
}