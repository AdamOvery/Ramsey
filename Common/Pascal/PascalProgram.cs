namespace Pascal;


public class PascalProgram
{
    public static void PascalMain()
    {
        Console.WriteLine("Hello, World!");

        MatrixGraph g = new MatrixGraph(41);

        Console.WriteLine("Creating Edges and CliqueWatcher...")
        var pentagonSearch = new PentagonCliqueWatcher(g);
        Console.WriteLine($"Originally on:{pentagonSearch.onCliques} off:{pentagonSearch.offCliques}");

        for (int a = 0; a <= 40; a++)
        {
            for (int b = 0; b < a; b++)
            {

                g.SetEdgeValue(a, b, true);
                Console.WriteLine($"Changed edge {a}-{b} on:{pentagonSearch.onCliques} off:{pentagonSearch.offCliques}");
            }
        }
    }
}