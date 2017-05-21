using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLabs_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new DirectedGraph();
            string path = "../../../Lab01-gr5-25.dat";
            graph.InitGraph(path);

            //Console.WriteLine("Матрица смежности: ");
            //for (int i = 0; i < graph.adjMatrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < graph.adjMatrix.GetLength(1); j++)
            //    {
            //        Console.Write(graph.adjMatrix[i, j]);
            //    }
            //    Console.WriteLine();
            //}

            Console.WriteLine("Списки смежности орграфа: ");
            foreach (var v in graph.adjList.OrderBy(v => v.Key))
            {
                Console.Write(v.Key + ": ");
                foreach (var n in v.Value)
                    Console.Write(n + ", ");
                Console.WriteLine();
            }

            Console.WriteLine("Полустепени захода: ");
            graph.GetHalfDegreesIn();
            foreach (var v in graph.HalfDegreesIn)
            {
                Console.Write(v.Key + " имеет " + v.Value + " степеней захода;");
                Console.WriteLine();
            }

            Console.WriteLine("Полустепени исхода: ");
            graph.GetHalfDegreesOut();
            foreach (var v in graph.HalfDegreesOut)
            {
                Console.Write(v.Key + " имеет " + v.Value + " степеней исхода;");
                Console.WriteLine();
            }

            //Console.WriteLine("Инверсированные списки смежности: ");
            //foreach (var v in graph.revAdjList.OrderBy(v => v.Key))
            //{
            //    Console.Write(v.Key + ": ");
            //    foreach (var n in v.Value)
            //        Console.Write(n + ", ");
            //    Console.WriteLine();
            //}

                Console.WriteLine("Cписки достижимости для каждой вершины: ");
            for (int v = 1; v <= graph.vertecesCount; v++)
            {
                var t = graph.Traverse(v);
                Console.Write(v + " : ");
                foreach (var n in t)
                    if (n != v)
                        Console.Write(n + " ");
                Console.WriteLine();
            }

            Console.Write("Число компонент сильной связности = ");
            var c = graph.Kasaraju();
            Console.Write(c.Select((v, i) => new { v,i }).GroupBy(v => v.v).Where(e => e.Count() > 1).Count());

            Console.WriteLine();
            Console.WriteLine("Ацикличен ли граф? " + graph.IsAcyclic());

            Console.WriteLine("//// Ациклический граф ////");              
                    
            var asGraph = new DirectedGraph();
            asGraph.InitGraph(@"../../../Lab02-gr5-25.dat");
            Console.WriteLine("Проверка на ацикличность дала результат: " + asGraph.IsAcyclic());
            Console.WriteLine("Топологическая фортировка: ");
            var ts = asGraph.TopologySort();
            foreach (var v in ts)
                Console.Write(v + " ");            

            Console.ReadKey();
        }
    }
}
