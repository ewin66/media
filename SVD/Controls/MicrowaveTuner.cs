using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FutureConcepts.Tools;
using System.Diagnostics;
using FutureConcepts.Media.Contract;
using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class MicrowaveTuner : UserControl
    {
        private Timer upDownArrowTimeout;
        private Timer typingTimeout;

        private Color LEDGood = Color.Lime;
        private Color LEDFail = Color.Red;
        private Color FCGreen = Color.FromArgb(24, 140, 79);
        private Color FCBlue = Color.FromArgb(0, 96, 160);

        private Dictionary<MicrowaveLinkQuality.Parameters, string> ttFormat;
        private Dictionary<MicrowaveLinkQuality.Parameters, Control> ttBinding;

        public MicrowaveTuner()
        {
            InitializeComponent();

            ttFormat = new Dictionary<MicrowaveLinkQuality.Parameters, string>();
            ttBinding = new Dictionary<MicrowaveLinkQuality.Parameters, Control>();

            ttFormat.Add(MicrowaveLinkQuality.Parameters.TunerLocked, "Tuner is{0} locked.");
            ttFormat.Add(MicrowaveLinkQuality.Parameters.DemodulatorLocked, "Demodulator is{0} locked.");
            ttFormat.Add(MicrowaveLinkQuality.Parameters.TransportStreamLocked, "Transport Stream is{0} locked.");
            ttFormat.Add(MicrowaveLinkQuality.Parameters.FECLocked, "Error Correction is{0} locked.");
            ttFormat.Add(MicrowaveLinkQuality.Parameters.DecoderLocked, "Video Decoder is{0} locked.");

            ttBinding.Add(MicrowaveLinkQuality.Parameters.TunerLocked, led_tuner);
            ttBinding.Add(MicrowaveLinkQuality.Parameters.DemodulatorLocked, led_demod);
            ttBinding.Add(MicrowaveLinkQuality.Parameters.TransportStreamLocked, led_mpeg);
            ttBinding.Add(MicrowaveLinkQuality.Parameters.FECLocked, led_fec);
            ttBinding.Add(MicrowaveLinkQuality.Parameters.DecoderLocked, led_decoder);

            upDownArrowTimeout = new Timer();
            upDownArrowTimeout.Interval = 3000;
            upDownArrowTimeout.Tick += new EventHandler(upDownArrowTimeout_Tick);

            typingTimeout = new Timer();
            typingTimeout.Interval = 6000;
            typingTimeout.Tick += new EventHandler(typingTimeout_Tick);
        }

        #region Capabilities Implementation

        /// <summary>
        /// Configures the control to display the capabilities needed
        /// </summary>
        /// <param name="caps"></param>
        public void Configure(MicrowaveCapabilities caps)
        {
            this.ChangingSelf = true;

            udFrequency.Minimum = caps.MinimumFrequency / (decimal)1000000.0;
            udFrequency.Maximum = caps.MaximumFrequency / (decimal)1000000.0;

            if (caps.IsSet(MicrowaveLinkQuality.Parameters.SignalToNoiseRatio))
            {
                pbSNR.Visible = true;
                lblSNR.Visible = true;
                pbSNR.Minimum = 0;
                pbSNR.Maximum = (int)caps.SignalToNoiseRatioMaximum;
            }
            else
            {
                pbSNR.Visible = false;
                lblSNR.Visible = false;
            }

            if (caps.IsSet(MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel))
            {
                pbRSSI.Visible = true;
                lblRSSI.Visible = true;
                pbRSSI.Minimum = (int)caps.ReceivedCarrierLevelMinimum;
                pbRSSI.Maximum = (int)caps.ReceivedCarrierLevelMaximum;

            }
            else
            {
                pbRSSI.Visible = false;
                lblRSSI.Visible = false;
            }

            bool anyLEDSupported = false;
            foreach (KeyValuePair<MicrowaveLinkQuality.Parameters, Control> paramLED in ttBinding)
            {
                paramLED.Value.Enabled = caps.IsSet(paramLED.Key);
                if (caps.IsSet(paramLED.Key))
                {
                    anyLEDSupported = true;
                }
            }

            //if no LEDs are supported, and Encryption is not supported, then we have no use for the side bar
            if (anyLEDSupported && !caps.IsSet(MicrowaveTuning.Parameters.Encryption))
            {
                pLEDs.Visible = false;
            }
            else
            {
                pLEDs.Visible = true;
            }

            ico_EncryptionActive.Visible = false;

            this.ChangingSelf = false;
        }

        #endregion 

        #region Public API

        private bool ChangingSelf { get; set; }

        /// <summary>
        /// Raised when the user has changed the frequency
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the user has changed the frequency.")]
        public event EventHandler FrequencyChanged;

        private void FireFrequencyChanged()
        {
            _inputViaTyping = false; //reset
            if (FrequencyChanged != null)
            {
                FrequencyChanged.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// This value should only be get when the FrequencyChanged event is raised.
        /// Specified in Hz
        /// This value should only be set by announcements from the microwave controller device
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Gets or sets the frequency.")]
        public UInt64 Frequency
        {
            get
            {
                return (UInt64)(udFrequency.Value * 1000000);
            }
            set
            {
                try
                {
                    ChangingSelf = true;
                    
                    udFrequency.Value = value / (decimal)1000000.0;
                    udFrequency.ForeColor = Color.LimeGreen;
                }
                catch
                {
                    udFrequency.ForeColor = Color.Red;
                }
                finally
                {
                    ChangingSelf = false;
                }
            }
        }

        private UInt64 _lastTentativeFrequency;
        /// <summary>
        /// Setting this property makes the UI indicate that a frequency change is pending
        /// </summary>
        [Browsable(false)]
        public UInt64 TentativeFrequency
        {
            get
            {
                return _lastTentativeFrequency;
            }
            set
            {
                try
                {
                    ChangingSelf = true;
                    _lastTentativeFrequency = value;
                    decimal valueMHz = value / (decimal)1000000.0;
                    if (udFrequency.Value != valueMHz)
                    {
                        udFrequency.Value = valueMHz;
                    }
                    udFrequency.ForeColor = Color.Yellow;
                }
                catch
                {
                    udFrequency.ForeColor = Color.Red;
                }
                finally
                {
                    ChangingSelf = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value displayed for the current signal strength
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("The currently displayed RSSI")]
        public double ReceivedCarrierLevel
        {
            get
            {
                return pbRSSI.Value;
            }
            set
            {
                pbRSSI.Value = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the value displayed for the current signal strength
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("The currently displayed SNR")]
        public double SignalToNoiseRatio
        {
            get
            {
                return pbSNR.Value;
            }
            set
            {
                pbSNR.Value = (int)value;
            }
        }



        #endregion

        #region GUI / Interaction

        void typingTimeout_Tick(object sender, EventArgs e)
        {
            if (this.InputEnabled)
            {
                System.Diagnostics.Debug.WriteLine("typingTimeout : FrequencyChanged");
                StopTimeouts();
                udFrequency.Validate();
                TentativeFrequency = (UInt64)(udFrequency.Value * 1000000);
                StopTimeouts();
                FireFrequencyChanged();
            }
            else
            {
                StopTimeouts();
            }
        }

        private void StopTimeouts()
        {
            Debug.WriteLine("upDownArrow / typing STOP");
            upDownArrowTimeout.Stop();
            typingTimeout.Stop();
        }

        void upDownArrowTimeout_Tick(object sender, EventArgs e)
        {
            if (this.InputEnabled)
            {
                System.Diagnostics.Debug.WriteLine("upDownArrowTimeout : FrequencyChanged");
                StopTimeouts();
                TentativeFrequency = (UInt64)(udFrequency.Value * 1000000);
                StopTimeouts();
                FireFrequencyChanged();
            }
            else
            {
                StopTimeouts();
            }
        }

        private void udFrequency_ValueChanged(object sender, EventArgs e)
        {
            if (ChangingSelf)
            {
                return;
            }

            udFrequency.ForeColor = Color.White;

            StopTimeouts();
            Debug.WriteLine("upDownArrowTimeout START");
            upDownArrowTimeout.Start();

            Debug.WriteLine("_inputViaTyping = " + _inputViaTyping);
            if (!_inputViaTyping)
            {
                decimal cur = udFrequency.Value;
                decimal frac = cur - (UInt64)cur;
                if (frac != (decimal)0.0)
                {
                    Debug.WriteLine("upDownArrow truncate fraction");
                    udFrequency.Value = (decimal)(UInt64)cur;
                }
            }
            _inputViaTyping = false;
        }

        private void udFrequency_Enter(object sender, EventArgs e)
        {
        //    Debug.WriteLine("udFrequency_Enter: typingTimeout RESET");
        //    typingTimeout.Stop();
        //    typingTimeout.Start();
        }

        private bool _inputViaTyping = false;

        private void udFrequency_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("udFrequency_KeyPress: typingTimeout RESET");
            udFrequency.ForeColor = Color.White;

            _inputViaTyping = true;

            typingTimeout.Stop();
            typingTimeout.Start();
        }

        private void udFrequency_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("udFrequency_Leave : FrequencyChanged");
            StopTimeouts();
            UInt64 inputFreq = (UInt64)(udFrequency.Value * 1000000);
            if(inputFreq != TentativeFrequency)
            {
                TentativeFrequency = inputFreq;
                StopTimeouts();
                FireFrequencyChanged();
            }
        }

        #endregion

        public void UpdateTuning(MicrowaveTuning t)
        {
            this.ChangingSelf = true;
            

            if (t.IsSet(MicrowaveTuning.Parameters.Frequency))
            {
                this.Frequency = t.Frequency;
                _lastTentativeFrequency = t.Frequency;
            }

            ico_EncryptionActive.Visible = false;
            if (t.IsSet(MicrowaveTuning.Parameters.Encryption))
            {
                if (t.Encryption != null)
                {
                    if ((t.Encryption.Type != EncryptionType.None) && (t.Encryption.Type != EncryptionType.Unknown))
                    {
                        ico_EncryptionActive.Visible = true;
                        tt.SetToolTip(ico_EncryptionActive, "Encryption Active (" + t.Encryption.Type.ToDisplayString() + " " + t.Encryption.KeyLength + " bit)");
                    }
                }
            }

            this.ChangingSelf = false;
        }

        public void UpdateLinkQuality(MicrowaveLinkQuality lq)
        {
            this.ChangingSelf = true;

            if (lq.IsSet(MicrowaveLinkQuality.Parameters.SignalToNoiseRatio))
            {
                this.SignalToNoiseRatio = lq.SignalToNoiseRatio;
            }
            if (lq.IsSet(MicrowaveLinkQuality.Parameters.ReceivedCarrierLevel))
            {
                this.ReceivedCarrierLevel = lq.ReceivedCarrierLevel;
            }

            foreach (KeyValuePair<MicrowaveLinkQuality.Parameters, Control> kvp in ttBinding)
            {
                if (lq.IsSet(kvp.Key))
                {
                    SetLED(lq, kvp.Key, (LED)kvp.Value);
                }
            }

            this.ChangingSelf = false;
        }

        private void SetLED(MicrowaveLinkQuality lq, MicrowaveLinkQuality.Parameters parameter, LED led)
        {
            bool on = false;
            switch (parameter)
            {
                case MicrowaveLinkQuality.Parameters.TunerLocked:
                    on = lq.TunerLocked;
                    break;
                case MicrowaveLinkQuality.Parameters.DemodulatorLocked:
                    on = lq.DemodulatorLocked;
                    break;
                case MicrowaveLinkQuality.Parameters.TransportStreamLocked:
                    on = lq.TransportStreamLocked;
                    break;
                case MicrowaveLinkQuality.Parameters.FECLocked:
                    on = lq.FECLocked;
                    break;
                case MicrowaveLinkQuality.Parameters.DecoderLocked:
                    on = lq.DecoderLocked;
                    break;
                default:
                    return;
            }

            led.LEDColor = on ? LEDGood : LEDFail;
            tt.SetToolTip(led, string.Format(ttFormat[parameter], (on ? string.Empty : " NOT")));
        }

        private void btnEnterKey_Click(object sender, EventArgs e)
        {
            if (EnterEncryptionKey != null)
            {
                EnterEncryptionKey(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised when the user wants to input a key
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the user has clicked the input key button.")]
        public event EventHandler EnterEncryptionKey;

        private bool _encKeyActive = false;

        public bool EncryptionKeyActive
        {
            get
            {
                return _encKeyActive;
            }
            set
            {
                _encKeyActive = value;
                if (value)
                {
                    btnEnterKey.BackColor = FCGreen;
                    tt.SetToolTip(btnEnterKey, "Encryption is Active. Click to change.");
                }
                else
                {
                    btnEnterKey.BackColor = FCBlue;
                    tt.SetToolTip(btnEnterKey, "Encryption is disabled. Click to change.");
                }
            }
        }

        private bool _inputEnabled = true;
        public bool InputEnabled
        {
            get
            {
                return _inputEnabled;
            }
            set
            {
                _inputEnabled = value;
                
                StopTimeouts();
            }
        }
    }
}
