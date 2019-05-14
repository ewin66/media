using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.DependencySetup
{
    [RunInstaller(true)]
    public partial class RegisterLeadTools : DLLHelper
    {
        public RegisterLeadTools()
        {
            InitializeComponent();
        }

        private string[] DLLs =
            {
            "DSKernel2.dll",
            "LCODCJ2K2.dll",
            "LCodcCMP2.dll",
            "LTMM15.dll",
            "LMNETCON2.dll",
            "LMNETSRC2.dll",
            "LMNETDMX2.dll",
            "LDecH2643.dll",
            "LMVTOVLY2.dll",
            "LMOggMux.dll",
            "LMOggSpl.dll",
            "LMVYUVxf.dll",
            "DGramProtocol.dll",
            };

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegisterDLL(DLLs);

            WriteAndExecuteRegIni();
        }

        /// <summary>
        /// This method generates a regini script (and executes it) to set the permissions for the LEADTOOLS Protocols.
        /// </summary>
        private void WriteAndExecuteRegIni()
        {
            FileStream f = null;
            StreamWriter s = null;
            try
            {
                string path = Path.GetTempFileName();

                Debug.WriteLine("Writing script to: " + path);

                f = new FileStream(path, FileMode.Create, FileAccess.Write);
                s = new StreamWriter(f);
                s.WriteLine(@"\registry\machine\software\classes");
                s.WriteLine(@"    CLSID");
                s.WriteLine(@"        {E2B7DE0C-38C5-11D5-91F6-00104BDB8FF9}");
                s.WriteLine(@"            Protocols [1 5 7 10 17 20]");
                s.WriteLine(@"                dgram [1 5 7 10 17 20]");
                s.WriteLine(@"                lmtcpip [1 5 7 10 17 20]");
                s.WriteLine(@"                mcast [1 5 7 10 17 20]");
                s.Close();
                s.Dispose();
                s = null;
                f.Close();
                f.Dispose();
                f = null;

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\regini.exe";
                psi.Arguments = "\"" + path + "\"";

                Debug.WriteLine(String.Format("Executing: {0} {1}", psi.FileName, psi.Arguments));
                Process regini = Process.Start(psi);
                regini.WaitForExit();

                Debug.WriteLine(String.Format("regini.exe returned {0}", regini.ExitCode));

                File.Delete(path);

                if (regini.ExitCode != 0)
                {
                    throw new Exception(String.Format("regini.exe returned {0}", regini.ExitCode));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(new Form() { TopMost = true }, "An error occurred while setting the LEADTOOLS registry permissions. You will have to set the permissions manually." + Environment.NewLine + Environment.NewLine + ex.Message, "Failure Setting LEADTOOLS Registry Permissions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorLogger.DumpToDebug(ex);
            }
            finally
            {
                if (s != null)
                {
                    s.Flush();
                    s.Close();
                    s.Dispose();
                    s = null;
                }
                if (f != null)
                {
                    f.Flush();
                    f.Close();
                    f.Dispose();
                    f = null;
                }
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            UnregisterDLL(DLLs);
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);
        }
    }
}
