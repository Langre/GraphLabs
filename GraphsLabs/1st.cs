using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GraphsLabs
{
    public class UndirectedGraph
    {
        public int[,] adjMatrix { get; private set; }
        public Dictionary<int, HashSet<int>> adjList { get; private set; }
        Dictionary<int, int> degrees;

        public int vertecesCount { get; private set; }

        public UndirectedGraph(int vertecesCount = 0)
        {
            this.adjList = new Dictionary<int, HashSet<int>>();
            this.vertecesCount = vertecesCount;
            this.adjMatrix = new int[vertecesCount, vertecesCount];

            for (int i = 0; i < vertecesCount; i++)
                this.adjList.Add(i + 1, new HashSet<int>());
        }

        public UndirectedGraph(string path) { this.InitGraph(path); }

        public UndirectedGraph(int[,] adjMatrix)
        {
            vertecesCount = adjMatrix.GetLength(0);
            this.adjMatrix = new int[vertecesCount, vertecesCount];
            for (int i = 0; i < vertecesCount; i++)
                for (int j = 0; j < vertecesCount; j++)
                    this.adjMatrix[i, j] = adjMatrix[i, j];

            this.adjList = new Dictionary<int, HashSet<int>>();
        }

        private void AddEdge(int i, int j)
        {
            if (i > 0 && i <= vertecesCount && j > 0 && j <= vertecesCount)
            {
                adjMatrix[i - 1, j - 1]++;
                if (i != j)
                    adjMatrix[j - 1, i - 1]++;
            }
        }

        public void AddArc(int i, int j)
        {
            if (!adjList.ContainsKey(i))
            {
                adjList.Add(i, new HashSet<int>());
                adjList[i].Add(j);
            }
            else
                adjList[i].Add(j);

            if (!adjList.ContainsKey(j))
            {
                adjList.Add(j, new HashSet<int>());
                adjList[j].Add(i);
            }
            else
                adjList[j].Add(i);
        }

        public Dictionary<int, int> GetDegrees()
        {
            degrees = new Dictionary<int, int>();

            for (int i = 0; i < vertecesCount; i++)
            {
                if (!degrees.ContainsKey(i + 1))
                    degrees.Add(i + 1, 0);

                for (int j = 0; j < vertecesCount; j++)
                {
                    degrees[i + 1] += adjMatrix[i, j];
                    if (i == j && adjMatrix[i, j] != 0)
                        degrees[i + 1]++;
                }
            }
            Console.WriteLine();


            return degrees;
        }

        public int[] GetHangingVertexies()
        {
            int[] hanging = adjList.Where(v => v.Value.Where(n => n != v.Key).Count() == 1).Select(v => v.Key).ToArray();
            return hanging;
        }

        public Dictionary<int, int> GetLoops()
        {
            var loops = new Dictionary<int, int>();
            for (int i = 0; i < vertecesCount; i++)
                if (adjMatrix[i, i] > 0)
                    loops.Add(i + 1, adjMatrix[i, i]);
            return loops;
        }

        public Dictionary<Tuple<int, int>, int> GetMultipleEdges()
        {
            var multE = new Dictionary<Tuple<int, int>, int>();
            for(int i = 0; i < vertecesCount; i++)
                for (int j= 0; j < vertecesCount; j++)
                {
                    if (adjMatrix[i, j] > 1 && !multE.Keys.Contains(new Tuple<int, int>(j + 1, i + 1)))
                        multE.Add(new Tuple<int, int>(i + 1, j + 1), adjMatrix[i, j]);
                }
            return multE;
        }

        private void InitGraph(string path)
        {
            var str = File.ReadAllText(path);
            var strA = str.Split(new char[] { ' ', '\n' }).Where(s => s != string.Empty).ToArray();
            int[] verts = Array.ConvertAll(strA, s => int.Parse(s));

            vertecesCount = verts[0];

            var step = (verts.Length - 1) % 3 == 0 ? 3 : 2;

            adjMatrix = new int[vertecesCount, vertecesCount];

            adjList = new Dictionary<int, HashSet<int>>();
            for (int v = 1; v <= vertecesCount; v++)
                adjList.Add(v, new HashSet<int>());

            for (int v = 1; v < verts.Length; v += step)
            {
                AddEdge(verts[v], verts[v + 1]);
                AddArc(verts[v], verts[v + 1]);                
            }            
        }
    }
}

