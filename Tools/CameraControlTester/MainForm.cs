using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using FutureConcepts.Tools;
using FutureConcepts.Media.CameraControls;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Tools.CameraControlTester
{
    public partial class MainForm : Form
    {
        private ModeConfig config = new ModeConfig();

        private Client.CameraControl serverClient = null;
        private DirectCameraControl pluginClient = null;

        private ICameraControl currentClient = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            config.ShowInTaskbar = true;
            if (config.ShowDialog() == DialogResult.OK)
            {
                config.ShowInTaskbar = false;
                btnStartStop.Text = "Stop";
                btnStartStop_MouseClick();

                InitInterface();

                btnPelcoDAddressSweep.Enabled = (config.PTZType == PTZType.PelcoD);
            }
            else
            {
                this.Close();
                Application.Exit();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearInterface();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (config.ShowDialog() == DialogResult.OK)
            {

                btnStartStop.Text = "Stop";
                btnStartStop_MouseClick();

                InitInterface();

                btnPelcoDAddressSweep.Enabled = (config.PTZType == PTZType.PelcoD);
            }
        }

        #region Interface Init/Clear Start/Stop

        /// <summary>
        /// Creates the correct object based on the config
        /// </summary>
        private void InitInterface()
        {
            this.Cursor = Cursors.AppStarting;
            if (config.Mode == ModeConfig.Modes.Client)
            {
                try
                {
                    pluginClient = null;
                    serverClient = new FutureConcepts.Media.Client.CameraControl(config.ServerAddress);
                    currentClient = serverClient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Server Client failed.");
                }
            }
            else if (config.Mode == ModeConfig.Modes.Direct)
            {
                try
                {
                    serverClient = null;
                    pluginClient = new DirectCameraControl();
                    currentClient = pluginClient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Plugin Client failed.");
                }
            }
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Initiates a click on the Start/Stop button, using the left mouse button
        /// </summary>
        private void btnStartStop_MouseClick()
        {
            btnStartStop_MouseClick(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private DialogResult promptForInitialize = DialogResult.Cancel;

        private void btnStartStop_MouseClick(object sender, MouseEventArgs e)
        {
            if (btnStartStop.Text.Contains("Start"))
            {
                ClearInterface();
                InitInterface();
                this.Cursor = Cursors.WaitCursor;

                btnStartStop.BackColor = Color.Red;
                btnStartStop.Text = "Stop";

                if (serverClient != null)
                {
                    btnStartStop.BackColor = Color.Orange;
                    btnStartStop.Text = "Connecting... (Stop)";
                    SetEnabled(false);

                    try
                    {
                        serverClient.Opened += new EventHandler(cameraControlClient_Opened);
                        serverClient.Open(new ClientConnectRequest(config.SourceName));
                        serverClient.PropertyChanged += new PropertyChangedEventHandler(currentClient_PropertyChanged);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.DumpToDebug(ex);
                        MessageBox.Show(ex.ToString(), "Error opening camera control client!");
                        btnStartStop_MouseClick();
                    }
                }
                else if (pluginClient != null)
                {
                    btnStartStop.BackColor = Color.Red;
                    btnStartStop.Text = "Stop";

                    try
                    {
                        CameraControlInfo cameraConfig = new CameraControlInfo();
                        cameraConfig.Address = config.Address;
                        cameraConfig.PTZType = config.PTZType;

                        cameraConfig.Capabilities = new CameraCapabilitiesAndLimits();
                        cameraConfig.Capabilities.HasDigitalZoom = true;
                        cameraConfig.Capabilities.HasFocus = true;
                        cameraConfig.Capabilities.HasPan = true;
                        cameraConfig.Capabilities.HasTilt = true;
                        cameraConfig.Capabilities.HasZoom = true;

                        switch (config.PTZType)
                        {
                            case PTZType.Visca:
                                cameraConfig.Capabilities.HasEmitter = true;
                                cameraConfig.Capabilities.HasInfrared = true;
                                cameraConfig.Capabilities.HasInverter = true;
                                cameraConfig.Capabilities.HasStabilizer = true;
                                cameraConfig.Capabilities.HasWiper = true;
                                cameraConfig.Capabilities.TiltMaxAngle = 90;
                                cameraConfig.Capabilities.TiltMinAngle = -37;
                                cameraConfig.Capabilities.ZoomMinLevel = 1;
                                cameraConfig.Capabilities.ZoomMaxLevel = 37;
                                break;
                            case PTZType.PelcoD:
                                cameraConfig.Capabilities.HasEmitter = false;
                                cameraConfig.Capabilities.HasInfrared = false;
                                cameraConfig.Capabilities.HasInverter = false;
                                cameraConfig.Capabilities.HasStabilizer = false;
                                cameraConfig.Capabilities.HasWiper = false;
                                cameraConfig.Capabilities.TiltMaxAngle = 90;
                                cameraConfig.Capabilities.TiltMinAngle = -45;
                                cameraConfig.Capabilities.ZoomMinLevel = 1;
                                cameraConfig.Capabilities.ZoomMaxLevel = 26;
                                break;
                            case PTZType.WWCC:
                            case PTZType.WWCA:
                                cameraConfig.Capabilities.HasEmitter = false;
                                cameraConfig.Capabilities.HasInfrared = false;
                                cameraConfig.Capabilities.HasInverter = false;
                                cameraConfig.Capabilities.HasStabilizer = false;
                                cameraConfig.Capabilities.HasWiper = false;
                                cameraConfig.Capabilities.TiltMaxAngle = 45;
                                cameraConfig.Capabilities.TiltMinAngle = -45;
                                cameraConfig.Capabilities.ZoomMinLevel = -20;
                                cameraConfig.Capabilities.ZoomMaxLevel = 20;
                                break;
                        }

                        SetPTZControlFromCapabilities(cameraConfig.Capabilities);

                        DialogResult r;
                        if (promptForInitialize == DialogResult.Cancel)
                        {
                            r = MessageBox.Show("Would you like to call Initialize() on the plugin?", "Call Initialize()?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        }
                        else
                        {
                            r = promptForInitialize;
                        }

                        pluginClient.PropertyChanged += new PropertyChangedEventHandler(currentClient_PropertyChanged);
                        pluginClient.Open(cameraConfig);

                        if (r == DialogResult.Yes)
                        {
                            pluginClient.Initialize();
                        }

                        SetEnabled(true);
                        this.Cursor = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.DumpToDebug(ex);
                        MessageBox.Show(ex.ToString(), "Error loading or initializing Plugin!");
                        btnStartStop_MouseClick();
                        this.Cursor = Cursors.Default;
                    }
                }

            }
            else if (btnStartStop.Text.Contains("Stop"))
            {
                ClearInterface();

                SetEnabled(false);
                btnStartStop.BackColor = Color.Green;
                btnStartStop.Text = "Start";
            }
            
        }

        private void SetPTZControlFromCapabilities(CameraCapabilitiesAndLimits caps)
        {
            ChangingSelfValues = true;

            ptzControl.PanEnabled = caps.HasPan;

            ptzControl.TiltEnabled = caps.HasTilt;
            ptzControl.TiltAngleMaximum = caps.TiltMaxAngle;
            ptzControl.TiltAngleMinimum = caps.TiltMinAngle;
            ptzControl.TiltAngleCustomTicks = new double[] { caps.TiltMinAngle, 0, caps.TiltMaxAngle };

            ptzControl.ZoomEnabled = caps.HasZoom;
            ptzControl.ZoomLevelMaximum = caps.ZoomMaxLevel;
            ptzControl.ZoomLevelMinimum = caps.ZoomMinLevel;
            ptzControl.ZoomLevelCustomTicks = new double[] { caps.ZoomMinLevel, 0, caps.ZoomMaxLevel };

            ChangingSelfValues = false;
        }

        /// <summary>
        /// Deletes any/all created interfaces
        /// </summary>
        private void ClearInterface()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (serverClient != null)
                {
                    serverClient.Close();
                    serverClient.Opened -= new EventHandler(cameraControlClient_Opened);
                    serverClient.PropertyChanged -= new PropertyChangedEventHandler(currentClient_PropertyChanged);
                    serverClient.Dispose();
                    
                }
                if (pluginClient != null)
                {
                    pluginClient.Close();
                    pluginClient.PropertyChanged -= new PropertyChangedEventHandler(currentClient_PropertyChanged);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while disconnecting the current interface. If you encounter further problems, restart this application." +
                                Environment.NewLine + Environment.NewLine + ex.ToString(),
                                "An error occured in ClearInterface()");
            }
            finally
            {
                serverClient = null;
                pluginClient = null;
                currentClient = null;
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        void cameraControlClient_Opened(object sender, EventArgs e)
        {
            SetEnabled(true);
            btnStartStop.BackColor = Color.Red;
            btnStartStop.Text = "Stop";
            this.Cursor = Cursors.Default;
        }

        private void SetEnabled(bool enabled)
        {
            ptzControl.Enabled = enabled;
            btnInq.Enabled = enabled;
            btnManualInput.Enabled = enabled;
        }

        #region PTZ Controls I/O

        void currentClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Cursor = Cursors.Default;

            ChangingSelfValues = true;
            switch (e.PropertyName)
            {
                case "CurrentPanAngle":
                    ptzControl.PanAngle = currentClient.CurrentPanAngle;
                    break;
                case "CurrentTiltAngle":
                    ptzControl.TiltAngle = currentClient.CurrentTiltAngle;
                    break;
                case "CurrentZoomPosition":
                    ptzControl.ZoomLevel = currentClient.CurrentZoomPosition;
                    break;
            }
            ChangingSelfValues = false;
        }

        private bool ChangingSelfValues
        {
            get;
            set;
        }

        private void ptzControl_PanAngleChanged(object sender, EventArgs e)
        {
            if (ChangingSelfValues)
            {
                return;
            }

            try
            {
                Debug.WriteLine(String.Format("CCTester: PanTiltAbsolute( {0},{1} )", ptzControl.PanAngle, ptzControl.TiltAngle));
                currentClient.PanTiltAbsolute(ptzControl.PanAngle, ptzControl.TiltAngle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error during PanAbsolute");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void ptzControl_TiltAngleChanged(object sender, EventArgs e)
        {
            if (ChangingSelfValues)
            {
                return;
            }
            
            try
            {
                Debug.WriteLine(String.Format("CCTester: PanTiltAbsolute( {0},{1} )", ptzControl.PanAngle, ptzControl.TiltAngle));
                currentClient.PanTiltAbsolute(ptzControl.PanAngle, ptzControl.TiltAngle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error during TiltAbsolute");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void ptzControl_ZoomLevelChanged(object sender, EventArgs e)
        {
            if (ChangingSelfValues)
            {
                return;
            }
            
            try
            {
                Debug.WriteLine("CCTester: ZoomAbsolute( " + ptzControl.ZoomLevel + " )");
                currentClient.ZoomAbsolute(ptzControl.ZoomLevel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error during ZoomAbsolute");
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void btnInq_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.AppStarting;
                Debug.WriteLine("CCTester: PanTiltZoomPositionInquire");
                currentClient.PanTiltZoomPositionInquire();
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                MessageBox.Show(ex.ToString(), "Error during PTZPosInq");
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        private void btnPelcoDAddressSweep_Click(object sender, EventArgs e)
        {
            string[] comPort = config.Address.Split(':');
            if (comPort.Length != 2)
            {
                comPort = new string[] { comPort[0], "1" };
            }

            promptForInitialize = DialogResult.No;

            if (DialogResult.Yes == MessageBox.Show("Commence Pelco-D Address Sweep on " + comPort[0] + "?", "Address Sweep?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                string validAddresses = "";

                btnStartStop.Text = "Stop";
                btnStartStop_MouseClick();

                for (int i = 0; i <= 255; i++)
                {
                    try
                    {
                        config.Address = comPort[0] + ":" + i.ToString();

                        btnStartStop.Text = "Start " + i.ToString();
                        btnStartStop_MouseClick();

                        pluginClient.PanTiltZoomPositionInquire();

                        Debug.WriteLine("Camera Present on Address " + i.ToString() + "!");
                        validAddresses += i.ToString() + ", ";
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Address " + i.ToString() + " is not responding. (" + ex.Message + ")");

                        btnStartStop.Text = "Stop";
                        btnStartStop_MouseClick();
                    }
                }
                if (string.IsNullOrEmpty(validAddresses))
                {
                    validAddresses = "(none)";
                }
                MessageBox.Show("Cameras were found at the following addresses: " + validAddresses, "Sweep Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            promptForInitialize = DialogResult.Cancel;
        }

        private void btnManualInput_Click(object sender, EventArgs e)
        {
            ManualEntry m = new ManualEntry();
            if (this.serverClient != null)
            {
                this.serverClient.PropertyChanged += new PropertyChangedEventHandler(m.ClientPropertyChanged);
            }
            else if(this.pluginClient != null)
            {
                this.pluginClient.PropertyChanged += new PropertyChangedEventHandler(m.ClientPropertyChanged);
            }
            m.Client = this.currentClient;
            m.Show(this);
        }
    }
}
