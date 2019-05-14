using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Controls.AntaresX.AntaresXControls.Controls;


namespace FutureConcepts.Media.TV.Scanner
{
    public partial class ChannelKeypad : DefaultUserControl
    {

        public ChannelKeypad()
        {
            InitializeComponent();
        }

        #region API

        [Category("Action"), Browsable(true), Description("Fired when the user clicks Enter, or has otherwise finished entering a channel")]
        public event EventHandler ChannelEntered;

        [Category("Action"), Browsable(true), Description("Fired when the user is in the process of entering a channel.")]
        public event EventHandler ChannelChanging;

        /// <summary>
        /// The last entered channel. The PhysicalChannel will always be -1. Will be null if nothing was ever entered, or an error occurred.
        /// </summary>
        [Browsable(false)]
        public Channel Channel
        {
            get;
            set;
        }

        /// <summary>
        /// Dispatches a ChannelChanging event, if anything is listening.
        /// </summary>
        private void FireChannelChanging()
        {
            if (ChannelChanging == null)
            {
                return;
            }

            Channel = ParseCurrentChannel();

            ChannelChanging(this, new EventArgs());
        }

        /// <summary>
        /// Dispatches a ChannelEntered event, if anything is listening
        /// </summary>
        private void FireChannelEntered()
        {
            inputTimeoutTimer.Stop();
            inputTimeoutTimer.Enabled = false;

            //if no listeners are attached, then there's no point in doing this stuff.
            if (ChannelEntered == null)
            {
                return;
            }

            Channel = ParseCurrentChannel();
            Accumulator = null;

            ChannelEntered(this, new EventArgs());
        }

        /// <summary>
        /// Sends a numeric key press to the Channel Keypad
        /// </summary>
        /// <param name="i">number indicated</param>
        public void SendKey(int i)
        {
            SendKey(i.ToString());
        }

        /// <summary>
        /// Sends a textual key press to the Channel Keypad
        /// </summary>
        /// <param name="s">the string indicated</param>
        public void SendKey(string s)
        {
            if (inputTimeoutTimer.Enabled)
            {
                inputTimeoutTimer.Stop();
            }

            if (s.Equals(Environment.NewLine))
            {
                FireChannelEntered();
                return;
            }

            if (!s.Equals("."))
            {
                Accumulator += s;
            }
            else if (!Accumulator.Contains("."))
            {
                Accumulator += s;
            }

            FireChannelChanging();

            if (!inputTimeoutTimer.Enabled)
            {
                inputTimeoutTimer.Enabled = true;
            }
            inputTimeoutTimer.Start();
        }

        #endregion

        #region Backend

        private string _accumulator;
        /// <summary>
        /// Used to accumulate characters until "enter" happens.
        /// </summary>
        private string Accumulator
        {
            get
            {
                if (_accumulator == null)
                {
                    _accumulator = "";
                }
                return _accumulator;
            }
            set
            {
                _accumulator = value;
            }
        }

        /// <summary>
        /// If the input timeout timer elapses, then they have entered a channel
        /// </summary>
        private void inputTimeoutTimer_Tick(object sender, EventArgs e)
        {
            FireChannelEntered();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            FireChannelEntered();
        }

        /// <summary>
        /// Returns the current representation of what is currenlty in the Accumulator
        /// </summary>
        /// <returns>returns a Channel object, or null if an error occurs</returns>
        private Channel ParseCurrentChannel()
        {
            string[] parts = Accumulator.Split('.');
            if (parts.Length == 1)
            {
                int major;
                if (!Int32.TryParse(parts[0], out major))
                {
                    return null;
                }
                return new Channel(-1, major, -1);
            }
            else if (parts.Length == 2)
            {
                int major, minor;
                if (!Int32.TryParse(parts[0], out major))
                {
                    //this segment of code allows a short cut.
                    //If they press ".", then it will use the current major channel, and specify a new subchannel.
                    if (Channel != null) 
                    {
                        Accumulator = Channel.MajorChannel.ToString() + Accumulator;
                        return ParseCurrentChannel();
                    }
                    else
                    {
                        return null;
                    }
                }
                if (!Int32.TryParse(parts[1], out minor))
                {
                    return new Channel(-1, major, 0);
                }
                return new Channel(-1, major, minor);
            }
            else //invalid format was entered somehow
            {
                return null;
            }
        }

        /// <summary>
        /// Handles the click of a "digit button"
        /// </summary>
        private void btnDigit_MouseDown(object sender, MouseEventArgs e)
        {
            Control btn = sender as Control;
            SendKey(btn.Text);
        }

        #endregion


    }
}
