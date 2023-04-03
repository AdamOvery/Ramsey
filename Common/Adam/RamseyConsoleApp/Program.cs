using Ramsey.Adam.RamseyLibrary;

class AdamProgram
{
    public static void AdamMain()
    {
        Console.WriteLine("Enter Ramsey Type: 'A' -> Quick Full Symmetrical or 'B' -> Node Identification");
        var ramseyGraphType = Console.ReadLine()?.ToUpperInvariant();
        if (!new string[] { "A", "B" }.Contains(ramseyGraphType))
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

        var ramseyConfig = new RamseyConfig(nodeCount, maxCliqueOn, maxCliqueOff, findAllSolutions ?? false);

        IRamseyGraph ramsey;
        if (ramseyGraphType == "A")
        {
            ramsey = new RamseyGraphA(ramseyConfig);
        }
        else
        {
            Console.WriteLine("Enter minimum 'On' edge count per node");
            var minEdgeCountString = Console.ReadLine();

            if (!int.TryParse(minEdgeCountString, out var minEdgeCount))
            {
                Console.WriteLine("Invalid Response");
                return;
            }

            var ramseyB = new RamseyGraphB(ramseyConfig);
            ramseyB.MinEdgeCount = minEdgeCount;

            ramsey = ramseyB;
            
        }
        ramsey.InitializeGraph();

        var description = $"R({ramseyConfig.MaxCliqueOn},{ramseyConfig.MaxCliqueOff}). Iterations: {ramsey.Iterations}. Node Count: {ramseyConfig.NodeCount}";
        var timeTaken = string.Format("{0:0.00}s", ramsey.TimeTaken.TotalMilliseconds / 1000);

        if (ramsey.IsSuccess)
        {
            foreach (var solution in ramsey.Solutions)
            {
                Console.WriteLine($"{description}. Solution = {solution.EdgeLinkDescription}. Found in {timeTaken}");
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