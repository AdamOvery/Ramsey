
public delegate void EdgeChangedHandler(IGraph sender, int n1, int n2, bool value);

public delegate void GraphChangedHandler(IGraph sender);

public interface IGraph
{
    int order { get; }
    bool GetEdgeValue(int n1, int n2);
    void SetEdgeValue(int n1, int n2, bool value);

    void Clear();

    event EdgeChangedHandler EdgeChanged;
    event GraphChangedHandler GraphChanged;

    public void CopyGraph(IGraph g1, IGraph g2)
    {
        if (g1.order <= g2.order)
        {
            g1.ForEachEdge((a, b) => g2.SetEdgeValue(a, b, g1.GetEdgeValue(a, b)));
        }
        else
        {
            g2.ForEachEdge((a, b) => g2.SetEdgeValue(a, b, g1.GetEdgeValue(a, b)));
        }
    }

    public void ForEachEdge(Action<int, int> action)
    {
        for (int b = 0; b < this.order; b++)
        {
            for (int a = 0; a < b; a++)
            {
                action(a, b);
            }
        }
    }

    private void forEachConfigurationEdge(int a0, int b0, Action action)
    {
        var a1 = a0 + 1;
        var b1 = b0;
        if (a1 >= b0)
        {
            a1 = 0;
            b1 += 1;
        }
        if (b1 == order)
        {
            SetEdgeValue(a0, b0, false);
            action();
            SetEdgeValue(a0, b0, true);
            action();
        }
        else
        {
            SetEdgeValue(a0, b0, false);
            forEachConfigurationEdge(a1, b1, action);
            SetEdgeValue(a0, b0, true);
            forEachConfigurationEdge(a1, b1, action);
        }
    }

    public void forEachConfiguration(Action action)
    {
        forEachConfigurationEdge(0, 1, action);
    }

}

public interface IGraphFactory
{
    IGraph newGraph(int order);
}

// IGraph cloneGraph(IGraph g1, IGraphFactory factory)
// {
//     IGraph g2 = factory.newGraph(g1.order);
//     copyGraph(g1, g2);
//     return g2;
// }

//void shuffleGraph(Graph graph) {
//   IGraph clone = cloneGraph(graph);
//   var newOrder = [...Array(graph.order).keys()];
//   for (let i = 0; i < 10; i++) newOrder = newOrder.sort((a, b) => 0.5 - Math.random());

//   forEachEdge(graph, (a, b) =>
//   {
//       graph.setEdgeValue(newOrder[a], newOrder[b], clone.getEdgeValue(a, b));
//   });
//}

