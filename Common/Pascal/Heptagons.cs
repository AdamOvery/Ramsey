class Heptagons : ICliqueCollection
{
    private Clique[] heptagons;
    private int order;
    public int Length { get; private set; }

    public Heptagons(int order, Hexagons hexagons)
    {
        this.order = order;
        this.Length = order * (order - 1) * (order - 2) * (order - 3) * (order - 4) * (order - 5) * (order - 6) / 5040;
        heptagons = new Clique[Length]; // (10*9*8*7*6*5*4)/5040 = 252

        var idx = 0;
        for (var g = 6; g < order; g++)
        {
            for (var f = 0; f < g; f++)
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
                                    var h1 = hexagons.GetHexagon(a, b, c, d, e, f);
                                    var h2 = hexagons.GetHexagon(a, b, c, e, f, g);
                                    var h3 = hexagons.GetHexagon(a, b, d, e, f, g);
                                    var h4 = hexagons.GetHexagon(a, c, d, e, f, g);
                                    var h5 = hexagons.GetHexagon(b, c, d, e, f, g);
                                    Clique heptagon = new Clique(7, $"{a}-{b}-{c}-{d}-{e}-{f}-{g}", new Clique[] { h1, h2, h3, h4, h5 });
                                    heptagons[idx++] = heptagon;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public IEnumerable<Clique> GetCliques()
    {
        return heptagons;
    }

    Clique GetHeptagon(int a, int b, int c, int d, int e, int f, int g)
    {
        if (a >= b || b >= c || c >= d || d >= e || e >= f || f >= g) throw new ArgumentException("Invalid Heptagon");
        var heptagonNo = a
         + b * (b - 1) / 2
          + c * (c - 1) * (c - 2) / 6
          + d * (d - 1) * (d - 2) * (d - 3) / 24
          + e * (e - 1) * (e - 2) * (e - 3) * (e - 4) / 120
          + f * (f - 1) * (f - 2) * (f - 3) * (f - 4) * (f - 5) / 720
          + g * (g - 1) * (g - 2) * (g - 3) * (g - 4) * (g - 5) * (g - 6) / 5040;
        return heptagons[heptagonNo];
    }
}

