class Pair
{
    int a;
    int b;
    public Pair(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    static Pair fromEdgeNo(int edgeNo)
    {
        int b = (int)Math.Sqrt(1 + 8 * edgeNo);
        int a = edgeNo - (b * (b - 1) / 2);

        return new Pair(a, b);
    }

}
