using System;
using System.Collections.Generic;
using System.Text;

namespace FutureConcepts.Media.Server.TestFastVDO
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseGraph graph = BaseGraph.CreateInstance();
            graph.Run();
            string cmd = Console.ReadLine();
            graph.Stop();
            graph.Dispose(true);
        }
    }
}
