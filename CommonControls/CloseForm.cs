using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class CloseForm : BaseDialog
    {
        public CloseForm()
        {
            InitializeComponent();
        }

        private void B_OK_Click(object sender, EventArgs e)
        {
            HandleOKClick();
        }

        protected virtual void HandleOKClick()
        {
            this.Close();
            //this.Dispose();
        }

        public bool OKButtonEnabled
        {
            get { return B_OK.Enabled; }
            set { B_OK.Enabled = value; }
        }
        
        public string OKButtonText
        {
            get { return B_OK.Text; }
            set { B_OK.Text = value; }
        }
	

    }
}