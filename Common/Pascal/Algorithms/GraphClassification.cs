namespace Pascal;


public class GraphClassification
{
    public static void Tests()
    {


        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        IGraph g = G6.parse("K~{???A????S");
        // g.BFS((int node, int? parentNode) =>
        // {
        //     Console.WriteLine($"BFS node:{node} parent:{parentNode}");
        // });
        var longestCycle = g.LongestCycle(0);

        Console.WriteLine(value: $"longestCycle:{StringUtils.Join1(longestCycle)}");
        //Console.WriteLine($"order:{order} count: {count}");

    }


}
