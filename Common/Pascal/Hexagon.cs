class Hexagons : ICliqueCollection
{
    private Clique[] hexagons;
    private int order;
    public int Length { get; private set; }

    public Hexagons(int order, Pentagons pentagons)
    {
        this.order = order;
        this.Length = order * (order - 1) * (order - 2) * (order - 3) * (order - 4) * (order - 5) / 720;
        hexagons = new Clique[Length]; // (10*9*8*7*6*5)/720 = 252

        var idx = 0;
        for (var f = 5; f < order; f++)
        {
            for (var e = 0; e < f; e++)
            {
                for (var d = 0; d < e; d++)
                {
                    for (var c = 0; c < d; c++)
                    {
                        for (var b = 0; b < c; b++)
                        {
                            for (var a = 0; a < b; a++)
                            {
                                var p1 = pentagons.GetPentagon(a, b, c, d, e);
                                var p2 = pentagons.GetPentagon(a, b, c, e, f);
                                var p3 = pentagons.GetPentagon(a, b, d, e, f);
                                var p4 = pentagons.GetPentagon(a, c, d, e, f);
                                var p5 = pentagons.GetPentagon(b, c, d, e, f);
                                Clique hexagon = new Clique(6, $"{a}-{b}-{c}-{d}-{e}-{f}", new Clique[] { p1, p2, p3, p4, p5 });
                                hexagons[idx++] = hexagon;
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerable<Clique> GetCliques()
    {
        return hexagons;
    }

    public Clique GetHexagon(int a, int b, int c, int d, int e, int f)
    {
        if (a >= b || b >= c || c >= d || d >= e || e >= f) throw new ArgumentException("Invalid Hexagon");
        var hexagonNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24
          + e * (e - 1) * (e - 2) * (e - 3) * (e - 4) / 120
          + f * (f - 1) * (f - 2) * (f - 3) * (f - 4) * (f - 5) / 720;
        return hexagons[hexagonNo];
    }
}
