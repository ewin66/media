using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class LoadScreen : BaseForm
    {
        public LoadScreen()
        {
            InitializeComponent();
            this.LocationChanged += new EventHandler(LoadScreen_LocationChanged);
        }

        void LoadScreen_LocationChanged(object sender, EventArgs e)
        {
            Rectangle r = Screen.FromPoint(this.Location).Bounds;
            this.Location = new Point(r.X + (r.Width - this.Width) / 2, r.Y + (r.Height - this.Height) / 2);
        }

        private delegate void Destroy();
        public void DestroyNeeded()
        {
            if (this != null)
            {
                if (this.InvokeRequired)
                    this.Invoke(new Destroy(DestroyNeeded));
                else
                {
                    try
                    {
                        this.Close();
                        base.Close();
                    }
                    catch
                    {
                        // Do nothing because the screen is already destroyed
                        // This will happen if the screen is already destroyed
                        // before the event gets fired.
                    }
                }
            }
        }

        private delegate void SetText(string text);

        private void SetTextNeeded(string text)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetText(SetTextNeeded), text);
            else
                lblStatus.Text = text;
        }

        private void SetTitleNeeded(string title)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetText(SetTitleNeeded), title);
            else
            {
                lblTitle.Text = title;
            }
        }

        private delegate void SetPercent(int percent);

        private void SetPercentNeeded(int percent)
        {
            if (this.InvokeRequired)
                this.Invoke(new SetPercent(SetPercentNeeded), percent);
            else
                pb.Value = percent;
        }

        private delegate void SetVisibleCallBack(bool value);
        public void SetTopMost(bool value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetVisibleCallBack(SetTopMost), value);
            }
            else
            {
                this.TopMost = value;
                this.TopLevel = value;
                this.Refresh();
            }
        }

        public string StatusText
        {
            set { SetTextNeeded(value); }
        }

        public int StatusPercent
        {
            set { SetPercentNeeded(value); }
        }

        public string Title
        {
            set { SetTitleNeeded(value);}
        }
    }
}