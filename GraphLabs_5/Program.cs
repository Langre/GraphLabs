using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLabs_5
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectedWeightedGraph dwg = new DirectedWeightedGraph("../../../Lab05-gr5-25.dat");
            Console.WriteLine("Короткий путь из вершины 0 методом Форда со стеком");
            dwg.FordWithStack(0);
            dwg.PrintShortWay();
            Console.WriteLine("Короткий путь из вершины 0 методом Форда с очередью");
            dwg.FordWithQueue(0);
            dwg.PrintShortWay();
            Console.WriteLine("Короткий путь из вершины 0 методом Форда с приоритетной очередью");
            dwg.FordWithPriorityQueue(0);
            dwg.PrintShortWay();

            Console.ReadLine();
        }
    }
}
