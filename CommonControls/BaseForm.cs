using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class BaseForm : Form
    {
        private int red = 0;
        private int delta = 10;
        private System.Windows.Forms.Timer blinkTimer;
        private FCMessageBox dbConnectionError;

        public BaseForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;

            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 20;
            blinkTimer.Tick += new EventHandler(blinkTimer_Tick);            
        }

        private bool _isBlinkerEnabled = false;
        /// <summary>
        /// Get/set a value indicating if the form should blink
        /// </summary>
        public bool IsBlinkerEnabled
        {
            get
            {
                return _isBlinkerEnabled;
            }
            set
            {
                if (_isBlinkerEnabled == value)
                    return;

                _isBlinkerEnabled = value;

                // Call thread safe method to update blinker state
                UpdateBlinkerState();
            }
        }

        /// <summary>
        /// Shows the error message if set to true every time the blinker is enabled
        /// </summary>
        public bool IsErrorMessageEnabled
        {
            get
            {
                return isErrorMessageEnabled;
            }
            set
            {
                isErrorMessageEnabled = value;
            }
        }
        private bool isErrorMessageEnabled = true;
        

        public delegate void CallBack();
        private void UpdateBlinkerState()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new CallBack(UpdateBlinkerState));
            else
            {
                if (_isBlinkerEnabled)
                {
                    blinkTimer.Start();
                    ShowErrorMessage();
                }
            }
        }
        // Change the background color
        void blinkTimer_Tick(object sender, EventArgs e)
        {
            blinkTimer.Stop();

            if (_isBlinkerEnabled)
            {
                try
                {
                    red += delta;
                    BackColor = Color.FromArgb(red, 0, 0);
                    if (red > 245 || red < 10)
                        delta = -delta;
                }
                catch { }

                // Start the blink timer again
                blinkTimer.Start();
            }
            else
            {
                BackColor = Color.Black;
                red = 0;
                delta = 10;

                HideErrorMessage();
            }
        }

        protected virtual void ShowErrorMessage() {

            if (IsErrorMessageEnabled)
            {
                if (dbConnectionError == null || dbConnectionError.IsDisposed)
                {
                    dbConnectionError = new FCMessageBox("There is a problem connecting to your database.");
                    dbConnectionError.Text = "Connection Error";
                }
                dbConnectionError.ShowDialog(this);
            }
        }

        protected virtual void HideErrorMessage() {
            if (dbConnectionError != null && !dbConnectionError.IsDisposed)
            {
                dbConnectionError.Close();
                dbConnectionError.Dispose();
                dbConnectionError = null;
            }
        }
    }
}