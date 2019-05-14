using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using FutureConcepts.Tools;
using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD
{
    static class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
//            DebugTestVivotekTunnel();
//              return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_UnhandledException);
 
            try
            {
                Application.Run(new SVDMain(args));
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
        }

        private static void DebugTestVivotekTunnel()
        {
//            FutureConcepts.Media.Client.CameraControlClients.VivotekTunnelPelcoD tunnel = new Client.CameraControlClients.VivotekTunnelPelcoD("vvtktunnel-pelcoD://10.0.201.135/uartchannel.cgi?channel=0");
//            tunnel.Initialize();
//            tunnel.PanTiltAbsolute(90.0, 90.0);
//            tunnel.PanTiltZoomPositionInquire();
//            double panAngle = tunnel.CurrentPanAngle;
//            double tiltAngle = tunnel.CurrentTiltAngle;
//            double zoomPosition = tunnel.CurrentZoomPosition;
//            tunnel.Close();
//            tunnel.Dispose();
        }



        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                ErrorLogger.WriteToEventLog("Unhandled Thread Exception. See next entry for Exception dump.", EventLogEntryType.Error);
                ErrorLogger.DumpToEventLog(e.Exception, EventLogEntryType.Error);

                string path = FutureConcepts.Settings.GenericSettings.AntaresXDataPath + @"Output\SVD\Errors\";
                Directory.CreateDirectory(path);
                StreamWriter streamWriter = new StreamWriter(path + "ThreadException.txt", true);
                streamWriter.WriteLine("-- Unhandled ThreadException @ " + DateTime.Now.ToString() + " --");
                DumpException(streamWriter, e.Exception);
                ErrorLogger.DumpToDebug(e.Exception);
                streamWriter.Close();
                streamWriter.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToEventLog("Error while writing ThreadException log!", EventLogEntryType.Error);
                ErrorLogger.DumpToEventLog(ex);
            }
            finally
            {
                FCMessageBox.Show("Network Error", "Received network error---please restart Streaming Video Desktop [code 1]");
                Application.Exit();
            }
        }

        static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ErrorLogger.WriteToEventLog("Unhandled AppDomain Exception. See next entry for Exception dump.", EventLogEntryType.Error);
                Exception exc = e.ExceptionObject as Exception;
                if (exc != null)
                {
                    ErrorLogger.DumpToEventLog(e.ExceptionObject as Exception, EventLogEntryType.Error);
                }
                else
                {
                    ErrorLogger.WriteToEventLog(e.ExceptionObject.GetType().ToString() + " " + e.ExceptionObject.ToString(), EventLogEntryType.Error);
                }

                string path = FutureConcepts.Settings.GenericSettings.AntaresXDataPath + @"Output\SVD\Errors\";
                Directory.CreateDirectory(path);
                StreamWriter streamWriter = new StreamWriter(path + "AppDomainException.txt", true);
                streamWriter.WriteLine("-- Unhandled AppDomain Exception @ " + DateTime.Now.ToString() + " --");
                if (exc != null)
                {
                    streamWriter.WriteLine("found CLS-compliant exception");
                    DumpException(streamWriter, exc);
                    ErrorLogger.DumpToDebug(exc);
                }
                else
                {
                    string info = String.Format("--- Non-CLS-Compliant exception: Type={0}, String={1}",
                        e.ExceptionObject.GetType(), e.ExceptionObject.ToString());
                    streamWriter.WriteLine(info);
                }
                streamWriter.Close();
                streamWriter.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToEventLog("Error while writing AppDomain Exception log!", EventLogEntryType.Error);
                ErrorLogger.DumpToEventLog(ex);
            }
            finally
            {
                FCMessageBox.Show("Network Error", "Received network error---please restart Streaming Video Desktop [code 2]");
                Application.Exit();
            }
        }

        static void DumpException(StreamWriter wr, Exception e)
        {
            wr.WriteLine("Exception.Message: " + e.Message);
            wr.WriteLine("Exception.StackTrace: " + e.StackTrace);
            wr.WriteLine("Exception.TargetSite.Name: " + e.TargetSite.Name);
            wr.WriteLine("Exception.Source: " + e.Source);
            if (e.InnerException != null)
            {
                wr.WriteLine("InnerException:");
                DumpException(wr, e.InnerException);
            }
        }
    }
}