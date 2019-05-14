using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Timers;

using FutureConcepts.Media.Client;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Server.ZViewer
{
    public class AppContext
    {
        GraphControl _proxy;
        Graph _graph;
        SessionDescription _sessionDescription;

        private Process _quickTimeProcess;

        public void Run(string[] args)
        {
            bool useVideoServer = true;
            string sourceName = "Vid1";
            string serverAddress = "localhost";

            Console.WriteLine("ZViewer is used to troubleshoot videoserver problems.");
         //   try
            {
                foreach (string arg in args)
                {
                    string[] parts = arg.Split(new char[] { '=' });
                    switch (parts[0])
                    {
                        case "server":
                            serverAddress = parts[1];
                            break;
                        case "source":
                            sourceName = parts[1];
                            break;
                        case "sinkurl":
                            _sessionDescription = new SessionDescription();
                            _sessionDescription.ClientURL = parts[1];
                            useVideoServer = false;
                            break;
                        default:
                            Console.WriteLine("Invalid Option " + parts[0]);
                            break;
                    }
                }
                _proxy = new GraphControl(serverAddress);
                Console.WriteLine("server=" + _proxy.ServerAddress);
                Console.WriteLine("source=" + sourceName);
                ClientConnectRequest clientRequest = new ClientConnectRequest(sourceName);
                _sessionDescription = _proxy.OpenGraph(clientRequest);
                Console.WriteLine("sinkURL=" + _sessionDescription.ClientURL);
                if (_sessionDescription.ClientURL.Contains("http:"))
                {
                    Console.WriteLine("The graph is now running but nothing will be rendered here.");
                }
                else if (!_sessionDescription.ClientURL.Contains("rtp:"))
                {
                    _graph = new Graph(_sessionDescription.ClientURL, null);
                    _graph.Run();
                }
                Console.WriteLine("b\tshow current bitrate");
                Console.WriteLine("r\tstart recording");
                Console.WriteLine("s\tstop recording");
                Console.WriteLine("x\tstop viewing");
                bool running = true;

                while (running)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    switch (keyInfo.KeyChar)
                    {
                        case 'b':
                            if (_graph != null)
                            {
                                int bitRate = _graph.AvgBitRate;
                                Console.WriteLine(bitRate / 1024 + " kbps");
                            }
                            break;
                        case 'x':
                            running = false;
                            break;
                        case 'r':
                            {
                                string recFileName = sourceName + @"\";
                                Directory.CreateDirectory(recFileName);
                                DateTime utc = DateTime.UtcNow;
                                recFileName += utc.Year.ToString() + utc.Month.ToString() + utc.Day.ToString() + utc.Hour.ToString() + utc.Minute.ToString() + ".lts";
                                _graph.RecordingFileName = recFileName;
                                Console.WriteLine("Started recording to file: " + recFileName);
                            }
                            break;
                        case 's':
                            _graph.RecordingFileName = null;
                            Console.WriteLine("Stopped recording");
                            break;
                    }
                }
                if (_graph != null)
                {
                    _graph.Dispose();
                    _graph = null;
                }
                if (_quickTimeProcess != null)
                {
                    Console.WriteLine("trying to kill QuickTime.");
                    _quickTimeProcess.CloseMainWindow();
                    _quickTimeProcess.Kill();
                    _quickTimeProcess.Dispose();
                    _quickTimeProcess = null;
                }
                _proxy.Dispose();
                System.Threading.Thread.Sleep(3000);
                _proxy = null;
            }
         //   catch (Exception exc)
        //    {
          //      Console.WriteLine(exc.Message);
          //  }
        }
    }
}
