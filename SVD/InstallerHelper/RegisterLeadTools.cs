using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using FutureConcepts.Media.Client.InstallerHelper;

namespace FutureConcepts.Media.SVD.InstallerHelper
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
            "LDecH2643.dll",
            "LDECSCR2.dll",
            "LCODCJ2K2.dll",
            "LMNETCON2.dll",
            "LMNETDMX2.dll",
            "LMNETSRC2.dll",
            "LMVFRAMCTRL2.dll",
            "LMVTOVLY2.dll",
            "LTMM15.dll",
            "LMOggMux.dll",
            "MCastProtocol.dll",
            "DGramProtocol.dll",
            "GMFBridge.dll",
            "DVRWriter.dll"
            };

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegisterDLL(DLLs);
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