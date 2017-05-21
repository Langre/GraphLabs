using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsLabs
{
    public class SimplifiedGraph
    {
        //private UndirectedGraph simpleGraph;
        public int[,] adjMatrix { get; private set; }
        public Dictionary<int, HashSet<int>> adjList { get; private set; }
        public bool[] markers { get; private set; }   
        public int[] label { get; private set; }
        public int vertecesCount { get; private set; }

        public SimplifiedGraph(int n = 0)
        {
            this.vertecesCount = n;
            this.adjMatrix = new int[n, n];
            this.adjList = new Dictionary<int, HashSet<int>>();
            for (int i = 0; i < vertecesCount; i++)
                this.adjList.Add(i + 1, new HashSet<int>());
            label = new int[vertecesCount];
            this.markers = new bool[vertecesCount];
            //this.simpleGraph = new UndirectedGraph();
        }
        public SimplifiedGraph(UndirectedGraph graph)
        {
            if (graph.vertecesCount != 0)
            {
                this.MakeGraphSimple(graph);
                label = new int[vertecesCount];
                this.markers = new bool[vertecesCount];
            }
        }

        public void AddEdge(int v, int n)
        {
            if ((v < 1) || (v > vertecesCount)) return;
            if ((n < 1) || (n > vertecesCount)) return;
            
            foreach (var x in adjList[v])
                if (x == n) return;
            adjList[v].Add(n);
            adjList[n].Add(v);
        }
        private void MakeGraphSimple(UndirectedGraph graph)
        {
            //this.simpleGraph = new UndirectedGraph(graph.adjMatrix);
            this.vertecesCount = graph.vertecesCount;
            this.adjList = new Dictionary<int, HashSet<int>>();
            this.adjMatrix = new int[this.vertecesCount, this.vertecesCount];


            for (int i = 0; i < vertecesCount; i++)
                for (int j = 0; j < vertecesCount; j++)
                {
                    if (i == j && graph.adjMatrix[i, j] > 0)
                        adjMatrix[i, j] = 0;
                    if (i != j && graph.adjMatrix[i, j] > 1)
                        adjMatrix[i, j] = 1; 
                }

            for (int i = 0; i < vertecesCount; i++)
            {
                if (!adjList.ContainsKey(i + 1))
                    adjList.Add(i + 1, new HashSet<int>());
                for (int j = 0; j < vertecesCount; j++)
                    if (graph.adjMatrix[i, j] != 0)
                        adjList[i + 1].Add(j + 1);                
            }                                
        }

        public void ClearMarks()
        {
            this.markers = new bool[vertecesCount];
        }

        public int CountComponents()
        {
            this.ClearMarks();
            int count = 0; 
            for(int i = 0; i < vertecesCount; i++)
            {
                if (markers[i])
                    continue;
                count++;
                this.FindBindingComponentsInDepth(i + 1);
            }

            return count;
        }

        public void FindBindingComponentsInDepth(int cur, List<int> verteces = null)
        {
            if (markers[cur - 1])
                return;
            markers[cur - 1] = true;
            if (verteces != null)
                verteces.Add(cur);
            foreach (var n in adjList[cur])
                FindBindingComponentsInDepth(n, verteces);
        }

        public void FindBindingComponentsInWidth(int s)
        {
            Console.Write(s + " ");
            Queue<int> q = new Queue<int>();
            q.Enqueue(s);
            //int[] d = new int[simpleGraph.vertecesCount];
            //int[] p = new int[simpleGraph.vertecesCount];
            markers[s - 1] = true;
            //p[s] = -1;
            var adjList = this.adjList;
            while (q.Count() != 0)
            {
                int v = q.Dequeue();
                for (int i = 0; i < adjList[v].Count(); i++)
                {
                    int to = adjList[v].ElementAt(i);
                    if (!markers[to - 1])
                    {
                        markers[to - 1] = true;
                        q.Enqueue(to);
                        //d[to] = d[v - 1] + 1;
                        //p[to] = v;

                        Console.Write(to + " ");
                    }
                }
            }
        }       

        public List<int> GetMaxComponent()
        {
            List<List<int>> components = new List<List<int>>();

            for (int i = 1; i <= vertecesCount; i++)
            {
                components.Add(new List<int>());
                FindBindingComponentsInDepth(i, components[i - 1]);
            }

            return components.OrderByDescending(c => c.Count).FirstOrDefault();
        }

        private void LabelComponent(int v, int count)
        {
            markers[v - 1] = true;
            label[v - 1] = count;

            foreach (var n in adjList[v])
                if (!markers[n - 1])
                    LabelComponent(n, count);
        }

        public int CountAndLabel(List<int> maxC)
        {            
            int count = 0;
            //label = new int[simpleGraph.vertecesCount];
            ClearMarks();

            for (int i = 1; i <= vertecesCount; i++)
                if (!markers[i - 1] && maxC.Contains(i))
                    LabelComponent(i, count++);
            return count;
        }

        public void PrintAdjLists()
        {
            foreach (var v in this.adjList.OrderBy(v => v.Key))
            {
                Console.Write(v.Key + ": ");
                foreach (var n in v.Value)
                    Console.Write(n + ", ");
                Console.WriteLine();
            }
        }
    }
}
