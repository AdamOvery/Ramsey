using System.Text;

namespace Pascal;

class EdgeBits
{


    public static String fromGraph(IGraph g)
    {
        var result = new StringBuilder();

        g.ForEachEdge((a, b) =>
        {
            result.Append(g.GetEdgeValue(a, b) ? "1" : "0");
        });
        return result.ToString();
    }
}

