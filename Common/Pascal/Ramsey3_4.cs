namespace Pascal;


public class Ramsey3_4
{
    public static void Tests()
    {

        Check_Ramsey_3_3_is_above_5();
        Check_Ramsey_3_3_is_equal_or_less_than_6();

        // Check_Ramsey_3_4_is_above_8();
        //Check_Ramsey_3_4_is_equal_or_less_than_9();


    }

    static void Check_Ramsey_3_3_is_above_5()
    {
        var NoCliquesCount = runRamsey(3, 3, 5);
        if (NoCliquesCount == 12) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static void Check_Ramsey_3_3_is_equal_or_less_than_6()
    {
        var NoCliquesCount = runRamsey(3, 3, 6);
        if (NoCliquesCount == 0) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static void Check_Ramsey_3_4_is_above_8()
    {
        var NoCliquesCount = runRamsey(3, 4, 8);
        if (NoCliquesCount == 17640) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static void Check_Ramsey_3_4_is_equal_or_less_than_9()
    {
        var NoCliquesCount = runRamsey(3, 4, 9);
        if (NoCliquesCount == 0) Console.WriteLine("We were sort of expecting it");
        else throw new Exception("Something is wrong");
    }

    static int runRamsey(int maxCliqueOn, int maxCliqueOff, int order)
    {
        MatrixGraph g = new MatrixGraph(order);
        var cliqueWatcher = new CliqueWatcher(g, maxCliqueOn, maxCliqueOff);
        int NoCliquesCount = 0;

        g.EdgeChanged += onEdgeChanged;
        (g as IGraph).ForEachConfiguration(() =>
        {
            if ((cliqueWatcher.onCliques + cliqueWatcher.offCliques) == 0)
            {
                Console.WriteLine($"*** Cliques on:{cliqueWatcher.onCliques} off:{cliqueWatcher.offCliques} https://ramsey-paganaye.vercel.app/pascal/1?g6={G6.fromGraph(g)}");
                NoCliquesCount += 1;
            }
        });
        Console.WriteLine($"We found {NoCliquesCount} Configurations of {order} nodes with no cliques"
        + ((maxCliqueOn == maxCliqueOff) ? $" of {maxCliqueOn}."
        : $" of on {maxCliqueOn} or off {maxCliqueOff}."));
        return NoCliquesCount;
    }

    private static void onEdgeChanged(IGraph sender, int n1, int n2, bool value)
    {
        // Console.WriteLine($"Edge changed {n1}-{n2} {value}");
    }
}