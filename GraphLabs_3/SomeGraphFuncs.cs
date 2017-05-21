using System;
using System.Collections.Generic;
using System.Linq;

using GraphLabs_2;

namespace GraphLabs_3
{
    static class SomeGraphFuncs
    {
        public static List<List<int>> ToTiersForms(DirectedGraph graph)
        {
            var bagOfVisitedVertices = new List<int>(graph.HalfDegreesIn.Where(v => v.Value == 0).Select(v => v.Key).ToList());
            var bagOfRemainVertices = new List<int>(graph.HalfDegreesIn.Where(v => v.Value > 0).Select(v => v.Key).ToList());
            var tiers = new List<List<int>>();
            tiers.Add(graph.HalfDegreesIn.Where(v => v.Value == 0).Select(v => v.Key).ToList());
            while (bagOfVisitedVertices.Count < graph.vertecesCount)
            {
                var lastTier = tiers.Last();
                tiers.Add(new List<int>());

                //foreach (var v in bagOfRemainVertices)
                int rv = 0;

                while (rv < bagOfRemainVertices.Count)
                {                    
                    bool flag = true;
                    var v = bagOfRemainVertices[rv];
                    for (int i = 0; i < graph.vertecesCount; i++)
                        if (graph.adjMatrixIns[v - 1, i] && (!bagOfVisitedVertices.Contains(i + 1)
                            || tiers.Last().Contains(i + 1)))
                            flag = false;
                    if (flag)
                    {
                        tiers.Last().Add(v);
                        bagOfVisitedVertices.Add(v);
                        bagOfRemainVertices.Remove(v);
                        rv = bagOfRemainVertices.Count > 0 ? -1 : 0;
                    }
                    rv++;
                }
            }

            return tiers;
        }

        public static dynamic GetMaxComponents(DirectedGraph graph)
        {
            var k = graph.Kasaraju();

            var comps = k.Select((v, i) => new { v, i })
                .GroupBy(e => e.v)
                .Where(e => e.Count() > 1)
                .OrderBy(e => e.Key)
                .ToList();

            foreach (var v in comps)
            {
                Console.Write(v.Key + "::");
                foreach (var vv in v)
                    Console.Write((vv.i + 1) + " ");
                Console.WriteLine();
            }

            return comps;
        }

        public static dynamic GetMetaGraph(DirectedGraph graph)
        {
            var k = graph.Kasaraju();            
            var comps = k.Select((v, i) => new { v, i })
                .GroupBy(e => e.v)
                .Where(e => e.Count() > 1)
                .OrderBy(e => e.Key)            
                .ToList();
            var metaV = comps.SelectMany(v => v.Select(e => e.i)).ToList();

            var metaAdjList = new Dictionary<int, HashSet<int>>();
            var metaAdjMatrix = new bool[comps.Count, comps.Count];

            var tempAdjL = graph.adjList.Where(v => metaV.Contains(v.Key));

            foreach (var c in comps)
            {
                metaAdjList.Add(c.Key, new HashSet<int>());
                foreach (var x in tempAdjL)
                    if (x.Value.Any(e => c.Any(j => j.i + 1 == e)))
                    {
                        var t = comps.Where(co => co.Any(coo => x.Value.Any(vvv => vvv == coo.i + 1))).FirstOrDefault();
                        metaAdjList[c.Key].Add(t.Key);
                    }                                              
            }

            foreach (var v in metaAdjList)
                if (v.Value.Contains(v.Key))
                    v.Value.Remove(v.Key);

            return metaAdjList;
        }
    }
}