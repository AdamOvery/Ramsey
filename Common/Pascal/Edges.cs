class Edges
{
    private Edge[] edges;

    public Edges(int order)
    {
        this.edges = new Edge[order * (order - 1) / 2];
        int idx = 0;
        for (var b = 1; b < order; b++)
        {
            for (var a = 0; a < b; a++)
            {
                Edge edge = new Edge(a, b);
                edges[idx++] = edge;
            }
        }
    }

    internal Edge getEdge(int a, int b)
    {
        int edgeNo;
        if (a < b) edgeNo = a + b * (b - 1) / 2;
        else if (a > b) edgeNo = b + a * (a - 1) / 2;
        else throw new ArgumentException("Invalid Edge");
        return edges[edgeNo];
    }

    static int getEdgeNo(int a, int b)
    {
        return a + b * (b - 1) / 2;
    }



}
