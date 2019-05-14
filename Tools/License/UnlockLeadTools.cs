using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace FutureConcepts.Media.Tools.License.LicensingInstallerHelper
{
    /// <summary>
    /// This class is an Installer class to invoke 
    /// </summary>
    [RunInstaller(true)]
    public partial class UnlockLeadTools : Installer
    {
        public UnlockLeadTools()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            Debug.WriteLine("UnlockLeadTools.Install");
            base.Install(stateSaver);
            LeadTools.Unlock();
            ElecardHelper.Unlock();
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            Debug.WriteLine("UnlockLeadTools.Uninstall");
            base.Uninstall(savedState);
            LeadTools.Lock();
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            Debug.WriteLine("UnlockLeadTools.Rollback");
            base.Rollback(savedState);
            LeadTools.Lock();
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            Debug.WriteLine("UnlockLeadTools.Commit");
            base.Commit(savedState);
        }
   }
}