using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.TV.Scanner.RemoteControls
{
    public partial class HCWRemote : UserControl
    {
        const int HCWPVR2 = 0x001E;     // 45-Button Remote
        const int HCWPVR = 0x001F;      // 34-Button Remote
        const int HCWCLASSIC = 0x0000;  // 21-Button Remote
        const int WM_TIMER = 0x0113;

        public HCWRemote()
        {
            InitializeComponent();

            string configButtonRelease = ConfigurationManager.AppSettings["HCWButtonRelease"];
            if (configButtonRelease != null)
            {
                _buttonRelease = TimeSpan.FromMilliseconds(Convert.ToInt32(configButtonRelease));
            }
            string configRepeatFilter = ConfigurationManager.AppSettings["HCWRepeatFilter"];
            if (configRepeatFilter != null)
            {
                _repeatFilter = Convert.ToInt32(configRepeatFilter);
            }
            string configRepeatSpeed = ConfigurationManager.AppSettings["HCWRepeatSpeed"];
            if (configRepeatSpeed != null)
            {
                _repeatSpeed = Convert.ToInt32(configRepeatSpeed);
            }
            string configFilterDoubleClicks = ConfigurationManager.AppSettings["HCWFilterDoubleClicks"];
            if (configFilterDoubleClicks != null)
            {
                _filterDoubleClicks = Convert.ToBoolean(configFilterDoubleClicks);
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources && (components != null))
            {
                components.Dispose();
            }
            HCWNative.IR_Close(this.Handle, 0);
            base.Dispose(disposeManagedResources);
        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        private void NotifyKeyPressed(KeyName keyName)
        {
            if (KeyPressed != null)
            {
                KeyPressedEventArgs e = new KeyPressedEventArgs();
                e.PressedKeyName = keyName;
                e.AttackTime = DateTime.Now;
                KeyPressed(this, e);
            }
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WM_TIMER:
                    ProcessSystemKeyCode();
                    break;
            }
            base.WndProc(ref msg);
        }

        private int _lastKeyCode = -1;
        private DateTime _lastTime = DateTime.Now;
        private int _sameKeyCodeCount = 0;
        private int _lastExecutedKeyCodeCount = 0;
        private TimeSpan _buttonRelease = TimeSpan.FromMilliseconds(400);
        private int _repeatFilter = 2;
        private int _repeatSpeed = 0;
        private bool _filterDoubleClicks = false;

        private void ProcessSystemKeyCode()
        {
            int repeatCount = 0;
            int remoteCode = 0;
            int keyCode = 0;
            // Time of button press - Use this for repeat delay calculations
            DateTime sentTime = DateTime.Now;

            if (!HCWNative.IR_GetSystemKeyCode(out repeatCount, out remoteCode, out keyCode))
            {
                return;
            }
            if (_lastKeyCode == keyCode)
            {
                // button release time elapsed since last identical command
                // if so, reset counter & start new session
                if ((sentTime - _lastTime) > _buttonRelease)
                {
                    _sameKeyCodeCount = 0;   // new session with this button
                }
                else
                {
                    _sameKeyCodeCount++;   // button release time not elapsed
                }
            }
            else
            {
                _sameKeyCodeCount = 0;   // we got a new button
            }
            bool executeKey = false;

            // new button / session
            if (_sameKeyCodeCount == 0)
            {
                executeKey = true;
            }
            //// we got the identical button often enough to accept it
            if (_sameKeyCodeCount == _repeatFilter)
            {
                executeKey = true;
            }
            // we got the identical button accepted and still pressed, repeat with repeatSpeed
            if ((_sameKeyCodeCount > _repeatFilter) && (_sameKeyCodeCount > _lastExecutedKeyCodeCount + _repeatSpeed))
            {
                executeKey = true;
            }
            // double click filter
            if (executeKey && _filterDoubleClicks)
            {
                if ((_sameKeyCodeCount > 0) &&
                    (keyCode == 46 || //46 = fullscreen/green button
                    keyCode == 37 ||  //37 = OK button
                    keyCode == 56 ||  //56 = yellow button
                    keyCode == 11 ||  //11 = red button
                    keyCode == 41 ||  //41 = blue button
                    keyCode == 13 ||  //13 = menu button
                    keyCode == 15 ||  //15 = mute button
                    keyCode == 48))   //48 = pause button
                {
                    executeKey = false;
                }
            }
            if (executeKey)
            {
                _lastExecutedKeyCodeCount = _sameKeyCodeCount;
                _lastKeyCode = keyCode;
                NotifyKeyPressed((KeyName)keyCode);
            }
            _lastTime = sentTime;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int repeatCount;
            int remoteCode;
            int keyCode;
            HCWNative.IR_Open(this.Handle, 0, false, 0);
            // ignore any initial remote commands
            while (HCWNative.IR_GetSystemKeyCode(out repeatCount, out remoteCode, out keyCode))
            {
            }
        }
    }
}
