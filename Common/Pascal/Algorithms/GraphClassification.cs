using System.Text.RegularExpressions;

namespace Pascal;


public class GraphClassification
{
    static Regex nodeIdRegex = new Regex(@"#\d+:", RegexOptions.Multiline);

    public static string getNodeSignature(string? signatureWithNodeIds)
    {
        return nodeIdRegex.Replace(signatureWithNodeIds ?? "", "");
    }

    class NodeSignatureComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return getNodeSignature(x).CompareTo(getNodeSignature(y));
        }

        public static NodeSignatureComparer instance = new NodeSignatureComparer();
    }

    private static string GetSignatureWithNodeIds(IGraph g, INode n1)
    {

        var getName = new Func<INode, int, String>((start, depth) => { return ""; });

        int[] visited = new int[g.order];
        getName = (INode start, int depth) =>
        {
            if (visited[start.id] > 0) return "#" + start.id + ":" + (visited[start.id] - depth);
            else
            {
                visited[start.id] = depth;
                var adjacentNodeStrings = start.adjacentNodes.Select(n =>
                {
                    if (n == n1) return "#" + n.id + ":0";
                    else return getName(n, depth + 1);
                }).ToArray();
                Array.Sort(adjacentNodeStrings, NodeSignatureComparer.instance);
                var result = String.Join(',', adjacentNodeStrings);
                visited[start.id] = 0;
                return "#" + start.id + ":[" + result + "]";
            }
        };
        // getName(n1, 1)
        var fullName = getName(n1, 1);
        return fullName;
    }

    public static void Tests()
    {

        // https://ramsey-paganaye.vercel.app/pascal/1?g6=K~{???A????S
        // var g = (G6.parse("Fg???").AsSubGraph())!;
        var g = (G6.parse("K~{???A????S").AsSubGraph())!;


        foreach (var n1 in g.nodes)
        {
            var signatureWithNodeIds = GetSignatureWithNodeIds(g.graph, n1);
            var signature = nodeIdRegex.Replace(signatureWithNodeIds, "");
            Console.WriteLine("#" + n1.id + " full: " + signatureWithNodeIds);
            Console.WriteLine("#" + n1.id + " abbr: " + signature);
        }

        //     {
        //         g.nodes

        //         //var n = g.nodes[i] as NamedNode;
        //         //n.name.Add(new List<int>());
        //         //n.name[pass-1].Add(i);
        //     }
        //     if (pass == 1) n

    }

}

