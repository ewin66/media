using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class BaseDialog : BaseForm
    {
        public BaseDialog()
        {
            InitializeComponent();
            MoveWindow mw = new MoveWindow(this, label1);
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (this.label1 != null)
                {
                    this.label1.Text = value;
                }
                base.Text = value;
            }
        }
        protected void CloseButton_Click(object sender, EventArgs e)
        {

        }

        protected virtual void CloseButtonClicked()
        {
            this.Close();
            //this.Dispose();
        }

        private bool hasControlBox;
        public bool HasControlBox
        {
            get { return hasControlBox; }
            set { hasControlBox = value; }
        }

        private bool hasVersion;
        public bool HasVersion
        {
            get { return hasVersion; }
            set { hasVersion = value; }
        }

        private bool hasCloseButton;
        public bool HasCloseButton
        {
            get { return hasCloseButton; }
            set { hasCloseButton = value; }
        }

    }
}