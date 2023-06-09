﻿using Pascal;

namespace Ramsey.Adam.RamseyLibrary
{
    public class Solution
    {
        public int[]? EdgeLinks { get; }
        public bool[,] Edges { get; }
        public string? G6Code { get; }

        public Solution(int[] edgeLinks, bool[,] edges)
        {
            EdgeLinks = edgeLinks.Clone() as int[] ?? Array.Empty<int>();
            Edges = edges.Clone() as bool[,] ?? new bool[0, 0];
        }

        public Solution(MatrixGraphAdam graph)
        {
            Edges = graph.edges.Clone() as bool[,] ?? new bool[0, 0];
            G6Code = G6.fromGraph(graph);
        }

        public string EdgeLinkDescription
        {
            get
            {
                var result = EdgeLinks is not null ? string.Join(",", EdgeLinks) : string.Empty;
                return result;
            }
        }

        public string EdgeDescription
        {
            get
            {
                var results = string.Empty;
                for (var node1 = 0; node1 < Edges.GetLength(0); node1++)
                {
                    var result = node1.ToString() + ":";

                    for (var node2 = 0; node2 < Edges.GetLength(1); node2++)
                    {
                        if (Edges[node1, node2])
                        {
                            result += " " + node2.ToString();
                        }
                    }

                    results += result + Environment.NewLine;
                }

                return results;
            }
        }

    }
}
