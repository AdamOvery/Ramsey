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
        TestR3X();
    }

    static void TestR3X()
    {
        TestR(3, 3, 6);
        TestR(3, 4, 9);
        TestR(3, 5, 14);
        TestR(3, 6, 18);
        TestR(3, 7, 23);
    }

    static void TestR(int onClique, int offClique, int knownResult)
    {
        for (int order = knownResult - 2; order <= knownResult; order++)
        {
            Console.WriteLine("====================================================================================");
            bool found = FindRamseyCounterExample(order, onClique, offClique);
            if (order < knownResult)
            {
                if (found) Console.WriteLine($"[PASS] ** Found a counter example for R({onClique},{offClique}) = {order}. This is expected R({onClique},{offClique}) is {knownResult}.");
                else Console.WriteLine($"[FAIL] ** No counter example found for R({onClique},{offClique}) = {order}. This is bad R({onClique},{offClique}) is {knownResult}.");
            }
            else
            {
        if (found) Console.WriteLine($"[FAIL]] ** Found a counter example for R({onClique},{offClique}) = {order}. This is really odd R({onClique},{offClique}) is {knownResult}."); 
                else  Console.WriteLine($"[PASS] ** Found No counter example found for R({onClique},{offClique}) = {order}. This is expected. R({onClique},{offClique}) is {knownResult}");
            }
        }
    }


    static void RandomTests()
    {
        bool B0 = false;
        bool B1 = true;


        TryGraph("BiggestR44", 17, 4, 4, new bool[]
            //00  01  02  03  04  05  06  07  08  09  
            { B0, B1, B1, B0, B1, B0, B0, B0, B1, B0,
        //10  11  12  13  14  15  16
          B0, B0, B0, B0, B0, B0, B0});
        Console.WriteLine("Expecting https://ramsey-paganaye.vercel.app/pascal/1?g6=PzlXWmJpZDeJEJbDgp%5CEJsWk");


        FindRamseyCounterExample(7, 3, 4);
        FindRamseyCounterExample(8, 3, 4);
        FindRamseyCounterExample(9, 3, 4);
        FindRamseyCounterExample(10, 3, 4);
        FindRamseyCounterExample(11, 3, 4);
        // ** R(4,4) = we should not find any 0 Off 0 On cliques



        FindRamseyCounterExample(17, 4, 4);
        // ** R(4,4) > 17: PCQefPsMcyXsxs[yVMaxsJfO Off cliques: 0, On Clique: 0

        FindRamseyCounterExample(18, 4, 4);
        // ** R(4,4) = we should not find any 0 Off 0 On cliques


        FindRamseyCounterExample(38, 5, 5);
        // it takes like 5 minutes then
        // ** R(5,5) > 38: ehfNNfx~J{fxfxR{s~IfxI^cS~GS~GI^cAfx?S~G@R{_Afx_AfxO@R{s?S~I_AfxI?I^eS?S~MS?S~NI?I^fq_Afx}S?S~NxO@R{nq_AfxNq_AfxfxO@R{_ Off cliques: 0, On Clique: 0

        FindRamseyCounterExample(39, 5, 5);
        // After 24 hours or so
        // ... 0.0038235157% R(5,5) f?`DEbg|BsFgFgbsW|FFg[]aw|Aw|@[]_VFgAw|CJbsGVFggVFgsJbs\Aw|BgVFgM`[]_\Aw|C\Aw|EM`[]bbgVFg{\Aw|BpsJbsVbgVFgVbgVFgJpsJbsA{\Aw|? Off cliques: 312, On Clique: 156
        // ... 0.0039090082% R(5,5) fCrffr{^d}Z{z{\}F^_z{BnoF^cF^aBno_z{cF^aO\}C_z{C_z{AO\}?cF^cC_z{oQBnr_cF^b_cF^doQBnr[C_z{z_cF^fmAO\}^[C_z{~[C_z{^mAO\}Fz_cF^_ Off cliques: 351, On Clique: 78
        // ... 0.0039653969% R(5,5) f?AEB?oeEWKokouW\eFko]rA|eA|eD]rBVkoY|eDjuWJVkojVkotjuW\Y|eFjVko]l]rA|Y|eE|Y|eB]l]r?vjVkoe|Y|eEZtjuWKvjVkoKvjVkoEZtjuW@e|Y|e? Off cliques: 117, On Clique: 819
        // we're 4 order of magnitude away from the finding a 39 nodes solution
        // we're 16 order of magnitude away from the finding a 40 nodes counter example
        // we're 28 order of magnitude away from the finding a 41 nodes counter example
        // we're 41 order of magnitude away from the finding a 42 nodes counter example
        // we're 53 order of magnitude away from the finding a 43 nodes counter example
        // we're 67 order of magnitude away from the finding a 44 nodes counter example
        // we're 80 order of magnitude away from the finding a 45 nodes counter example

        Console.WriteLine("Expecting nothing this is not a solution");
    }

    static bool FindRamseyCounterExample(int order, int maxCliqueOn, int maxCliqueOff)
    {
        bool foundCounterExample = false;
        Title($"Checking if R({maxCliqueOn},{maxCliqueOff}) > {order}", () =>
        {
            SymmetricGraph g = new SymmetricGraph(order);
            CliqueWatcher w = new CliqueWatcher(g, maxCliqueOn, maxCliqueOff);
            long cpt = 0;
            double cptMax = Math.Pow(2.0, order - 1);
            int best = 1000;

            GraphGrayCode.ForEachGrayCode(order - 2, (i, b) =>
            {

                if (w.offCliques + w.onCliques < best)
                {
                    best = w.offCliques + w.onCliques;
                    Console.WriteLine($"... {cpt * 100.0 / cptMax:0.##########}% Best R({maxCliqueOn},{maxCliqueOff}) {G6.fromGraph(g)} Off cliques: {w.offCliques}, On Clique: {w.onCliques}");
                }
                else if (cpt % 5_000L == 0)
                {
                    Console.WriteLine($"... {cpt * 100.0 / cptMax:0.##########}% R({maxCliqueOn},{maxCliqueOff}) {G6.fromGraph(g)} Off cliques: {w.offCliques}, On Clique: {w.onCliques}");
                }
                cpt++;
                g.SetEdgeValue(0, i + 1, b);
                if (w.onCliques == 0 && w.offCliques == 0)
                {
                    Console.WriteLine($"** Found a counter example of R({maxCliqueOn},{maxCliqueOff}) < {order}: {G6.fromGraph(g)} Off cliques: {w.offCliques}, On Clique: {w.onCliques}. We can say: R({maxCliqueOn},{maxCliqueOff}) > {order}");
                    foundCounterExample = true;
                    return true;
                }
                else return false;
            });

            if (!foundCounterExample)
            {
                Console.WriteLine($"** We have not found a counter example of R({maxCliqueOn},{maxCliqueOff}) < {order}, in {cpt} configations. We are going to claim that R({maxCliqueOn},{maxCliqueOff}) <= {order}");
            }
        });
        return foundCounterExample;
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