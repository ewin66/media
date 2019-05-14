using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Settings;

namespace FutureConcepts.Media.CommonControls
{
    public partial class MainForm : BaseForm
    {
        protected FutureConcepts.Media.CommonControls.MoveWindow mw;
        public MainForm()
        {
            InitializeComponent();

//            if(SystemInfo.DoCheck())
            if (true)
                mw = new FutureConcepts.Media.CommonControls.MoveWindow(this, label1);
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
            CloseButtonClicked();
        }

        protected virtual void CloseButtonClicked()
        {
            this.Close();
            //this.Dispose();
        }

        public bool HasControlBox
        {
            get
            {
                return ControlBoxPanel.Visible;
            }
            set
            {
                ControlBoxPanel.Visible = value;                    
            }
        }

        public bool HasVersion
        {
            get
            {
                return lblVersion.Visible;
            }
            set
            {
                lblVersion.Visible = value;
                if (value)
                {
                    label1.TextAlign = ContentAlignment.TopCenter;
                }
                else
                {
                    label1.TextAlign = ContentAlignment.MiddleCenter;
                }
            }
        }

        public bool HasCloseButton
        {
            get { return CloseButton.Visible; }
            set { CloseButton.Visible = value; }
        }

        /// <summary>
        /// Defines whether the form can be dragged to a new position
        /// </summary>
        public bool CanMove
        {
            get { return mw != null; }
            set
            {
                if (value && mw == null)
                {
                    mw = new FutureConcepts.Media.CommonControls.MoveWindow(this, label1);
                }
                else if(!value && mw != null)
                {
                    mw.CleanUp();
                    mw = null;
                }
            }
        }

	
        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                MaximizeButton.ImageList = ilRestore;
                tt.SetToolTip(MaximizeButton, "Restore down");
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                MaximizeButton.ImageList = ilMaximize;
                tt.SetToolTip(MaximizeButton, "Maximize");
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = Application.ProductVersion;
        }
                
        public Font TitleFont
        {
            get { return label1.Font; }
            set { label1.Font = value; }
        }
	
    }
}