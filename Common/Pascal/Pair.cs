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
        // gosh this is ugly.  
        // I'm sure there's a better way to do this. 
       

        int b = (int)Math.Sqrt(1 + 8 * edgeNo);
        int a = edgeNo - (b * (b - 1) / 2);

        return new Pair(a, b);
    }

}
