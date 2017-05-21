using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using GraphsLabs;

namespace GraphLabs_4
{
    class WeightedGraph
    { // взвешенный граф
        public Dictionary<int, HashSet<Tuple<int, int>>> AdjLists { get; private set; } // списки смежности
        private bool[] markers; // маркеры вершин
        public int[] label { get; private set; } // принадлежность вершины компоненте связности
        SimplifiedGraph F; // промежуточный лес
        int n; // кол-во вершин
        int m; // кол-во ребер
        //string path;
        List<int> maxComponent;

        private void AddEdge(int i, int j, int w)
        {
            if (i > 0 && i <= n && j > 0 && j <= n)
            {
                AdjLists[i].Add(new Tuple<int, int>(j, w));
                AdjLists[j].Add(new Tuple<int, int>(i, w));
            }
        }

        public WeightedGraph(string path)
        {
            //this.path = path;

            F = new SimplifiedGraph(new UndirectedGraph(path));
            maxComponent = F.GetMaxComponent();

            //this.n = maxComponent.Count;

            var str = File.ReadAllText(path);
            var strA = str.Split(new char[] { ' ', '\n' }).Where(s => s != string.Empty).ToArray();
            int[] verts = Array.ConvertAll(strA, s => int.Parse(s));

            n = verts[0];

            //F = new SimplifiedGraph(n);

            //adjMatrix = new int[vertecesCount, vertecesCount];
            //adjMatrixIns = new bool[vertecesCount, vertecesCount];

            AdjLists = new Dictionary<int, HashSet<Tuple<int, int>>>();
            for (int i = 1; i <= n; i++)
                AdjLists.Add(i, new HashSet<Tuple<int, int>>());

            for (int v = 1; v < verts.Length; v += 3)
            {
                //if (maxComponent.Contains(verts[v]) && maxComponent.Contains(verts[v + 1]))
                    this.AddEdge(verts[v], verts[v + 1], verts[v + 2]);
            }

            F = new SimplifiedGraph(this.n);
        }

        void AddAllSafeEdges(int count)
        {
            List<Tuple<int, int, int>> S = new List<Tuple<int, int, int>>();
            for (int i = 0; i < count; i++)
            {
                S.Add(new Tuple<int, int, int>(0, 0, 9999));
            }

            //for (int u = 1; u <= n; u++)
            foreach (var u in AdjLists.Keys)
                foreach (var v in AdjLists[u])
                    if (maxComponent.Contains(u) && maxComponent.Contains(v.Item1) && maxComponent.Contains(v.Item2))
                    {
                        if (label[u - 1] != label[v.Item1 - 1])
                        {
                            if (v.Item2 < (S[label[u - 1]]).Item3)
                                S[label[u - 1]] = new Tuple<int, int, int>(u, v.Item1, v.Item2);
                            if (v.Item2 < (S[label[v.Item1 - 1]].Item3))
                                S[label[v.Item1 - 1]] = new Tuple<int, int, int>(u, v.Item1, v.Item2);
                        }
                    }

            for (int i = 0; i < count; i++)
                if (S[i].Item3 != 1000)
                    F.AddEdge(S[i].Item1, S[i].Item2);
            F.PrintAdjLists();
            Console.WriteLine("----------------");
        }

        public void Borvka()
        {
            this.label = new int[n];
            
            int count = F.CountAndLabel(maxComponent);
            while (count > 1)
            {
                label = F.label;
                AddAllSafeEdges(count);
                count = F.CountAndLabel(maxComponent);
                Console.WriteLine("COUNT: " + count);
                Console.WriteLine("----------------");
            }
        }

