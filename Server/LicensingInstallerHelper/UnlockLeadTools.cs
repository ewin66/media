using System.ComponentModel;
using System.Configuration.Install;

namespace FutureConcepts.Media.Server.LicensingInstallerHelper
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