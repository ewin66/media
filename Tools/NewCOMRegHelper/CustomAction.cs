using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Microsoft.Deployment.WindowsInstaller;

namespace FutureConcepts.Media.Tools.NewCOMRegHelper
{
    public class CustomActions
    {
        #region Registration Core Helpers

        protected static bool SilentMode
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers all the libraries specified in the array of relative paths
        /// </summary>
        /// <param name="dllPaths">an array of relative paths to libraries, relative to the installation path</param>
        protected static void RegisterDLL(Session session, string[] dllPaths)
        {
            foreach (string dll in dllPaths)
            {
                RegisterDLL(session, dll);
            }
        }

        /// <summary>
        /// Registers the specified library.
        /// </summary>
        /// <param name="relativePath">relative path to the library, relative to the installation path</param>
        protected static void RegisterDLL(Session session, string relativePath)
        {
            String installLocation = session.CustomActionData["INSTALLLOCATION"];
//            string filename = Directory.GetParent(Assembly.GetCallingAssembly().Location) + @"\" + relativePath;
            string filename = installLocation + @"\" + relativePath;
            session.Log("Attempting to register " + filename);
            try
            {
                System.IO.FileAttributes fileAttributes = File.GetAttributes(filename);
                string cmdLine = "/s \"" + filename + "\"";
                session.Log(String.Format("regsvr32 {0}", cmdLine));
                Process proc = Process.Start("regsvr32", cmdLine);
                proc.WaitForExit();
                session.Log(String.Format("regsvr32 returned {0}", proc.ExitCode));
                proc.Dispose();
            }
            catch (Exception exc)
            {
                session.Log("Failed to register " + filename);
                session.Log(exc.Message);
                if (!SilentMode)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message, "Exception occurred while registering " + filename);
                }
            }
        }

        /// <summary>
        /// Unregisters all the libraries specified in the array of relative paths.
        /// </summary>
        /// <param name="dllPaths">an array of relative paths to libraries, relative to the installation path</param>
        protected static void UnregisterDLL(Session session, string[] dllPaths)
        {
            foreach (string dll in dllPaths)
            {
                UnregisterDLL(session, dll);
            }
        }

        /// <summary>
        /// Unregisters the specified library
        /// </summary>
        /// <param name="relativePath">relative path to the library, relative to the installation path</param>
        protected static void UnregisterDLL(Session session, string relativePath)
        {
            string cmdLine = "/s /u \"";
            cmdLine += session.CustomActionData["INSTALLLOCATION"] + @"/";
            cmdLine += relativePath + "\"";
            try
            {
                Process proc = Process.Start("regsvr32.exe", cmdLine);
                proc.WaitForExit();
                session.Log(String.Format("regsvr32 returned {0}", proc.ExitCode));
                proc.Dispose();
            }
            catch (Exception exc)
            {
                session.Log(exc.Message);
                session.Log("Exception occurred while unregistering " + relativePath);
            }
        }

        #endregion

        #region Parameter / InstallState Helpers

        /// <summary>
        /// Fetches a parameter's value from the command line
        /// </summary>
        /// <param name="paramName">parameter name</param>
        /// <returns>the parameter's value, or null if the parameter was not present</returns>
        protected static string GetParameter(Session session, string paramName)
        {
            session.Log("GetParameter " + paramName);
            if (session.CustomActionData.ContainsKey(paramName))
            {
                string value = session.CustomActionData[paramName];
                if (value == null)
                {
                    session.Log(String.Format("GetParameter {0} got NULL", paramName));
                    return null;
                }
                session.Log(String.Format("GetParameter {0} got {1}", paramName, value));
                return value;
            }
            else
            {
                session.Log("CustomActionData does not contain " + paramName);
                return null;
            }
        }

        /// <summary>
        /// Updates the SilentMode property based on the contents of the command line
        /// </summary>
        private static void SetSilentMode(Session session)
        {
            SilentMode = false;
            if (GetParameter(session, "mode") == "2")
            {
                SilentMode = true;
                Debug.WriteLine("Registration Installer detected UI Level SILENT");
            }
        }

        /// <summary>
        /// Fetches the Targets from the command line
        /// </summary>
        /// <returns>the array of target DLLs to register or unregister</returns>
        private static string[] GetTargets(Session session)
        {
            try
            {
                string input = GetParameter(session, "targets");
                string delim = ",";
                string[] output = null;
                if (input != null)
                {
                    output = input.Split(new string[] { delim }, StringSplitOptions.RemoveEmptyEntries);
                    if (output.Length == 0)
                    {
                        return null;
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DLLHelper.GetTargets Failed... " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// key used to store targets in InstallState
        /// </summary>
        private const string TargetsKey = "targets";

        #endregion

        [CustomAction]
        public static ActionResult Install(Session session)
        {
            try
            {
                session.Log("DLLHelper.Install");
                SetSilentMode(session);

                string[] targets = GetTargets(session);
                if (targets == null)
                {
                    session.Log("DLLHelper did not find any targets to register!");
                }
                else
                {
                    RegisterDLL(session, targets);
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("DLLHelper.Install: " + ex.ToString());
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult Rollback(Session session)
        {
            try
            {
                session.Log("DLLHelper.Rollback");
                SetSilentMode(session);

                //command line overrides
                string[] targets = GetTargets(session);
                if (targets != null)
                {
                    session.Log("DLLHelper.Rollback: targets list overridden by command line");
                    UnregisterDLL(session, targets);
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("DLLHelper.Rollback: " + ex.ToString());
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult Commit(Session session)
        {
            try
            {
                session.Log("DLLHelper.Commit");
                SetSilentMode(session);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("DLLHelper.Commit: " + ex.ToString());
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult Uninstall(Session session)
        {
            try
            {
                session.Log("DLLHelper.Uninstall");
                SetSilentMode(session);

                string[] targets = GetTargets(session); //get from command line
                if (targets != null)
                {
                    UnregisterDLL(session, targets);
                }
                else
                {
                    session.Log("DLLHelper did not find any targets to register!");
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("DLLHelper.Uninstall: " + ex.ToString());
                return ActionResult.Failure;
            }
        }
    }
}
