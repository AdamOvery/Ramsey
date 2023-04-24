using Pascal;
using static Pascal.TestEngine;

class SymmetricGraph : IGraph
{
    public int order { get; private set; }

    private bool[] deltas;

    public SymmetricGraph(int order)
    {
        this.order = order;
        deltas = new bool[order];
    }

    public event EdgeChangedHandler EdgeChanged = delegate { };
    public event GraphChangedHandler GraphChanged = delegate { };


    public void Clear()
    {
        deltas = new bool[order];
        GraphChanged.Invoke(this);
    }

    public bool GetEdgeValue(int n1, int n2)
    {
        int idx = (order + n2 - n1) % order;
        return deltas[idx];
    }

    public void SetEdgeValue(int n1, int n2, bool value)
    {
        int idx1 = (order + n2 - n1) % order;
        int idx2 = (order + n1 - n2) % order;
        deltas[idx1] = value;
        deltas[idx2] = value;
        
         for (int a = 0; a < order; a++)
         {
             int b = (a + idx1) % order;
             if (a < b) EdgeChanged.Invoke(this, a, b, value);
             else EdgeChanged.Invoke(this, b, a, value);
         }
    }


}
