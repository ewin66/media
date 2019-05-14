using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Diagnostics;
using System.Reflection;


namespace FutureConcepts.Media.Client.DependencySetup
{
    [RunInstaller(true)]
    public partial class DLLHelper : Installer
    {
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
            string filename = Directory.GetParent(Assembly.GetCallingAssembly().Location) + @"\" + relativePath;
            try
            {
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
                Debug.WriteLine("Failed to register " + filename);
                Debug.WriteLine(exc.Message);
                System.Windows.Forms.MessageBox.Show(exc.Message, "Exception occurred while registering " + relativePath);
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
                Process proc = Process.Start("regsvr32", cmdLine);
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
    }
}
