using System;

using FutureConcepts.Media.Client;

using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD
{
    public partial class InitialSettingsDialog : OKCancelForm
    {
        public InitialSettingsDialog()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(TB_UserName.Text))
            {
                TB_UserName.Text = Environment.MachineName;
            }
        }

        public string UserName
        {
            get
            {
                return TB_UserName.Text;
            }
            set
            {
                TB_UserName.Text = value;
            }
        }
    }
}