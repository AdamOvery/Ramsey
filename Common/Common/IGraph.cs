
using System.Collections.Immutable;
using System.Text;
using Pascal;

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

    public void ForEachActiveEdge(Action<int, int> action)
    {
        for (int b = 0; b < this.order; b++)
        {
            for (int a = 0; a < b; a++)
            {
                if (this.GetEdgeValue(a, b)) action(a, b);
            }
        }
    }

    public void ForEachConfiguration(Action action)
    {
        Action<int, int> forEachEdge = (a0, b0) => { };
        forEachEdge = (a0, b0) =>
        {
            var a1 = a0 + 1;
            var b1 = b0;
            if (a1 >= b0)
            {
                a1 = 0;
                b1 += 1;
            }
            SetEdgeValue(a0, b0, false);
            if (b1 == order) action(); else forEachEdge(a1, b1);
            SetEdgeValue(a0, b0, true);
            if (b1 == order) action(); else forEachEdge(a1, b1);
        };
        forEachEdge(0, 1);
    }

    public void ForEachGrayConfiguration(Action action)
    {
        int order = this.order;

        Action<int, int> forEachEdge = (a0, b0) => { };
        forEachEdge = (a0, b0) =>
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
                this.SetEdgeValue(a0, b0, !this.GetEdgeValue(a0, b0));
                action();
            }
            else
            {
                forEachEdge(a1, b1);
                this.SetEdgeValue(a0, b0, !this.GetEdgeValue(a0, b0));
                action();
                forEachEdge(a1, b1);
            }
        };
        forEachEdge(0, 1);
    }

    public string ToString(string title = "Graph")
    {
        var output = new StringBuilder();
        output.AppendLine(title + " " + G6.fromGraph(this));
        for (int i = 0; i < this.order; i++)
        {
            var adj = new List<int>();
            for (int j = 0; j < this.order; j++)
            {
                if (i != j && this.GetEdgeValue(i, j)) adj.Add(j);
            }
            output.AppendLine($"N{i}: {adj.Count} [{String.Join(",", adj.Select(n => "N" + n))}]");
        }
        return output.ToString();
    }
}

public delegate void OnNodeIdVisited(int visitedNode, int? parentNode);
