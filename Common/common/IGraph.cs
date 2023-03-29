
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

    void copyGraph(IGraph g1, IGraph g2)
    {
        if (g1.order <= g2.order)
        {
            g1.forEachEdge((a, b) => g2.SetEdgeValue(a, b, g1.GetEdgeValue(a, b)));
        }
        else
        {
            g2.forEachEdge((a, b) =>  g2.SetEdgeValue(a, b, g1.GetEdgeValue(a, b)));
        }
    }

    void forEachEdge(Action<int, int> action)
    {
        for (int b = 0; b < this.order; b++)
        {
            for (int a = 0; a < b; a++)
            {
                action(a, b);
            }
        }
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

