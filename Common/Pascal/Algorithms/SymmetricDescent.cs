
using Pascal;
using static Pascal.TestEngine;

static class SymmetricDescent
{
    internal static void Tests()
    {
        BiggestR44();
    }

    static void BiggestR44()
    {
        bool B0 = false;
        bool B1 = true;

        TryGraph("BiggestR44", 17, 4, 4, new bool[] 
        //00  01  02  03  04  05  06  07  08  09  
        { B0, B1, B1, B0, B1, B0, B0, B0, B1, B0,
        //10  11  12  13  14  15  16
          B0, B0, B0, B0, B0, B0, B0});
        Console.WriteLine("Expecting https://ramsey-paganaye.vercel.app/pascal/1?g6=PzlXWmJpZDeJEJbDgp%5CEJsWk");

        TryGraphs("BiggestR44", 17, 4, 4);
        Console.WriteLine("Expecting nothing this is not a solution");

        for (int order = 30; order < 50; order++)
        {
            TryGraphs("BiggestR55 order:" + order, order, 5, 5);
        }
        Console.WriteLine("Expecting nothing this is not a solution");
    }

    static void TryGraphs(String title, int order, int maxCliqueOn, int maxCliqueOff)
    {
        Test(title, () =>
        {
            SymmetricGraph g = new SymmetricGraph(order);
            CliqueWatcher w = new CliqueWatcher(g, maxCliqueOn, maxCliqueOff);

            GraphGrayCode.ForEachGrayCode(order - 2, (i, b) =>
            {
                g.SetEdgeValue(0, i + 1, b);
                if (w.onCliques == 0 && w.offCliques == 0)
                {
                    Console.WriteLine(G6.fromGraph(g));
                    Console.WriteLine("Off cliques: " + w.offCliques + ", On Clique: " + w.onCliques);
                    return true;
                }  else return false;
            });
        });
    }

    static void TryGraph(String title, int order, int maxCliqueOn, int maxCliqueOff, bool[] edges)
    {
        Test(title, () =>
        {
            SymmetricGraph g = new SymmetricGraph(order);
            CliqueWatcher w = new CliqueWatcher(g, maxCliqueOn, maxCliqueOff);
            for (int b = 0; b < edges.Length; b++)
            {
                if (edges[b]) g.SetEdgeValue(0, b, true);
            }
            Console.WriteLine(G6.fromGraph(g));
            Console.WriteLine("Off cliques: " + w.offCliques + ", On Clique: " + w.onCliques);
        });
    }

}