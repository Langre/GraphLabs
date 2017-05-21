using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLabs_2
{
    public enum States { New, Active, Done }
    public class DirectedGraph
    {
        public int[,] adjMatrix { get; private set; }
        public Dictionary<int, HashSet<int>> adjList { get; private set; }
        public Dictionary<int, HashSet<int>> revAdjList { get; private set; }
        public int vertecesCount { get; private set; }

        private bool[] markers;
        private Stack<int> vertStack;
        private int[] label;
        private States[] states;

        public bool[,] adjMatrixIns { get; private set; }

        private Dictionary<int, HashSet<int>> asycTempAdjList;

        public Dictionary<int, int> HalfDegreesOut { get; private set; }
        public Dictionary<int, int> HalfDegreesIn { get; private set; }

        public DirectedGraph() { }
        public DirectedGraph(int[,] adjMatrix,
                             Dictionary<int, HashSet<int>> adjList)
        {
            this.adjMatrix = adjMatrix;
            this.adjList = adjList;
            this.vertecesCount = adjList.Count;
        }

        private void AddEdgeToMatrix(int i, int j)
        {
            if (i > 0 && i <= vertecesCount && j > 0 && j <= vertecesCount)
            {
                adjMatrix[i - 1, j - 1] += 1;
                adjMatrixIns[j - 1, i - 1] = true;
            }
        }

        private void AddArcToList(int v, int n)
        {
            if (v > 0 && n <= vertecesCount && v > 0 && n <= vertecesCount)
            {
                adjList[v].Add(n);
                revAdjList[n].Add(v);
            }
        }

        public void InitGraph(string path)
        {
            var str = File.ReadAllText(path);
            var strA = str.Split(new char[] { ' ', '\n' }).Where(s => s != string.Empty).ToArray();
            int[] verts = Array.ConvertAll(strA, s => int.Parse(s));

            vertecesCount = verts[0];            

            adjMatrix = new int[vertecesCount, vertecesCount];
            adjMatrixIns = new bool[vertecesCount, vertecesCount];

            adjList = new Dictionary<int, HashSet<int>>();
            revAdjList = new Dictionary<int, HashSet<int>>();

            for (int v = 1; v <= vertecesCount; v++)
            {
                adjList.Add(v, new HashSet<int>());
                revAdjList.Add(v, new HashSet<int>());
            }

            for (int v = 1; v < verts.Length; v += 2)
            {
                AddEdgeToMatrix(verts[v], verts[v + 1]);
                AddArcToList(verts[v], verts[v + 1]);
            }

            GetHalfDegreesIn();
            GetHalfDegreesOut();
        }

        public void GetHalfDegreesOut()
        {
            HalfDegreesOut = new Dictionary<int, int>();

            foreach (var v in adjList)
                HalfDegreesOut.Add(v.Key, v.Value.Count);      
        }

        public void GetHalfDegreesIn()
        {
            HalfDegreesIn = new Dictionary<int, int>();

            for (int i = 0; i < vertecesCount; i++)
            {
                HalfDegreesIn.Add(i + 1, 0);
                var t = adjList.Where(v => v.Key != (i + 1));
                foreach (var n in t)
                    if (n.Value.Contains(i + 1))
                        HalfDegreesIn[i + 1]++;
            }
        }

        public List<int> GetSources()
        {
            GetHalfDegreesIn();
            GetHalfDegreesOut();

            return HalfDegreesOut.Where(v => !HalfDegreesIn.ContainsKey(v.Key)).Select(v => v.Key).ToList();
        }

        public List<int> GetStokes()
        {
            return HalfDegreesIn.Where(v => !HalfDegreesOut.ContainsKey(v.Key)).Select(v => v.Key).ToList();
        }

        public void ClearMarks()
        {
            this.markers = new bool[vertecesCount];
        }

        public void RevDFS(int cur)
        {
            markers[cur - 1] = true;
            foreach (var n in revAdjList[cur])
                if (!markers[n - 1])
                    RevDFS(n);
            vertStack.Push(cur);
        }

        public void LabelDFS(int cur, int count)
        {
            markers[cur - 1] = true;
            label[cur - 1] = count;
            foreach (var n in adjList[cur])
                if (!markers[n - 1])
                    LabelDFS(n, count);
        }        

        public List<int> Traverse(int v, bool r = true)
        {
            ClearMarks();
            var bag = new Stack<Tuple<int, int>>();
            bag.Push(new Tuple<int, int>(0, v));
            var parent = new int[vertecesCount];
            var reach = new List<int>();
            var adj = r ? adjList : revAdjList;
            Tuple<int, int> p = new Tuple<int, int>(0, 0);
            while (bag.Count > 0)
            {
                p = bag.Pop();
                if (markers[p.Item2 - 1])
                    continue;
                markers[p.Item2 - 1] = true;
                parent[p.Item2 - 1] = p.Item1;
                reach.Add(p.Item2);
                foreach (var n in adj[p.Item2])
                {
                    if (markers[n - 1])
                        continue;
                    bag.Push(new Tuple<int, int>(p.Item2, n));
                }  
            }

            return reach;
        }

        public List<int> GetComponent(int v)
        {
            return Traverse(v).Intersect(Traverse(v, false)).ToList();
        }

        public List<int> Kasaraju()
        {
            vertStack = new Stack<int>();
            label = new int[vertecesCount];
            ClearMarks();
            for (int v = 1; v <= vertecesCount; v++)
                if (!markers[v - 1])
                    RevDFS(v);

            ClearMarks();
            for (int count = 0; vertStack.Count > 0;)
            {
                int v = vertStack.Pop();
                if (!markers[v - 1])
                    LabelDFS(v, ++count);
            }
            return label.ToList();          
        }

        private bool IsAcyclicDFS(int v)
        {
            states[v - 1] = States.Active;

            foreach(var n in asycTempAdjList[v])
            { 
                if(states[n - 1] == States.Active)
                        return false;
                if (states[n - 1] == States.New)
                    if (IsAcyclicDFS(n) == false)
                            return false;
            }
            states[v - 1] = States.Done;
            return true;
        }

        public bool IsAcyclic()
        {
            asycTempAdjList = new Dictionary<int, HashSet<int>>(adjList);
            asycTempAdjList.Add(asycTempAdjList.Last().Key + 1, new HashSet<int>());
            for (int i = 1; i <= vertecesCount; i++)
                adjList.Last().Value.Add(i);

            states = new States[vertecesCount + 1];

            return IsAcyclicDFS(asycTempAdjList.Last().Key);
        }

        public List<int> TopologySort()
        {
            if (IsAcyclic())
            {
                var topologyList = new List<int>();
                states = new States[vertecesCount];
                while(true)
                {
                    bool flag = true;
                    for (int i = 1; i <= vertecesCount; i++)
                    {
                        if (states[i - 1] == States.Done)
                            continue;
                        if (HalfDegreesIn[i] > 0)
                            continue;
                        flag = false;
                        topologyList.Add(i);
                        states[i - 1] = States.Done;
                        foreach (var n in adjList[i])
                            if (states[n - 1] == States.New)
                                HalfDegreesIn[n]--;
                    }
                    if (flag)
                        break;
                }
                return topologyList;
            }
            return null;
        }
    }
}
