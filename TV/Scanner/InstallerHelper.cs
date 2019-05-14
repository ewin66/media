using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

using FutureConcepts.Media.Client.InstallerHelper;


namespace FutureConcepts.Media.TV.Scanner
{
    [RunInstaller(true)]
    public partial class InstallerHelper : DLLHelper
    {
        public InstallerHelper()
        {
            InitializeComponent();
        }

        private static readonly string[] tvDlls = {
                                                    "DSKernel2.dll",
                                                    "LMNetDmx2.dll",
                                                    "LMNetSrc2.dll",
                                                    "LMVTOvlty2.dll",
                                                    "DUMP 1.ax",
                                                    "hcwCCnv2.ax"
                                                  };

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            RegisterDLL(tvDlls);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            UnregisterDLL(tvDlls);
        }
    }
}
