class Triangles
{
    private Clique[] triangles;
    private int order;
    public int Length { get; private set; }

    public Triangles(int order, Edges edges)
    {
        this.order = order;
        this.Length = order * (order - 1) * (order - 2) / 6;
        triangles = new Clique[Length]; //  (10*9*8)/6 = 120

        var idx = 0;
        for (var c = 2; c < order; c++)
        {
            for (var b = 0; b < c; b++)
            {
                for (var a = 0; a < b; a++)
                {
                    var e1 = edges.getEdge(a, b/**/);
                    var e2 = edges.getEdge(a, /**/c);
                    var e3 = edges.getEdge(/**/b, c);
                    Clique triangle = new Clique(3, $"{a}-{b}-{c}", new Clique[] { e1, e2, e3 });
                    triangles[idx++] = triangle;
                }
            }
        }
    }

    public Clique getTriangle(int a, int b, int c)
    {
        if (a >= b || b >= c) throw new ArgumentException("Invalid Triangle");
        var triangleNo = a + b * (b - 1) / 2 + c * (c - 1) * (c - 2) / 6;
        return triangles[triangleNo];
    }
}