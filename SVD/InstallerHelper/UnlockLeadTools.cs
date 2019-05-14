using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FutureConcepts.Media.SVD.InstallerHelper
{
    [RunInstaller(true)]
    public partial class UnlockLeadTools : Installer
    {
        public UnlockLeadTools()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            FutureConcepts.Media.License.LeadTools.Unlock();
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            FutureConcepts.Media.License.LeadTools.Lock();
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