using static Clique;

namespace Pascal;

public class CliqueWatcher
{
    Edges edges;
    Triangles triangles;
    Squares? squares;
    Pentagons? pentagons;

    public int offCliques;
    public int onCliques;
    bool valueOn;
    bool valueOff;
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

    public CliqueWatcher(IGraph graph, int maxCliqueOn, int maxCliqueOff)
    {

        this.graph = graph;
        int o = graph.order;
        edges = new Edges(o); //  (10*9) / 2 = 45
        triangles = new Triangles(o, edges);
        if (maxCliqueOn >= 4 || maxCliqueOff >= 4)
        {
            squares = new Squares(o, triangles);
            if (maxCliqueOn >= 5 || maxCliqueOff >= 5)
            {
                pentagons = new Pentagons(o, squares);
            }
        }

        if (maxCliqueOn == maxCliqueOff)
        {
            watchCliques(maxCliqueOn, true, true);
        }
        else
        {
            watchCliques(maxCliqueOn, true, false);
            watchCliques(maxCliqueOff, false, true);
        }

        onCliques = 0;
        offCliques = GetCliqueCollection(maxCliqueOff).Length;

        graph.EdgeChanged += onEdgeChanged;
        graph.GraphChanged += onGraphChanged;
    }

    void watchCliques(int size, bool forOnClique, bool forOffClique)
    {
        ICliqueCollection collection = GetCliqueCollection(size);

        foreach (Clique p in collection.GetCliques())
        {
            if (forOnClique) p.CliqueChanged += onCliqueOnChanged;
            if (forOffClique) p.CliqueChanged += onCliqueOffChanged;
        }
    }

    ICliqueCollection GetCliqueCollection(int size)
    {
        switch (size)
        {
            case 3: return triangles;
            case 4: return squares!;
            case 5: return pentagons!;
            default: throw new Exception($"Invalid clique size {size}");
        }
    }


    protected void onCliqueOnChanged(Clique innerClique, CliqueValue value, CliqueValue previousValue)
    {
        if (previousValue == CliqueValue.AllOn) onCliques -= 1;
        if (value == CliqueValue.AllOn) onCliques += 1;
        this.valueOn = onCliques > 0;
    }

    protected void onCliqueOffChanged(Clique innerClique, CliqueValue value, CliqueValue previousValue)
    {
        if (previousValue == CliqueValue.AllOff) offCliques -= 1;
        if (value == CliqueValue.AllOff) offCliques += 1;

        this.valueOff = offCliques > 0;
    }
}