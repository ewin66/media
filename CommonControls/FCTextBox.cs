using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace FutureConcepts.Media.CommonControls
{
    public partial class FCTextBox : TextBox, IEditableUserControl
    {
        public FCTextBox()
        {
            InitializeComponent();
            this.GotFocus += new EventHandler(FCTextBox_GotFocus);
            this.LostFocus += new EventHandler(FCTextBox_LostFocus);
        }

        void FCTextBox_LostFocus(object sender, EventArgs e)
        {
            if (highlight)
                FlipColor();
        }

        void FCTextBox_GotFocus(object sender, EventArgs e)
        {
            if (highlight)
                FlipColor();
        }

        private bool editable;
        public bool Editable
        {
            get { return editable; }
            set { 
                editable = value;                
                try
                {
                    SetReadOnly(value);
                }
                catch (Exception e)
                {
                    if (this.InvokeRequired == true)
                    {
                        System.Diagnostics.Debug.WriteLine("InvokeRequired: " + this.InvokeRequired);
                    }
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        private delegate void SettingReadOnly(bool value);
        private void SetReadOnly(bool value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SettingReadOnly(SetReadOnly), value);
            }
            else
            {
                this.BackColor = value ? Color.White : Color.Black;
                this.ForeColor = value ? Color.Black : Color.White;
                this.ReadOnly = !value;
            }
        }

        private bool highlight;
        public bool Highlight
        {
            get { return highlight; }
            set { highlight = value; }
        }

        private delegate void ColorCallback();
        private void FlipColor()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ColorCallback(FlipColor));
            }
            else
            {
                if (this.BackColor.Equals(Color.DarkSeaGreen))
                {
                    this.BackColor = Color.White;
                }
                else
                {
                    this.BackColor = Color.DarkSeaGreen;
                }
            }
        }

    }
}
