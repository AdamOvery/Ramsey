using System.Collections;

class Pentagons
{
    private Clique[] pentagons;
    private int order;
    public int Length { get; private set; }

    public Pentagons(int order, Squares squares)
    {
        this.order = order;
        this.Length = order * (order - 1) * (order - 2) * (order - 3) * (order - 4) / 120;
        pentagons = new Clique[Length]; // (10*9*8*7*6)/120 = 255

        var idx = 0;
        for (var e = 4; e < order; e++)
        {
            for (var d = 0; d < e; d++)
            {
                for (var c = 0; c < d; c++)
                {
                    for (var b = 0; b < c; b++)
                    {
                        for (var a = 0; a < b; a++)
                        {
                            var s1 = squares.getSquare(a, b, c, d/**/);
                            var s2 = squares.getSquare(a, b, c, /**/e);
                            var s3 = squares.getSquare(a, b, /**/d, e);
                            var s4 = squares.getSquare(a, /**/c, d, e);
                            var s5 = squares.getSquare(/**/b, c, d, e);
                            Clique pentagon = new Clique(5, $"{a}-{b}-{c}-{d}-{e}", new Clique[] { s1, s2, s3, s4, s5 });
                            pentagons[idx++] = pentagon;
                        }
                    }
                }
            }

        }
    }


    public IEnumerable<Clique> GetPentagons()
    {
        return pentagons;
    }

    private Clique GetPentagon(int a, int b, int c, int d, int e)
    {
        if (a >= b || b >= c || c >= d || d >= e) throw new ArgumentException("Invalid Pentagon");
        var pentagonNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24
          + e * (e - 1) * (e - 2) * (e - 3) * (e - 4) / 120;
        return pentagons[pentagonNo];
    }

}