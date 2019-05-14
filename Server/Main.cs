using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
//using FutureConcepts.SystemTools.Devices.PowerSupplyControlLibrary;
//using FutureConcepts.SystemTools.Management.SystemShutdown;
//using FutureConcepts.SystemTools.Monitoring.SystemPanel;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server
{
    class MediaServer
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);

            if (args.Length > 0 && args[0].ToUpper() == "DEBUG")
            {
                Process debugViewProcess = null;
                if (args.Length > 1 && args[1].ToUpper() == "VIEWER")
                {
                    ProcessStartInfo debugViewStartInfo = new ProcessStartInfo("DbgView.exe", null);
                    debugViewStartInfo.UseShellExecute = false;
                    debugViewProcess = Process.Start(debugViewStartInfo);
                    debugViewProcess.WaitForInputIdle();
                }
                WindowsService windowsService = new WindowsService();
                windowsService.StartDebug();
                if (debugViewProcess != null)
                {
                    debugViewProcess.WaitForExit();
                }
                else
                {
                    Console.ReadLine();
                }
                Console.WriteLine("DebugView has been closed");
                windowsService.EndDebug();
                Console.WriteLine("Goodbye!");
            }
            else
            {
                ServiceBase.Run(new WindowsService());
            }
        }

        static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppLogger.Message("Unhandled Exception!");
            Exception exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                AppLogger.Message("found CLS-compliant exception");
                AppLogger.Dump(exc);
                ErrorLogger.DumpToEventLog(exc, EventLogEntryType.Error);
            }
            else
            {
                string info = String.Format("--- Non-CLS-Compliant exception: Type={0}, String={1}",
                    e.ExceptionObject.GetType(), e.ExceptionObject.ToString());
                AppLogger.Message(info);
            }
        }

        #region Server Needs to Commit Suicide Support

        /// <summary>
        /// Causes the Media Server process to terminate itself
        /// </summary>
        public static void CommitSuicide()
        {
            AppLogger.Message("MediaServer process is aborting itself!");

            GraphManager.ShutdownInProgress = true;

            AppLogger.Message("-- Aborting all WCF Services!");
            IndigoServices.Host.AbortServices();

            ErrorLogger.WriteToEventLog("MediaServer process is aborting itself!", EventLogEntryType.Error);
            AppLogger.Message("-- Terminating MediaServer process");
            Process.GetCurrentProcess().Kill();
        }

        #endregion

        #region Server Needs Power Cycle Support

        private static bool _needsPowerCycle = false;
        /// <summary>
        /// This flag is set to true if the server machine requires a power cycle, as determined by the Media Server.
        /// Please note, if the app.config key DeadMangoSelfPowerCycle is set to true, MS will take it upon itself
        /// to perform the power cycle, when setting this flag
        /// </summary>
        public static bool ServerNeedsPowerCycle
        {
            get
            {
                return _needsPowerCycle;
            }
            set
            {
                _needsPowerCycle = value;

                if (value)
                {
                    //spin off another thread to take care of this unpleasentness
                    Thread t = new Thread(new ThreadStart(HandlePowerCycle));
                    t.Start();
                }
            }
        }

        private static bool _powerCycleInvoked = false;
        /// <summary>
        /// Handles power cycling.
        /// </summary>
        private static void HandlePowerCycle()
        {
            try
            {
                if (_powerCycleInvoked)
                {
                    AppLogger.Message("Power Cycle has already been invoked!");
                    ErrorLogger.WriteToEventLog("Power Cycle has already been invoked!", EventLogEntryType.Information);
                    return;
                }

                bool selfRestart = false;
                bool.TryParse(ConfigurationManager.AppSettings["DeadMangoSelfPowerCycle"], out selfRestart);
                if (!selfRestart)
                {
                    AppLogger.Message("DeadMangoSelfPowerCycle is not configured.");
                    ErrorLogger.WriteToEventLog("DeadMangoSelfPowerCycle is not configured.", EventLogEntryType.Information);
                    return;
                }

                _powerCycleInvoked = true;  //guard against multiple power cycling

                GraphManager.ShutdownInProgress = true;
//                SystemPanelSettings sysPanelSettings = new SystemPanelSettings();
//                if (sysPanelSettings.PowerSupplyExists)
//                {
//                    PowerSupplyController ps = new PowerSupplyController(sysPanelSettings.PowerSupplyPort);
//                    bool success = false;
//                    for (int i = 0; i < 10; i++)
//                    {
//                        //tell power supply to shutdown and restart
//                        success = ps.Shutdown(true);
//                        if (success)
//                        {
//                            AppLogger.Message("Power supply set to power cycle in " + ps.GetSecondsToShutdown() + " seconds.");
//                            ErrorLogger.WriteToEventLog("Power supply set to power cycle in " + ps.GetSecondsToShutdown() + " seconds.", EventLogEntryType.Information);
//                            //tell Windows to shutdown
//                            if (!Host.Shutdown("localhost", "localhost"))
//                            {
//                                AppLogger.Message("Failed to invoke ShutdownService!");
//                                ErrorLogger.WriteToEventLog("Failed to invoke ShutdownService!", EventLogEntryType.Warning);
//                            }
//                            return;
//                        }
//                    }
//                    AppLogger.Message("Failed to power cycle computer!");
//                    ErrorLogger.WriteToEventLog("Failed to power cycle computer!", EventLogEntryType.Error);
//                }
//                else
//                {
//                    //we can't solve the problem, SUPER BUMMER
//                    //TODO email notification??
//                    AppLogger.Message("Power Supply cannot be controlled or is not configured!");
//                    ErrorLogger.WriteToEventLog("Power Supply cannot be controlled or is not configured!", EventLogEntryType.Error);
//                    GraphManager.ShutdownInProgress = false; // re-enable incoming connections
//                }
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                ErrorLogger.DumpToEventLog(ex);
                ErrorLogger.WriteToEventLog("Failed to handle MX CardEnable Timeout!", EventLogEntryType.Error);
            }
        }

        #endregion
    }
}
