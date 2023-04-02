namespace Pascal;


public class GraphGrayCode
{
    public static void Tests()
    {
        ForEachGrayConfigurationStats(true, 4, 4);

    }

    public static void ForEachGrayConfigurationStats(bool verbose = false, int minOrder = 3, int maxOrder = 8)
    {
        for (int order = minOrder; order <= maxOrder; order++)
        {
            long normalEdgeChangedCount = 0;
            long grayEdgeChangedCount = 0;
            {
                IGraph g = new MatrixGraph(order);
                g.EdgeChanged += (sender, n1, n2, value) => { normalEdgeChangedCount += 1; };
                int cpt = 0;
                g.ForEachConfiguration(() =>
                {
                    if (verbose) Console.WriteLine((cpt++) + " " + G6.fromGraph(g) + " " + EdgeBits.fromGraph(g) + " " + normalEdgeChangedCount);
                });
            }
            {
                IGraph g = new MatrixGraph(order);
                g.EdgeChanged += (sender, n1, n2, value) => { grayEdgeChangedCount += 1; };
                int cpt = 0;
                g.ForEachGrayConfiguration(() =>
                {
                    if (verbose) Console.WriteLine((cpt++) + " " + G6.fromGraph(g) + " " + EdgeBits.fromGraph(g) + " " + grayEdgeChangedCount);
                });
            }
            Console.WriteLine($"order:{order} normalEdgeChangedCount: {normalEdgeChangedCount}"
                + $" grayEdgeChangedCount: {grayEdgeChangedCount}"
                + $" {grayEdgeChangedCount * 100L / normalEdgeChangedCount}%");
        }
        /*
        Gray codes divides the number of configuration changes by 2
        order normalEdgeChangedCount grayEdgeChangedCount   ratio
            3                     11                    7   63 %
            4                    120                   63   52 %
            5                  2 036                1 023   50 %
            6                 65 519               32 767   50 %
            7              4 194 281            2 097 151   50 %
            8            536 870 882          268 435 455   50 %
        */
    }

    // function that prints the gray codes 1 to 255
    static void printGrayCodes1()
    {
        int i;
        int last = 0;
        for (i = 0; i <= 255; i++)
        {
            var gray = (i ^ (i >> 1));
            var diff = gray ^ last;

            Console.WriteLine(i + ") " + gray + " " + Convert.ToString(gray, 2).PadLeft(8, '0') + " " + diff);
            last = gray;
        }
    }

    static void printGrayCodes2()
    {
        Action<int> forEachEdge = (a0) => { };
        int gray = 0;
        int cpt = 0;
        var diff = 0;

        Console.WriteLine((cpt++) + ") " + gray + " " + Convert.ToString(gray, 2).PadLeft(8, '0') + " " + diff);
        forEachEdge = (n) =>
        {
            if (n == 0)
            {
                diff = 1 << n;
                gray = gray ^ (1 << n);
                Console.WriteLine((cpt++) + ") " + gray + " " + Convert.ToString(gray, 2).PadLeft(8, '0') + " " + diff);
            }
            else
            {
                forEachEdge(n - 1);
                diff = 1 << n;
                gray = gray ^ (1 << n);
                Console.WriteLine((cpt++) + " " + gray + " " + Convert.ToString(gray, 2).PadLeft(8, '0') + " " + diff);
                forEachEdge(n - 1);
            }
        };
        forEachEdge(7);
    }
}