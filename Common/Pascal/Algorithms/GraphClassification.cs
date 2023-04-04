using static Pascal.SubGraph;

namespace Pascal;


public class GraphClassification
{
    public static void Tests()
    {


        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        var g = G6.parse("K~{???A????S").AsSubGraph(ClassifiedNodeGraph.factory);
        // g.BFS((int node, int? parentNode) =>
        // {
        //     Console.WriteLine($"BFS node:{node} parent:{parentNode}");
        // });

        //Console.WriteLine($"order:{order} count: {count}");
        // for (int pass = 1; pass < g.order; pass++)
        // {
        // }
        
    }

    class ClassifiedNodeGraphFactory : ISubGraphFactory
    {
        public ISubGraph CreateSubGraph(IGraph graph)
        {
            return new ClassifiedNodeGraph(graph);
        }
    }

    class ClassifiedNodeGraph : SubGraph
    {
        public readonly static new ClassifiedNodeGraphFactory factory = new ClassifiedNodeGraphFactory();

        public ClassifiedNodeGraph(IGraph graph, IEnumerable<INode>? nodes = null) : base(graph, nodes)
        {
        }

        override protected Node CreateNode(int id)
        {
            return new NamedNode(this, id);
        }
    }

    class NamedNode : Node
    {
        public NamedNode(SubGraph subGraph, int id) : base(subGraph, id)
        {
        }
    }

}
