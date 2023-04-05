using static Pascal.SubGraph;

namespace Pascal;


public class GraphClassification
{
    public static void Tests()
    {


        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        var g = (G6.parse("Fg???").AsSubGraph(ClassifiedNodeGraph.factory) as ClassifiedNodeGraph)!;

        var nodes = g!.nodes.Select(n => (NamedNode)n).ToList();
        var order = nodes.Count;


        for (var pass = 0; pass < 4; pass++)
        {
            foreach (var n1 in nodes)
            {
                if (pass == 0)
                {
                    n1.name.Add("{n:" + n1.id + ", a:" + n1.adjacentNodes.Count.ToString() + "}");
                }
                else if (pass == 1)
                {
                    n1.name.Add("{n:" + n1.id + ", a:[" +
                     String.Join(", ", n1.adjacentNodes.Select(n2 =>
                    {
                        var nn = (NamedNode)n2;
                        var previousName = nn.name[pass - 1];
                        //if (previousName.StartsWith("#" + n1.id + "(")) return "S";
                        return previousName ?? "?";
                    }))
                     + "]");
                }
                else if (pass == 2)
                {
                    n1.name.Add("{n:" + n1.id + ", a:[" +
                     String.Join(", ", n1.adjacentNodes.Select(n2 =>
                    {
                        var nn = (NamedNode)n2;
                        var previousName = nn.name[pass - 1];
                        //if (previousName.StartsWith("#" + n1.id + "(")) return "S";
                        return previousName ?? "?";
                    }))
                     + "]");
                }
                else
                {
                    var nextName = n1.name[0] + "[" + String.Join(",", n1.adjacentNodes.Select(n2 =>
                    {
                        var nn = (NamedNode)n2;
                        var previousName = nn.name[pass - 1];
                        //if (previousName.StartsWith("#" + n1.id + "(")) return "S";
                        return previousName ?? "?";
                    })) + "]";
                    n1.name.Add(nextName);
                }
            }
            //     {
            //         g.nodes

            //         //var n = g.nodes[i] as NamedNode;
            //         //n.name.Add(new List<int>());
            //         //n.name[pass-1].Add(i);
            //     }
            //     if (pass == 1) n

        }
        foreach (var n in nodes)
        {
            Console.WriteLine("#" + n.id);
            foreach (var o in n.name)
            {
                Console.WriteLine("   " + o);
            }
        }
    }
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
    public List<string> name;

    public NamedNode(SubGraph subGraph, int id) : base(subGraph, id)
    {
        name = new List<string>();
    }

    override public string ToString()
    {
        return string.Join(" ", name);
    }
}

