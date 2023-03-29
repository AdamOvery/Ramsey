namespace Pascal;


public class PascalProgram
{
    public static void PascalMain()
    {
        Console.WriteLine("Hello, World!");

        Check_Ramsey_3_3_is_above_5();
        Check_Ramsey_3_3_is_equal_or_less_than_6();
    }

    static void Check_Ramsey_3_3_is_above_5()
    {
        var NoCliquesCount = runRamsey3x3(5);
        if (NoCliquesCount == 12) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static void Check_Ramsey_3_3_is_equal_or_less_than_6()
    {
        var NoCliquesCount = runRamsey3x3(6);
        if (NoCliquesCount == 0) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static int runRamsey3x3(int order)
    {
        MatrixGraph g = new MatrixGraph(order);
        var edgeCount = order * (order - 1) / 2;
        var cliqueWatcher = new CliqueWatcher(g, 3, 3);
        int NoCliquesCount = 0;

        g.EdgeChanged += onEdgeChanged;
        (g as IGraph).forEachConfiguration(() =>
        {
            if ((cliqueWatcher.onCliques + cliqueWatcher.offCliques) == 0)
            {
                Console.WriteLine($"*** Cliques on:{cliqueWatcher.onCliques} off:{cliqueWatcher.offCliques} {G6.fromGraph(g)}");
                NoCliquesCount += 1;
            }
        });
        Console.WriteLine($"We found {NoCliquesCount} Configurations of {order} nodes with no cliques of 3");
        return NoCliquesCount;
    }

    private static void onEdgeChanged(IGraph sender, int n1, int n2, bool value)
    {
        // Console.WriteLine($"Edge changed {n1}-{n2} {value}");
    }
}