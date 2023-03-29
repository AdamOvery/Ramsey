class Squares
{
    private Clique[] squares;
    private int order;
    public int Length { get; private set; }

    public Squares(int order, Triangles triangles)
    {
        this.order = order;
        this.Length = order * (order - 1) * (order - 2) * (order - 3) / 24;
        squares = new Clique[Length]; //  (10*9*8*7)/24 = 210

        var idx = 0;
        for (var d = 3; d < order; d++)
        {
            for (var c = 0; c < d; c++)
            {
                for (var b = 0; b < c; b++)
                {
                    for (var a = 0; a < b; a++)
                    {
                        var t1 = triangles.getTriangle(a, b, c/**/);
                        var t2 = triangles.getTriangle(a, b, /**/d);
                        var t3 = triangles.getTriangle(a, /**/c, d);
                        var t4 = triangles.getTriangle(/**/b, c, d);
                        Clique square = new Clique(4, $"{a}-{b}-{c}-{d}", new Clique[] { t1, t2, t3, t4 });
                        squares[idx++] = square;
                    }
                }
            }
        }
    }

    public Clique getSquare(int a, int b, int c, int d)
    {
        if (a >= b || b >= c || c >= d) throw new ArgumentException("Invalid Square");
        var squareNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24;
        return squares[squareNo];
    }
}