using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Media;

namespace FutureConcepts.Media.Tools.CameraControlTester
{
    public partial class ModeConfig : Form
    {
        public ModeConfig()
        {
            InitializeComponent();
        }

        private void ModeConfig_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ServerAddress))
            {
            }
            if (string.IsNullOrEmpty(SourceName))
            {
                txtSourceName.Text = "Vid1";
            }
            if (cbPTZType.Items.Count == 0)
            {
                foreach (string s in Enum.GetNames(typeof(PTZType)))
                {
                    cbPTZType.Items.Add(s);
                }
            }
            if (cbPTZType.SelectedIndex == -1)
            {
                cbPTZType.SelectedIndex = 0;
            }
            if (string.IsNullOrEmpty(Address))
            {
                txtAddress.Text = "COM1";
            }
        }

        public enum Modes { Direct, Client };

        public Modes Mode
        {
            get
            {
                if (rb_client.Checked)
                {
                    return Modes.Client;
                }
                else
                {
                    return Modes.Direct;
                }
            }
        }

        public string ServerAddress
        {
            get
            {
                return txtServerAddress.Text;
            }
        }

        public string SourceName
        {
            get
            {
                return txtSourceName.Text;
            }
        }

        public PTZType PTZType
        {
            get
            {
                try
                {
                    return (PTZType)Enum.Parse(typeof(PTZType), cbPTZType.SelectedItem.ToString());
                }
                catch
                {
                    return PTZType.Null;
                }
            }
        }

        public string Address
        {
            get
            {
                return txtAddress.Text;
            }
            set
            {
                txtAddress.Text = value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void rb_mode_CheckedChanged(object sender, EventArgs e)
        {
            txtServerAddress.Enabled = (Mode == Modes.Client);
            txtSourceName.Enabled = (Mode == Modes.Client);
            cbPTZType.Enabled = (Mode == Modes.Direct);
            txtAddress.Enabled = (Mode == Modes.Direct);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
