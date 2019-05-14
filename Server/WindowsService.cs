using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Text;
using System.Xml.Serialization;

using FutureConcepts.Media.Server.Graphs;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server
{
    partial class WindowsService : ServiceBase
    {
        private bool _serviceMode = true;

        public bool ServiceMode
        {
            get
            {
                return _serviceMode;
            }
            set
            {
                _serviceMode = value;
            }
        }

        private RTSP.RTSPServer _rtspServer;

        public WindowsService()
        {
            InitializeComponent();
        }

        public void StartDebug()
        {
            ServiceMode = false;
            OnStart(new string[] { });
        }

        public void EndDebug()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                AppLogger.ShowAssembly = false;
                AppLogger.ShowClass = false;
                AppLogger.ShowDate = false;
                AppLogger.ShowMethod = false;
                AppLogger.Message(DateTime.Now.ToString() + " " + Assembly.GetAssembly(typeof(MediaServer)).ToString());
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                if (ServiceMode)
                {
                    //this.RequestAdditionalTime(60000);
                }
                SetupNativeEnvironment();
//                QueryOriginServers.StartBackgroundThread();
                IndigoServices.Host.StartServices();
                GraphManager.ShutdownInProgress = false;
                GraphManager.StartPushGraphs();
//                _rtspServer = RTSP.RTSPServer.LoadFromFile();
//                if (_rtspServer != null)
//                {
//                    _rtspServer.Start();
//                }
                NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
                NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
                AppLogger.Message("MediaServer is READY");
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                AppLogger.Dump(e.ExceptionObject as Exception);
                ErrorLogger.DumpToEventLog(e.ExceptionObject as Exception);
            }
            if (e.IsTerminating)
            {
                IndigoServices.Host.StopServices();
            }
        }

        protected override void OnStop()
        {
            if (_rtspServer != null)
            {
                _rtspServer.Stop();
            }
            GraphManager.ShutdownInProgress = true;
//            QueryOriginServers.StopBackgroundThread();
            GraphManager.StopAllGraphs();
            Thread.Sleep(2000);
            GraphManager.AbortAllGraphs();
            IndigoServices.Host.StopServices();
        }

        private void SetupNativeEnvironment()
        {
            AppLogger.Message("SetupNativeEnvironment:");
            NameValueCollection env = ConfigurationManager.GetSection("nativeEnvironment") as NameValueCollection;
            DirectoryInfo dirInfo = Directory.GetParent(Assembly.GetEntryAssembly().Location);
            string workingDirectory = dirInfo.FullName;
            Environment.SetEnvironmentVariable("AntaresX_Video_WorkingDirectory", workingDirectory);
            AppLogger.Message("    WorkingDirectory=" + workingDirectory);
            for (int i = 0; i < env.Count; i++)
            {
                Environment.SetEnvironmentVariable("AntaresX_Video_" + env.GetKey(i), env.Get(i));

                AppLogger.Message(String.Format("    {0}={1}", env.GetKey(i), env.Get(i)));
            }
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            ShowAllNetworkInterfaces();
            //            throw new Exception("Network Address Changed");
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            ErrorLogger.WriteToEventLog("Network Availability Changed - now " + e.IsAvailable.ToString(), EventLogEntryType.Information);
        }

        private void ShowAllNetworkInterfaces()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                AppLogger.Message("Network Interfaces:");
                foreach (NetworkInterface iface in interfaces)
                {
                    AppLogger.Message(String.Format("    {0} is {1}", iface.Name, iface.OperationalStatus));
                    IPInterfaceProperties props = iface.GetIPProperties();
                    foreach (UnicastIPAddressInformation info in props.UnicastAddresses)
                    {
                        AppLogger.Message(String.Format("        {0}", info.Address.ToString()));
                    }
                }
            }
            catch (Exception exc)
            {
                AppLogger.Message(exc.Message);
            }
        }
    }
}
