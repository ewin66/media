using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using FutureConcepts.Media.Contract;
using FutureConcepts.Tools;
using System.Threading;
using System.Configuration;
using System.Reflection;

namespace FutureConcepts.Media.Server.IndigoServices
{
    public static class Host
    {
        private static List<ServiceHost> _services = new List<ServiceHost>();
        private static List<WebServiceHost> _webServices = new List<WebServiceHost>();

        /// <summary>
        /// when a ServiceHost faults more than this many times in a row, the service process will be terminated
        /// </summary>
        private static int _maxFaultsPerService = 10;

        static Host()
        {
            int temp;
            if (Int32.TryParse(ConfigurationManager.AppSettings["IndigoServicesHostMaxFaults"], out temp))
            {
                _maxFaultsPerService = temp;
            }
        }

        #region Public API

        /// <summary>
        /// Starts all known types of WCF Services
        /// </summary>
        public static void StartServices()
        {          
            Type[] types = {
                //typeof(ServerStatusService),
                typeof(ServerConfigService),
                typeof(StreamService),
                typeof(TVStreamService),
                //typeof(RecordService),
                typeof(CameraControlService),
                typeof(MicrowaveControlService),
                typeof(MicrowaveControlService2)
            };
            Type[] webTypes = {
                typeof(WebStreamService)
            };

            foreach (Type type in types)
            {
                AddService(type);
            }

            foreach (Type type in webTypes)
            {
                AddWebService(type);
            }
        }

        /// <summary>
        /// Closes all Hosted WCF Services
        /// </summary>
        public static void StopServices()
        {
            foreach (ServiceHost host in _services)
            {
                try
                {
                    DetachService(host);
                    host.Close();
                }
                catch
                {
                }
            }
            _services.Clear();
            _services = null;
           
            //clean up web service hosts
            foreach (WebServiceHost wsHost in _webServices)
            {
                try
                {
                    DetachService(wsHost);
                    wsHost.Close();
                }
                catch
                {
                }
            }
            _webServices.Clear();
            _webServices = null;
        }

