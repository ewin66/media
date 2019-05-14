using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace FutureConcepts.Media.Tools.COMRegHelper
{
    [RunInstaller(true)]
    public partial class DLLHelper : Installer
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
        protected static void RegisterDLL(string[] dllPaths)
        {
            foreach (string dll in dllPaths)
            {
                RegisterDLL(dll);
            }
        }

        /// <summary>
        /// Registers the specified library.
        /// </summary>
        /// <param name="relativePath">relative path to the library, relative to the installation path</param>
        protected static void RegisterDLL(string relativePath)
        {
            try
            {
                string filename = Directory.GetParent(Assembly.GetCallingAssembly().Location) + @"\" + relativePath;
                FileAttributes fileAttributes = File.GetAttributes(filename);
                string cmdLine = "/s \"" + filename + "\"";
                Debug.WriteLine(String.Format("regsvr32 {0}", cmdLine));
                Process proc = Process.Start("regsvr32", cmdLine);
                proc.WaitForExit();
                Debug.WriteLine(String.Format("regsvr32 returned {0}", proc.ExitCode));
                proc.Dispose();
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Failed to register " + relativePath);
                Debug.WriteLine(exc.Message);
                if (!SilentMode)
                {
                    System.Windows.Forms.MessageBox.Show(exc.Message, "Exception occurred while registering " + relativePath);
                }
            }
        }

        /// <summary>
        /// Unregisters all the libraries specified in the array of relative paths.
        /// </summary>
        /// <param name="dllPaths">an array of relative paths to libraries, relative to the installation path</param>
        protected static void UnregisterDLL(string[] dllPaths)
        {
            foreach (string dll in dllPaths)
            {
                UnregisterDLL(dll);
            }
        }

        /// <summary>
        /// Unregisters the specified library
        /// </summary>
        /// <param name="relativePath">relative path to the library, relative to the installation path</param>
        protected static void UnregisterDLL(string relativePath)
        {
            string cmdLine = "/s /u \"";
            cmdLine += Directory.GetParent(Assembly.GetCallingAssembly().Location) + @"/";
            cmdLine += relativePath + "\"";
            try
            {
                Process proc = Process.Start("regsvr32.exe", cmdLine);
                proc.WaitForExit();
                Debug.WriteLine(String.Format("regsvr32 returned {0}", proc.ExitCode));
                proc.Dispose();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine("Exception occurred while unregistering " + relativePath);
            }
        }

        #endregion

        #region Parameter / InstallState Helpers

        /// <summary>
        /// Fetches a parameter's value from the command line
        /// </summary>
        /// <param name="paramName">parameter name</param>
        /// <returns>the parameter's value, or null if the parameter was not present</returns>
        protected string GetParameter(string paramName)
        {
            if (Context.Parameters.ContainsKey(paramName))
            {
                string value = Context.Parameters[paramName];
                if (value == null) return null;
                return value;
            }
            return null;
        }

        /// <summary>
        /// Updates the SilentMode property based on the contents of the command line
        /// </summary>
        private void SetSilentMode()
        {
            SilentMode = false;
            if (GetParameter("mode") == "2")
            {
                SilentMode = true;
                Debug.WriteLine("Registration Installer detected UI Level SILENT");
            }
        }

        /// <summary>
        /// Fetches the Targets from the command line
        /// </summary>
        /// <returns>the array of target DLLs to register or unregister</returns>
        private string[] GetTargets()
        {
            try
            {
                string input = GetParameter("targets");
                string delim = GetParameter("delimiter");
                if (delim == null)
                {
                    delim = ";";
                }

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

        /// <summary>
        /// Fetches list of targets from an InstallState
        /// </summary>
        /// <param name="installState">installState to read</param>
        /// <returns>the list of targets in the InstallState, or null if none found</returns>
        private string[] GetSavedStateTargets(IDictionary installState)
        {
            if (installState == null) return null;

            if (installState.Contains(TargetsKey))
            {
                return installState[TargetsKey] as string[];
            }
            return null;
        }

        /// <summary>
        /// Cachces an array of targets in the InstallState
        /// </summary>
        /// <param name="stateSaver">installState to write to</param>
        /// <param name="targets">targets to store</param>
        private void SetSavedStateTargets(IDictionary stateSaver, string[] targets)
        {
            if (stateSaver.Contains(TargetsKey))
            {
                stateSaver[TargetsKey] = targets;
            }
            else
            {
                stateSaver.Add(TargetsKey, targets);
            }
        }

        #endregion

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                Debug.WriteLine("DLLHelper.Install");
                base.Install(stateSaver);
                SetSilentMode();

                string[] targets = GetTargets();
                if (targets == null)
                {
                    Debug.WriteLine("DLLHelper did not find any targets to register!");
                }
                else
                {
                    SetSavedStateTargets(stateSaver, targets);
                    RegisterDLL(targets);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DLLHelper.Install: " + ex.ToString());
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                Debug.WriteLine("DLLHelper.Rollback");
                base.Rollback(savedState);
                SetSilentMode();

                //command line overrides
                string[] targets = GetTargets();
                if (targets != null)
                {
                    Debug.WriteLine("DLLHelper.Rollback: targets list overridden by command line");
                    UnregisterDLL(targets);
                }

                targets = GetSavedStateTargets(savedState);
                if (targets != null)
                {
                    UnregisterDLL(targets);
                }
                else
                {
                    Debug.WriteLine("DLLHelper did not find any targets to unregister!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DLLHelper.Rollback: " + ex.ToString());
            }
        }

        public override void Commit(IDictionary savedState)
        {
            try
            {
                Debug.WriteLine("DLLHelper.Commit");
                base.Commit(savedState);
                SetSilentMode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DLLHelper.Commit: " + ex.ToString());
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                Debug.WriteLine("DLLHelper.Uninstall");
                base.Uninstall(savedState);
                SetSilentMode();

                string[] targets = GetSavedStateTargets(savedState);
                if (targets != null)
                {
                    UnregisterDLL(targets);
                }
                else
                {
                    targets = GetTargets(); //get from command line
                    if (targets != null)
                    {
                        UnregisterDLL(targets);
                    }
                    else
                    {
                        Debug.WriteLine("DLLHelper did not find any targets to register!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DLLHelper.Uninstall: " + ex.ToString());
            }
        }
    }
}
