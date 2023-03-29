namespace Pascal;


public class PascalProgram
{
    public static void PascalMain()
    {
        Console.WriteLine("Hello, World!");

        int order = 6;

        G6.Tests();

        MatrixGraph g = new MatrixGraph(order);
        var edgeCount = order * (order - 1) / 2;
        Console.WriteLine($"Creating {edgeCount} Edges and CliqueWatcher...");
        var cliqueWatcher = new CliqueWatcher(g, 3, 3);
        Console.WriteLine($"Originally on:{cliqueWatcher.onCliques} off:{cliqueWatcher.offCliques}");

        for (int a = 0; a < order; a++)
        {
            for (int b = 0; b < a; b++)
            {
                g.SetEdgeValue(a, b, true);
                Console.WriteLine($"Changed edge {a}-{b} on:{cliqueWatcher.onCliques} off:{cliqueWatcher.offCliques} {G6.fromGraph(g)}");
            }
        }
    }
}