using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphsLabs
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../Lab01-gr5-25.dat";
            var graph = new UndirectedGraph(path);
            
            Console.WriteLine("Матрица смежности: ");
            for (int i = 0; i < graph.adjMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < graph.adjMatrix.GetLength(1); j++)
                {
                    Console.Write(graph.adjMatrix[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Списки смежности: ");
            foreach (var v in graph.adjList.OrderBy(v => v.Key))
            {
                Console.Write(v.Key + ": ");
                foreach (var n in v.Value)
                    Console.Write(n + ", ");
                Console.WriteLine();
            }

            Console.Write("Степени вершин: ");
            var d = graph.GetDegrees();
            foreach (var v in d)
                Console.WriteLine("Вершина " + v.Key + ", чья степень = " + v.Value);

            Console.WriteLine("Висячие врешины(c соседями): ");
            var h = graph.GetHangingVertexies();
            foreach (var hv in h)
                Console.Write(hv + "(" + graph.adjList[hv].First() + ") ");
            Console.WriteLine();

            Console.WriteLine("Изолированные врешины: ");
            foreach (var i in graph.adjList.Where(v => v.Value.Count() == 0))
                Console.Write(i.Key + " ");
            Console.WriteLine();

            Console.WriteLine("Кратные врешины: ");
            var multipleEdges = graph.GetMultipleEdges();
            foreach (var me in multipleEdges)
                Console.Write(me + " ");
            Console.WriteLine();

            Console.WriteLine("Петли и их кратности: ");
            var loops = graph.GetLoops();
            foreach (var hv in h)
                Console.Write(hv + " ");
            Console.WriteLine();

            Console.WriteLine("/////// Упрощенный граф ////////////");

            var simplifiedGraph = new SimplifiedGraph(graph);

            Console.WriteLine("Списки смежности: ");
            foreach (var v in simplifiedGraph.adjList.OrderBy(v => v.Key))
            {
                Console.Write(v.Key + ": ");
                foreach (var n in v.Value)
                    Console.Write(n + ", ");
                Console.WriteLine();
            }

            Console.WriteLine("Число компонент упрощенного графа - " + simplifiedGraph.CountComponents());

            Console.WriteLine("Компоненты связности. ");
            Console.WriteLine("Поиск в глубину: ");
            var depthTree = new List<int>();
            simplifiedGraph.ClearMarks();
            simplifiedGraph.FindBindingComponentsInDepth(1, depthTree);
            foreach (var v in depthTree)
                Console.Write(v + " ");
            Console.WriteLine();
            Console.WriteLine("Поиск в ширину: ");
            simplifiedGraph.ClearMarks();
            simplifiedGraph.FindBindingComponentsInWidth(1);

            Console.ReadKey();
        }
    }
}