        public void PrimaYarnik()
        {
            List<Tuple<int, int, int>> MST = new List<Tuple<int, int, int>>();
            //неиспользованные ребра
            List<Tuple<int, int, int>> notUsedE = new List<Tuple<int, int, int>>();
            foreach (var v in this.AdjLists)
                if (maxComponent.Contains(v.Key))
                    foreach (var n in v.Value)
                        if (maxComponent.Contains(n.Item1))
                            notUsedE.Add(new Tuple<int, int, int>(v.Key, n.Item1, n.Item2));
            //использованные вершины
            List<int> usedV = new List<int>();
            //неиспользованные вершины
            List<int> notUsedV = new List<int>(maxComponent);
            //for (int i = 1; i <= n; i++)
            //    notUsedV.Add(i);
            //выбираем случайную начальную вершину
            Random rand = new Random();
            usedV.Add(maxComponent[rand.Next(0, maxComponent.Count - 1)]);
            notUsedV.Remove(usedV[0]);            
            while (notUsedV.Count > 0)
            {
                Tuple<int, int, int> minE = new Tuple<int, int, int> (usedV.Last(), 0 , 1000); //наименьшее ребро
                //поиск наименьшего ребра
                //for (int i = 0; i < notUsedE.Count; i++)
                //{

                var t = this.AdjLists[minE.Item1].Where(e => !usedV.Contains(e.Item1)).OrderBy(e => e.Item2).FirstOrDefault();
                if (t == null)
                {
                    int k = 0;                    
                    while (t == null)
                    {
                        k++;
                        var prevV = usedV[usedV.Count - k];
                        var elements = AdjLists[prevV]
                            .Where(e => !usedV.Contains(e.Item1))
                            .OrderBy(e => e.Item2)
                            .ToList();

                        t = elements.Count() > 0 ? elements.First() : null;
                        //Console.WriteLine("--" + usedV[usedV.Count - k]);                        
                    }
                    //foreach (var v in AdjLists[usedV[usedV.Count - k]].Where(e => !usedV.Contains(e.Item1)).OrderBy(e => e.Item2))
                    //   Console.WriteLine(v + "; ");
                    //Console.WriteLine(usedV[usedV.Count - k] + " " + t.Item1 + " " + t.Item2);
                    minE = new Tuple<int, int, int>(usedV[usedV.Count - k], 0, 0);
                }
                minE = new Tuple<int, int, int>(minE.Item1, t.Item1, t.Item2);
                //}
                //заносим новую вершину в список использованных и удаляем ее из списка неиспользованных
                //if (usedV.IndexOf(notUsedE[minE].Item1) != -1)
                //{
                usedV.Add(minE.Item2);
                notUsedV.Remove(minE.Item2);
                //}
                //else
                //{
                //    usedV.Add(notUsedE[minE].Item1);
                //    notUsedV.Remove(notUsedE[minE].Item1);
                //}
                //заносим новое ребро в дерево и удаляем его из списка неиспользованных
                MST.Add(minE);                
                //notUsedE.RemoveAt(minE);
            }

            foreach (var m in MST)
            {
                Console.Write(m + " ");
                Console.WriteLine();
            }
        }

        public void Kraskal()
        { 
            List<Tuple<int, int, int>> result = new List<Tuple<int, int, int>>();
            List<Tuple<int, int, int>> edges = new List<Tuple<int, int, int>>();
            foreach (var v in this.AdjLists)
                if (maxComponent.Contains(v.Key))
                    foreach (var n in v.Value)
                        if (maxComponent.Contains(n.Item1))
                            edges.Add(new Tuple<int, int, int>(v.Key, n.Item1, n.Item2));
            edges = edges.OrderBy(e => e.Item3).ToList();

            int m = edges.Count();
            List<int> treeID = new List<int>();
            for (int i = 1; i <= n; i++)
                treeID.Add(i);
            int cost = 0;

            for (int i = 0; i < m; i++)
            {
                int a = edges[i].Item1;
                int b = edges[i].Item2;
                int l = edges[i].Item3;
                if (treeID[a - 1] != treeID[b - 1])
                {
                    cost += l;
                    result.Add(new Tuple<int, int, int>(a, b, l));
                    int oldID = treeID[a - 1];
                    int newID = treeID[b - 1];
                    for (int j = 0; j < n; j++)
                        if (treeID[j] == oldID)
                            treeID[j] = newID;
                }
            }
            foreach (var v in result)
            {
                Console.Write("(" + v.Item1 + " " + v.Item2 + " ~ " + v.Item3 + "); ");
                Console.WriteLine();
            }
        }
    }   
}
