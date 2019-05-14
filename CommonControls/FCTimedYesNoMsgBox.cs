using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class FCTimedYesNoMsgBox : FCYesNoMsgBox
    {
        public FCTimedYesNoMsgBox()
        {
            InitializeComponent();
        }

        private int _countDown = 30;

        private Timer _timer;

        private int _interval = 1000;

        public int Interval
        {
            set
            {
                _interval = value;
            }
            get
            {
                return _interval;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _timer = new Timer();
            _timer.Interval = _interval;
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Enabled = true;
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _countDown--;
            if (_countDown == 0)
            {
                _timer.Enabled = false;
                Dispose();
            }
            else
            {
                CancelButtonText = "No " + _countDown;
            }
        }
    }
}
