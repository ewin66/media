using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class OKCancelForm : BaseDialog
    {
        public OKCancelForm()
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

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            HandleCancelClick();
        }

        protected virtual void HandleCancelClick()
        {
            this.Close();
            //this.Dispose();
        }

        public bool OKButtonEnabled
        {
            get { return B_OK.Enabled; }
            set { B_OK.Enabled = value; }
        }

        public bool CancelButtonEnabled
        {
            get { return B_Cancel.Enabled; }
            set { B_Cancel.Enabled = value; }
        }

        public string OKButtonText
        {
            get { return this.B_OK.Text; }
            set { this.B_OK.Text = value; }
        }

        public string CancelButtonText
        {
            get { return this.B_Cancel.Text; }
            set { this.B_Cancel.Text = value; }
        }

        public DialogResult OKButtonDialogResult
        {
            get { return this.B_OK.DialogResult; }
            set { this.B_OK.DialogResult = value; }
        }

        public DialogResult CancelButtonDialogResult
        {
            get{return this.B_Cancel.DialogResult;}
            set{this.B_Cancel.DialogResult = value;}
        }
    }
}

