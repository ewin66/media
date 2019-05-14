using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace FutureConcepts.Media.Server
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        /// <summary>
        /// The name to reference the service by
        /// </summary>
        public const string MediaServerServiceName = "AntaresX Media Server";

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            RegistryKey k = null;
            try
            {
                if (Environment.OSVersion.Version.Major != 5)
                {
                    throw new Exception("This method has only been tested for Windows XP. Please set Recovery options manually.");
                }

                k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + MediaServerServiceName, true);
                k.SetValue("FailureActions",
                           new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                                        0x53, 0x00, 0x65, 0x00, 0x01, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00 },
                           RegistryValueKind.Binary);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Could not set the Recovery options for " +
                                                     MediaServerServiceName +
                                                     Environment.NewLine + Environment.NewLine + ex.Message,
                                                     "Error setting Recovery options!",
                                                     System.Windows.Forms.MessageBoxButtons.OK,
                                                     System.Windows.Forms.MessageBoxIcon.Warning);
            }
            finally
            {
                if (k != null)
                {
                    k.Close();
                }
            }
        }
    }
}
