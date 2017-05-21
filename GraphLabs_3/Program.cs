using System;

using GraphLabs_2;

namespace GraphLabs_3
{
    class Program
    {
        static void Main(string[] args)
        {
            var asGraph2 = new DirectedGraph();
            asGraph2.InitGraph("../../../Lab02-gr5-25.dat");

            Console.WriteLine("Ярусно-параллельная форма графа из лаб2: ");
            var tf =SomeGraphFuncs.ToTiersForms(asGraph2);
            for (int i = 0; i < tf.Count; i++)
            {
                foreach (var v in tf[i])
                    Console.Write(v + " ");
                Console.WriteLine();
            }

            Console.WriteLine("Компоненты сильной связности: ");
            var graph3 = new DirectedGraph();
            graph3.InitGraph("../../../Lab03-gr5-25.dat");
            SomeGraphFuncs.GetMaxComponents(graph3);

            Console.WriteLine("Списки смежности метаграфа: ");
            var some = SomeGraphFuncs.GetMetaGraph(graph3);
            foreach (var v in some)
            {
                Console.Write(v.Key + ": ");
                foreach (var n in v.Value)
                    Console.Write(n + ", ");
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
