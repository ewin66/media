using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class MicrowaveFreqSweep : UserControl
    {
        public MicrowaveFreqSweep()
        {
            InitializeComponent();

            pbProgress.DisplayValue = false;
            pbProgress.Value = 0;
        }

        /// <summary>
        /// Raised when the user wants to start a sweep
        /// </summary>
        [Category("Action"), Description("Raised when the user wants to start a sweep.")]
        public event EventHandler StartSweep;

        /// <summary>
        /// Raised when the user wants to cancel a sweep
        /// </summary>
        [Category("Action"), Description("Raised when the user wants to cancel a sweep.")]
        public event EventHandler CancelSweep;

        /// <summary>
        /// Gets or sets the start frequency
        /// </summary>
        [Category("Parameters"), Description("The frequency where the sweep will begin.")]
        public int StartFrequency
        {
            get
            {
                return (int)udStartFreq.Value;
            }
            set
            {
                udStartFreq.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the end frequency
        /// </summary>
        [Category("Parameters"), Description("The frequency where the sweep will end.")]
        public int EndFrequency
        {
            get
            {
                return (int)udStopFreq.Value;
            }
            set
            {
                udStopFreq.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the end frequency
        /// </summary>
        [Category("Parameters"), Description("The minimum threshold of signal strength.")]
        public int Threshold
        {
            get
            {
                return (int)udThreshold.Value;
            }
            set
            {
                udThreshold.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the progress bar's value
        /// </summary>
        public int Progress
        {
            get
            {
                return pbProgress.Value;
            }
            set
            {
                pbProgress.Value = value;
            }
        }

        /// <summary>
        /// Sets the control in the "Sweep in proress" state
        /// </summary>
        public void SetSweepStarted()
        {
            btnStartCancel.Text = "Cancel";
            btnStartCancel.BackColor = Color.Red;

            udStartFreq.Enabled = false;
            udStopFreq.Enabled = false;
            udThreshold.Enabled = false;

            pbProgress.DisplayValue = true;
            pbProgress.Value = 0;
        }

        /// <summary>
        /// Sets the control in the "sweep complete" state
        /// </summary>
        public void SetSweepComplete()
        {
            btnStartCancel.Text = "Start";
            btnStartCancel.BackColor = Color.FromArgb(0, 96, 160);

            udStartFreq.Enabled = true;
            udStopFreq.Enabled = true;
            udThreshold.Enabled = true;

            pbProgress.DisplayValue = false;
            pbProgress.Value = 0;
        }


        private void btnStartCancel_Click(object sender, EventArgs e)
        {
            if (btnStartCancel.Text.Equals("Start"))
            {
                SetSweepStarted();

                if (StartSweep != null)
                {
                    StartSweep.Invoke(this, new EventArgs());
                }
            }
            else
            {
                SetSweepComplete();

                if (CancelSweep != null)
                {
                    CancelSweep.Invoke(this, new EventArgs());
                }
            }
        }

        private void udStartFreq_ValueChanged(object sender, EventArgs e)
        {
            if (udStartFreq.Value > udStopFreq.Value)
            {
                udStopFreq.Value = udStartFreq.Value;
            }
        }

        private void udStopFreq_ValueChanged(object sender, EventArgs e)
        {
            if (udStopFreq.Value < udStartFreq.Value)
            {
                udStartFreq.Value = udStopFreq.Value;
            }
        }

        public void Configure(MicrowaveCapabilities caps)
        {
            udStartFreq.Minimum = caps.MinimumFrequency / 1000000;
            udStartFreq.Maximum = caps.MaximumFrequency / 1000000;
            udStartFreq.Value = caps.MinimumFrequency / 1000000;

            udStopFreq.Minimum = caps.MinimumFrequency / 1000000;
            udStopFreq.Maximum = caps.MaximumFrequency / 1000000;
            udStopFreq.Value = caps.MaximumFrequency / 1000000;

            this.udThreshold.Minimum = 0;
            this.udThreshold.Maximum = (int)(caps.ReceivedCarrierLevelMaximum);
            this.udThreshold.Value = (int)(0.2 * caps.ReceivedCarrierLevelMaximum);
        }
    }
}
