using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using FutureConcepts.SystemTools.Networks.AntaresXNetworkServices;

namespace FutureConcepts.Media.SVD.QueryVideoServices
{
    class Program
    {
        public enum Command
        {
            Invalid = -1,
            list = 0,
            get,
            getall,
            watch,
            exit,
            help,
            server
        }

        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString() + " QueryVideoServices " + new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);

            bool argsOnCommandLine = args.Length > 0;
            Dictionary<Command, string> argList = BuildArgList(args);

            do
            {
                if (argList.Count == 0)
                {
                    argList.Add(Command.help, null);
                }

                if (argsOnCommandLine)
                {
                    argList.Add(Command.exit, null);
                }


                foreach (KeyValuePair<Command, string> c in argList)
                {
                    switch (c.Key)
                    {
                        case Command.list:
                            Engine.PrintAdvertisedServices();
                            break;
                        case Command.get:
                            Engine.PrintQueryService(c.Value);
                            break;
                        case Command.getall:
                            Engine.PrintAdvertisedServices();
                            List<AdvertisedServiceInfo> axinfo = Engine.GetAllAdvertisedServices();
                            foreach (AdvertisedServiceInfo i in axinfo)
                            {
                                Engine.PrintQueryService(i);
                                Console.WriteLine();
                                Console.WriteLine();
                            }
                            break;
                        case Command.watch:
                            ServerWatcher w = new ServerWatcher();
                            w.Run();
                            break;
                        case Command.exit:
                            return;
                        case Command.help:
                            Console.WriteLine("Commands:");
                            Console.WriteLine("       help - this menu");
                            Console.WriteLine("       list - lists all advertised video services");
                            Console.WriteLine("       get hostOrIP - queries a specific host or IP");
                            Console.WriteLine("       getall - attempts to query all advertised servers");
                            Console.WriteLine("       watch - starts a QueryMediaServers watcher");
                            Console.WriteLine("       exit - exits");
                            break;
                        case Command.server:
                            Engine.NetmanagerAddress = c.Value;
                            Console.WriteLine("Netmanager address set to: " + c.Value);
                            break;
                    }
                }

                if (!argsOnCommandLine)
                {
                    Console.WriteLine();
                    Console.Write("qvs > ");
                    string input = Console.ReadLine();
                    Console.WriteLine();
                    argList = BuildArgList(input.Split(' '));
                }
            }
            while (true);
        }

        private static Dictionary<Command, string> BuildArgList(string[] args)
        {
            Dictionary<Command, string> list = new Dictionary<Command, string>();
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    Command c = (Command)Enum.Parse(typeof(Command), args[i].ToLowerInvariant());
                    string value = null;
                    if ((c == Command.get) && (i < args.Length - 1))
                    {
                        value = args[i + 1];
                        i++;
                    }
                    if (c != Command.Invalid)
                    {
                        list.Add(c, value);
                    }
                }
                catch(Exception)
                {
                }
            }
            return list;
        }
    }
}
