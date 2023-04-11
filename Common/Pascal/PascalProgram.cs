namespace Pascal;


public class PascalProgram
{
    public static void PascalMain()
    {
        // Console.WriteLine("Hello, World!");
        // BFSTraversal.Tests();
        // ConnectedGroup.Tests();
        // // broken CutVerticesAlgorithm.Tests();
        // DFSTraversal.Tests();
        // OldGraphClassification.Tests();
        // NewGraphClassification.Tests();
        // GraphGrayCode.Tests();
        // IsCliqueAlgorithm.Tests();
        // IsConnectedAlgorithm.Tests();
        // LongestCycleSearch.Tests();
        // Ramsey3_4.Tests();
        // UITest.Tests();
        // ShuffledGraph.Tests();
        // RandomGraph.Tests();
        // DFSNodeComparer.Tests();
        // SortedGraph.Tests(DFSNodeComparer.instance);

        BFSNodeComparer.Tests();
        SortedGraph.Tests(BFSNodeComparer.instance);

    }


}