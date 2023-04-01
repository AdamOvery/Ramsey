namespace Pascal;


public class GraphClassification
{
    public static void Run()
    {
        const int order = 12;

        IGraph g = new MatrixGraph(order);

        // int count = 0;
        // g.ForEachGrayConfiguration(() =>
        // {
        //     count += 1;

        // });
        int edgeNo = 0;
        g.ForEachEdge((n1, n2) =>
        {
            Console.WriteLine($"{edgeNo++},{n1},{n2}");
        });
        //Console.WriteLine($"order:{order} count: {count}");

    }
}