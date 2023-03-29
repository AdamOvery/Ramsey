using static Clique;

namespace Pascal;

public class PentagonCliqueSearch
{
    Edge[] edges;
    Clique[] triangles;
    Clique[] squares;
    Clique[] pentagons;

    int offInnerCliquesCount = 0;
    int onInnerCliquesCount = 0;
    bool value;


    public PentagonCliqueSearch(IGraph graph)
    {
        int o = graph.order;
        edges = new Edge[o * (o - 1) / 2]; //  (10*9) / 2 = 45
        triangles = new Clique[o * (o - 1) * (o - 2) / 6]; //  (10*9*8)/6 = 120
        squares = new Clique[o * (o - 1) * (o - 2) * (o - 3) / 24]; //  (10*9*8*7)/24 = 210
        pentagons = new Clique[o * (o - 1) * (o - 2) * (o - 3) * (o - 4) / 120]; // (10*9*8*7*6)/120 = 255

        // recursion is for people who don't understand cut and paste
        int idx = 0;
        for (var b = 1; b < o; b++)
        {
            for (var a = 0; a < b; a++)
            {
                Edge edge = new Edge(a, b);
                edges[idx++] = edge;
                Assert(getEdge(a, b) == edge);
            }
        }

        idx = 0;
        for (var c = 2; c < o; c++)
        {
            for (var b = 0; b < c; b++)
            {
                for (var a = 0; a < b; a++)
                {
                    var e1 = getEdge(a, b/**/);
                    var e2 = getEdge(a, /**/c);
                    var e3 = getEdge(/**/b, c);
                    Clique triangle = new Clique(3, $"{a}-{b}-{c}", new Clique[] { e1, e2, e3 });
                    triangles[idx++] = triangle;
                    Assert(getTriangle(a, b, c) == triangle);
                }
            }
        }
        idx = 0;
        for (var d = 3; d < o; d++)
        {
            for (var c = 0; c < d; c++)
            {
                for (var b = 0; b < c; b++)
                {
                    for (var a = 0; a < b; a++)
                    {
                        var t1 = getTriangle(a, b, c/**/);
                        var t2 = getTriangle(a, b, /**/d);
                        var t3 = getTriangle(a, /**/c, d);
                        var t4 = getTriangle(/**/b, c, d);
                        Clique square = new Clique(4, $"{a}-{b}-{c}-{d}", new Clique[] { t1, t2, t3, t4 });
                        squares[idx++] = square;
                        Assert(getSquare(a, b, c, d) == square);
                    }
                }
            }
        }

        idx = 0;
        for (var e = 4; e < o; e++)
        {
            for (var d = 0; d < e; d++)
            {
                for (var c = 0; c < d; c++)
                {
                    for (var b = 0; b < c; b++)
                    {
                        for (var a = 0; a < b; a++)
                        {
                            var s1 = getSquare(a, b, c, d/**/);
                            var s2 = getSquare(a, b, c, /**/e);
                            var s3 = getSquare(a, b, /**/d, e);
                            var s4 = getSquare(a, /**/c, d, e);
                            var s5 = getSquare(/**/b, c, d, e);
                            Clique pentagon = new Clique(5, $"{a}-{b}-{c}-{d}-{e}", new Clique[] { s1, s2, s3, s4, s5 });
                            pentagons[idx++] = pentagon;
                            Assert(getPentagon(a, b, c, d, e) == pentagon);
                        }
                    }
                }
            }

        }
        foreach (Clique p in pentagons)
        {
            p.CliqueChanged += onInnerCliqueChanged;
        }
        offInnerCliquesCount = pentagons.Length;
    }


    protected void onInnerCliqueChanged(Clique innerClique, CliqueValue value, CliqueValue previousValue)
    {
        if (previousValue == CliqueValue.AllOn) onInnerCliquesCount -= 1;
        else if (previousValue == CliqueValue.AllOff) offInnerCliquesCount -= 1;

        if (value == CliqueValue.AllOn) onInnerCliquesCount += 1;
        else if (value == CliqueValue.AllOff) offInnerCliquesCount += 1;

        this.value = offInnerCliquesCount > 0 || onInnerCliquesCount > 0;
    }


    private Edge getEdge(int a, int b)
    {
        if (a >= b) throw new ArgumentException("Invalid Edge");
        var edgeNo = a + b * (b - 1) / 2;
        return edges[edgeNo];
    }

    private Clique getTriangle(int a, int b, int c)
    {
        if (a >= b || b >= c) throw new ArgumentException("Invalid Triangle");
        var triangleNo = a + b * (b - 1) / 2 + c * (c - 1) * (c - 2) / 6;
        return triangles[triangleNo];
    }

    private Clique getSquare(int a, int b, int c, int d)
    {
        if (a >= b || b >= c || c >= d) throw new ArgumentException("Invalid Square");
        var squareNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24;
        return squares[squareNo];
    }

    private Clique getPentagon(int a, int b, int c, int d, int e)
    {
        if (a >= b || b >= c || c >= d || d >= e) throw new ArgumentException("Invalid Pentagon");
        var pentagonNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24
          + e * (e - 1) * (e - 2) * (e - 3) * (e - 4) / 120;
        return pentagons[pentagonNo];
    }


    private void Assert(bool value)
    {
        if (!value) throw new Exception("Internal Assertion failed");
    }
}