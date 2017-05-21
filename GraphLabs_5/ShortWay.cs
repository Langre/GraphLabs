using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLabs_5
{
    class DirectedWeightedGraph
    {
        public int[,] adjMatrix { get; private set; }
        public Dictionary<int, HashSet<Tuple<int, int>>> adjList { get; private set; }
        public int vertecesCount { get; private set; }

        private bool[] markers;
        private Stack<int> vertStack;
        private int[] label;

        private int[] dist;
        private int[] pred;

        public DirectedWeightedGraph(string path)
        {
            this.InitGraph(path);
        }

        private void AddEdgeToMatrix(int i, int j, int w)
        {
            if (i > 0 && i <= vertecesCount && j > 0 && j <= vertecesCount)
            {
                adjMatrix[i - 1, j - 1] += w;
            }
        }

        private void AddArcToList(int v, int n, int w)
        {
            if (v >= 0 && n <= vertecesCount && v >= 0 && n <= vertecesCount)
            {
                adjList[v].Add(new Tuple<int, int>(n, w));
            }
        }

        public void InitGraph(string path)
        {
            var str = File.ReadAllText(path);
            var strA = str.Split(new char[] { ' ', '\n' }).Where(s => s != string.Empty).ToArray();
            int[] verts = Array.ConvertAll(strA, s => int.Parse(s));

            vertecesCount = verts[0];

            adjMatrix = new int[vertecesCount, vertecesCount];

            adjList = new Dictionary<int, HashSet<Tuple<int, int>>>();

            for (int v = 0; v < vertecesCount; v++)
            {
                adjList.Add(v, new HashSet<Tuple<int, int>>());
            }

            for (int v = 1; v < verts.Length; v += 3)
            {
                AddEdgeToMatrix(verts[v], verts[v + 1], verts[v + 2]);
                AddArcToList(verts[v], verts[v + 1], verts[v + 2]);
            }
        }

        public void FordWithQueue(int s)
        {
            dist = new int[vertecesCount]; // оценки длин путей
            pred = new int[vertecesCount]; // предшествующая вершина
            for (int i = 0; i < vertecesCount; i++)
            {
                dist[i] = int.MaxValue;
                pred[i] = -1;
            }

            dist[s] = 0;
            //stack<int> bag;
            Queue<int> bag = new Queue<int>();
            bag.Enqueue(s);
            //var T = new Tuple<int, int>();
            //priority_queue<T, vector<T>, greater<T> > bag;
            //bag.push(make_pair(0, s));
            while (bag.Count > 0)
            {
                //int u = bag.top(); // for stack
                int u = bag.Dequeue(); // for queue
                //T p = bag.top(); // for priority_queue
                //int u = p.second;
                //bag.pop();

                foreach (var x in adjList[u])
                {
                    int v = x.Item1, w = x.Item2;
                    if (dist[u] + w < dist[v])
                    {
                        dist[v] = dist[u] + w;
                        pred[v] = u;
                        bag.Enqueue(v);
                        //bag.push(make_pair(dist[v], v));
                    }

                }
            }
        }

        public void FordWithStack(int s)
        {
            dist = new int[vertecesCount]; // оценки длин путей
            pred = new int[vertecesCount]; // предшествующая вершина
            for (int i = 0; i < vertecesCount; i++)
            {
                dist[i] = int.MaxValue;
                pred[i] = -1;
            }

            dist[s] = 0;
            Stack<int> bag = new Stack<int>();
            bag.Push(s);

            while (bag.Count > 0)
            {
                int u = bag.Pop();

                foreach (var x in adjList[u])
                {
                    int v = x.Item1, w = x.Item2;
                    if (dist[u] + w < dist[v])
                    {
                        dist[v] = dist[u] + w;
                        pred[v] = u;
                        bag.Push(v);                        
                    }

                }
            }
        }

        public void FordWithPriorityQueue(int s)
        {
            dist = new int[vertecesCount]; // оценки длин путей
            pred = new int[vertecesCount]; // предшествующая вершина
            for (int i = 0; i < vertecesCount; i++)
            {
                dist[i] = int.MaxValue;
                pred[i] = -1;
            }

            dist[s] = 0;

            PriorityQueue<int, int> bag = new PriorityQueue<int, int>();
            bag.Enqueue(0, s);
            while (bag.Count > 0)
            {
                var p = bag.Dequeue();
                int u = p.Value;

                foreach (var x in adjList[u])
                {
                    int v = x.Item1, w = x.Item2;
                    if (dist[u] + w < dist[v])
                    {
                        dist[v] = dist[u] + w;
                        pred[v] = u;
                        bag.Enqueue(dist[v], v);
                    }
                }            
        }
    }

    public void PrintShortWay()
    {
        dist = dist.OrderBy(e => e).ToArray();
        for (int v = 0; v < vertecesCount; v++)
        {
            if (dist[v] < int.MaxValue)
            {
                Console.Write(dist[v] + " : " + v);
                for (int x = v; pred[x] >= 0; x = pred[x])
                    Console.Write("<-" + pred[x]);
                Console.WriteLine();
            }
        }
    }
    }
}
