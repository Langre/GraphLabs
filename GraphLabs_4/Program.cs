using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphsLabs;

namespace GraphLabs_4
{
    class Program
    {
        static void Main(string[] args)
        {
            WeightedGraph wg = new WeightedGraph("../../../Lab04-gr5-25.dat");
            Console.WriteLine("//// Borvka ////");
            wg.Borvka();
            Console.WriteLine();
            Console.WriteLine("//// PRIMA-YARNIK ////");
            wg.PrimaYarnik();
            Console.WriteLine();
            Console.WriteLine("//// KRUSKAL ////");
            wg.Kraskal();
            Console.ReadKey();
        }
    }
}
