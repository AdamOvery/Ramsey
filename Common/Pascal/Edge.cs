public class Edge : Clique
{
    private int n1, n2;
    static Clique[] EmptyCliqueArray = new Clique[0];

    public Edge(int n1, int n2) : base(2, $"{n1}-{n2}", EmptyCliqueArray)
    {
        this.n1 = n1;
        this.n2 = n2;
    }

    public new bool value
    {
        get { return base.value == CliqueValue.AllOn ? true : false; }
        set { base.value = value ? CliqueValue.AllOn : CliqueValue.AllOff; }
    }
}
