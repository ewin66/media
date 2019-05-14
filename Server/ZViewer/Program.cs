using System;
using System.Collections.Generic;
using System.Text;
//using System.Threading;
using System.Timers;

namespace FutureConcepts.Media.Server.ZViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext app = new AppContext();
            app.Run(args);
            System.Threading.Thread.Sleep(2000);
        }
    }
}
