using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    public partial class HorizontalBitrateSelector : UserControl
    {
        public HorizontalBitrateSelector()
        {
            InitializeComponent();
        }

        [Category("Appearance"), Browsable(true)]
        public string Label
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }


        [Category("Behavior"), Browsable(true)]
        public int Value
        {
            get
            {
                return (int)updown.Value;
            }
            set
            {
                if (value < updown.Minimum)
                    value = (int)updown.Minimum;
                else if (value > updown.Maximum)
                    value = (int)updown.Maximum;

                updown.Value = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public int Minimum
        {
            get
            {
                return (int)updown.Minimum;
            }
            set
            {
                if (value == 0)
                    value = 1;

                trackbar.Minimum = BitrateToExponent(value);
                updown.Minimum = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public int Maximum
        {
            get
            {
                return (int)updown.Maximum;
            }
            set
            {
                if (value == 0)
                    value = 1;

                trackbar.Maximum = BitrateToExponent(value);
                updown.Maximum = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public new bool Enabled
        {
            get
            {
                return label.Enabled;
            }
            set
            {
                label.Enabled = value;
                trackbar.Enabled = value;
                updown.Enabled = value;
                units.Enabled = value;
            }
        }

        private void trackbar_Scroll(object sender, EventArgs e)
        {
            if ((trackbar == null) || (updown == null))
                return;

            if (RestrictToPowersOfTwo)
            {
                trackbar.CurrentValue = Math.Round(trackbar.CurrentValue, 0);
            }
            updown.Value = ExponentToBitrate(trackbar.CurrentValue);
        }

        private void updown_ValueChanged(object sender, EventArgs e)
        {
            if ((trackbar == null) || (updown == null))
                return;

            int newValue = (int)updown.Value;

            if (RestrictToPowersOfTwo)
            {
                newValue = ExponentToBitrate(Math.Round(BitrateToExponent(newValue), 0));
            }
            trackbar.Value = BitrateToExponent(newValue);
            FireValueChanged();
        }

        /// <summary>
        /// When using trackbars whose values are actually exponents for the powers of 2, this method finds "x" where
        /// 2^x = kbps.
        /// </summary>
        /// <param name="kbps">the answer to 2^x = kbps</param>
        /// <returns>the value of x such that 2^x = kbps</returns>
        protected double BitrateToExponent(int kbps)
        {
            if (kbps == 0)
                return 0;

            return Math.Log((double)kbps) / Math.Log(2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerOfTwo"></param>
        /// <returns></returns>
        protected int ExponentToBitrate(double powerOfTwo)
        {
            return (int)Math.Pow(2, powerOfTwo);
        }

        private void HorizontalBitrateSelector_Paint(object sender, PaintEventArgs e)
        {
            return;
        }

        /// <summary>
        /// This event is fired when the value changes.
        /// </summary>
        [Category("Action"), Browsable(true)]
        public event EventHandler ValueChanged;

        protected void FireValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }

        private bool _restrictToPowersOfTwo;
        /// <summary>
        /// Set this property to true to restrict to only powers of two.
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public bool RestrictToPowersOfTwo
        {
            get { return _restrictToPowersOfTwo; }
            set { _restrictToPowersOfTwo = value; }
        }

    }
}