        /// <summary>
        /// Aborts all Hosted WCF Services
        /// </summary>
        public static void AbortServices()
        {
            foreach (ServiceHost host in _services)
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(AbortService), host);
                }
                catch
                {
                }
            }
            _services.Clear();
            _services = null;

            //clean up web service hosts
            foreach (WebServiceHost wsHost in _webServices)
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(AbortService), wsHost);
                }
                catch
                {
                }
            }
            _webServices.Clear();
            _webServices = null;

            Thread.Sleep(2000); //give 2 seconds for services to shut down
        }

        #endregion

        #region API Helpers

        /// <summary>
        /// WaitCallback implementation to abort a service
        /// </summary>
        /// <param name="hostObj"></param>
        private static void AbortService(object hostObj)
        {
            if (hostObj is ServiceHost)
            {
                try
                {
                    //DetachService(hostObj as ServiceHost);
                    ((ServiceHost)hostObj).Abort();
                }
                catch (Exception ex)
                {
                    ErrorLogger.WriteToEventLog("Could not abort service " + ((ServiceHost)hostObj).Description.ConfigurationName + "!", EventLogEntryType.Error);
                    AppLogger.Dump(ex);
                    ErrorLogger.DumpToEventLog(ex);
                }
            }
        }

        private static void AddService(Type type)
        {
            ServiceHost serviceHost = new ServiceHost(type);
            serviceHost.Opening += new EventHandler(serviceHost_Opening);
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Faulted += new EventHandler(serviceHost_Faulted);
            serviceHost.Closing += new EventHandler(serviceHost_Closing);
            serviceHost.Closed += new EventHandler(serviceHost_Closed);
            serviceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);

            serviceHost.Open();
            _services.Add(serviceHost);
        }

        private static void AddWebService(Type type)
        {
            WebServiceHost serviceHost = new WebServiceHost(type);
            serviceHost.Opening += new EventHandler(serviceHost_Opening);
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Faulted += new EventHandler(serviceHost_Faulted);
            serviceHost.Closing += new EventHandler(serviceHost_Closing);
            serviceHost.Closed += new EventHandler(serviceHost_Closed);
            serviceHost.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);
            serviceHost.Open();
            _webServices.Add(serviceHost);
        }

        /// <summary>
        /// Detaches listeners from a service host
        /// </summary>
        /// <param name="host">host to remove</param>
        private static void DetachService(ServiceHost host)
        {
            host.Opening -= new EventHandler(serviceHost_Opening);
            host.Opened -= new EventHandler(serviceHost_Opened);
            host.Faulted -= new EventHandler(serviceHost_Faulted);
            host.Closing -= new EventHandler(serviceHost_Closing);
            host.Closed -= new EventHandler(serviceHost_Closed);
            host.UnknownMessageReceived -= new EventHandler<UnknownMessageReceivedEventArgs>(serviceHost_UnknownMessageReceived);

        }

        #endregion

        #region serviceHost Event Handlers

        private static Dictionary<string, int> _faultRecord = new Dictionary<string, int>();

        /// <summary>
        /// Increments the fault count for the specified ServiceHost. If the new count
        /// is greater or equal to the _maxFaultsPerService, then the MediaServer Process is terminated.
        /// </summary>
        /// <param name="serviceHostName">service host configuration name</param>
        private static void RecordFault(string serviceHostName)
        {
            if (_maxFaultsPerService < 1)
            {
                return;
            }

            if (!_faultRecord.ContainsKey(serviceHostName))
            {
                _faultRecord.Add(serviceHostName, 1);
            }
            else
            {
                _faultRecord[serviceHostName]++;
                if (_faultRecord[serviceHostName] >= _maxFaultsPerService)
                {
                    ErrorLogger.WriteToEventLog("The ServiceHost " + serviceHostName + " reached the max consecutive fault limit (" + _maxFaultsPerService + ")", EventLogEntryType.Error);
                    MediaServer.CommitSuicide();
                }
            }
        }

        /// <summary>
        /// Resets the fault count for a service host to 0
        /// </summary>
        /// <param name="serviceHostName">service host configuration name</param>
        private static void ResetFault(string serviceHostName)
        {
            _faultRecord.Remove(serviceHostName);
        }


        private static void serviceHost_Opening(object sender, EventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            AppLogger.Message("WCF Service " + host.BaseAddresses[0].AbsoluteUri + " (" + host.State.ToString() + ")");
        }

        private static void serviceHost_Opened(object sender, EventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            AppLogger.Message("WCF Service " + host.Description.ConfigurationName + " (" + host.State.ToString() + ")");
            ResetFault(host.Description.ConfigurationName);
            foreach (ServiceEndpoint serviceEndpoint in host.Description.Endpoints)
            {
                AppLogger.Message(String.Format("  {0} ({1})", serviceEndpoint.Address.ToString(), serviceEndpoint.Binding.Name));
            }
        }

        /// <summary>
        /// Restarts any ServiceHost that faults.
        /// </summary>
        private static void serviceHost_Faulted(object sender, EventArgs e)
        {
            try
            {
                ServiceHost host = (ServiceHost)sender;
                
                string message = "WCF Service " + host.Description.ConfigurationName + " (" + host.State.ToString() + ")";
                AppLogger.Message(message);
                ErrorLogger.WriteToEventLog(message, EventLogEntryType.Error);

                RecordFault(host.Description.ConfigurationName);

                DetachService(host);    //remove listeners

                if (host is WebServiceHost)
                {
                    _webServices.Remove((WebServiceHost)host);
                    host.Abort();
                    Thread.Sleep(1000);
                    AddWebService(host.Description.ServiceType);
                }
                else
                {
                    _services.Remove(host);
                    host.Abort();
                    Thread.Sleep(1000);
                    AddService(host.Description.ServiceType);
                }
                
                host = null;
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                ErrorLogger.DumpToEventLog(ex, EventLogEntryType.Error);
            }
        }

        private static void serviceHost_Closing(object sender, EventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            AppLogger.Message("WCF Service " + host.BaseAddresses[0].AbsoluteUri + " (" + host.State.ToString() + ")");
        }

        private static void serviceHost_Closed(object sender, EventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            AppLogger.Message("WCF Service " + host.Description.ConfigurationName + " (" + host.State.ToString() + ")");
        }

        private static void serviceHost_UnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            AppLogger.Message("WCF Service " + host.BaseAddresses[0].AbsoluteUri + " UnknownMessageReceived" + Environment.NewLine + e.Message);
        }

        #endregion
    }
}
