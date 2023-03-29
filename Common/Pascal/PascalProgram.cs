namespace Pascal;


public class PascalProgram
{
    public static void PascalMain()
    {
        Console.WriteLine("Hello, World!");

        MatrixGraph g = new MatrixGraph(10);

        g.GraphChanged += onGraphChanged;
        g.EdgeChanged += onEdgeChanged;

        void onGraphChanged(IGraph sender)
        {
            Console.WriteLine("Graph changed");
        }

        void onEdgeChanged(IGraph sender, int n1, int n2, bool value)
        {
            Console.WriteLine("Edge changed");
        }
        g.Clear();
        g.SetEdgeValue(1, 2, true);

        var pentagonSearch = new PentagonCliqueSearch(g);
    }
}