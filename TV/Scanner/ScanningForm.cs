using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Controls.AntaresX.AntaresXForms;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class ScanningForm : BaseForm
    {
        public ScanningForm()
        {
            InitializeComponent();
            Message = "Scanning for available channels...";
            Progress = 0;
            StopButtonText = "Cancel";
        }

        public event EventHandler StopClicked;

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (StopClicked != null)
            {
                StopClicked(this, new EventArgs());
            }
        }

        /// <summary>
        /// Set the progress
        /// </summary>
        public int Progress
        {
            set
            {
                if (value < 1)
                {
                    lblPleaseWait.Text = "Please Wait...";
                }
                else
                {
                    lblPleaseWait.Text = "Please Wait, " + value.ToString() + "% complete...";
                }
            }
        }

        /// <summary>
        /// Message to display during channel scanning
        /// </summary>
        public string Message
        {
            get
            {
                return lblMessage.Text;
            }
            set
            {
                lblMessage.Text = value;
            }
        }

        /// <summary>
        /// Get/Set the text on the stop button
        /// </summary>
        public string StopButtonText
        {
            get
            {
                return btnStop.Text;
            }
            set
            {
                btnStop.Text = value;
            }
        }
    }
}