using static Clique;

namespace Pascal;

public class PentagonCliqueWatcher
{
    Edges edges;
    Triangles triangles;
    Squares squares;
    Pentagons pentagons;

    public int offCliques = 0;
    public int onCliques = 0;
    bool value;
    private IGraph graph;

    void onGraphChanged(IGraph sender)
    {
        this.graph.forEachEdge((a, b) =>
        {
            Edge edge = edges.getEdge(a, b);
            edge.value = graph.GetEdgeValue(a, b);
        });
    }

    void onEdgeChanged(IGraph sender, int n1, int n2, bool value)
    {
        Edge edge = edges.getEdge(n1, n2);
        edge.value = value;
    }

    public PentagonCliqueWatcher(IGraph graph)
    {
        this.graph = graph;
        int o = graph.order;
        edges = new Edges(o); //  (10*9) / 2 = 45
        triangles = new Triangles(o, edges);
        squares = new Squares(o, triangles);
        pentagons = new Pentagons(o, squares);

        foreach (Clique p in pentagons.GetPentagons())
        {
            p.CliqueChanged += onPentagonCliqueChanged;
        }
        offCliques = pentagons.Length;

        graph.EdgeChanged += onEdgeChanged;
        graph.GraphChanged += onGraphChanged;

    }


    protected void onPentagonCliqueChanged(Clique innerClique, CliqueValue value, CliqueValue previousValue)
    {
        if (previousValue == CliqueValue.AllOn) onCliques -= 1;
        else if (previousValue == CliqueValue.AllOff) offCliques -= 1;

        if (value == CliqueValue.AllOn) onCliques += 1;
        else if (value == CliqueValue.AllOff) offCliques += 1;

        this.value = offCliques > 0 || onCliques > 0;
    }

}