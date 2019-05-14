using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Media.TV.Scanner.Properties;

using FutureConcepts.Media.DirectShowLib;
using System.Text.RegularExpressions;
using FutureConcepts.Controls.AntaresX.AntaresXForms;
using FutureConcepts.SystemTools.Settings.AntaresX.AntaresXSettings;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.TV.Scanner.Config;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class AdvancedSettings : OKCancelForm
    {
   //     Regex ipRegex = new Regex("\\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b");

        public AdvancedSettings()
        {
            InitializeComponent();

            foreach (string s in Enum.GetNames(typeof(DirectShowLib.TunerInputType)))
            {
                cbTunerInputType.Items.Add(s);
            }

            foreach (string s in Enum.GetNames(typeof(TVSource)))
            {
                cbSourceType.Items.Add(s);
            }

            foreach (string s in Enum.GetNames(typeof(TVMode)))
            {
                cbInputMode.Items.Add(s);
            }
        }

        private void AdvancedSettings_Load(object sender, EventArgs e)
        {
            try
            {
                VideoInputDeviceList devices = new VideoInputDeviceList();
                Tuners = devices.Items;
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Error loading DeviceConfig.xml!", "There was an error loading DeviceConfig.xml. Please check the validity of the file." + Environment.NewLine + Environment.NewLine + ex.ToString(), this);
            }

            this.AutoSnapInterval = AppUser.Current.SnapshotInterval;
            this.AutoSnapMaximum = AppUser.Current.SnapshotMaximum;
            this.TunerInputType = AppUser.Current.TunerInputType;
            this.TunerName = AppUser.Current.TunerDeviceName;
            this.ServerAddress = AppUser.Current.ServerAddress;
            this.ServerSource = AppUser.Current.ServerSourceName;
            this.cbSourceType.SelectedItem = AppUser.Current.TVSource.ToString();
            this.cbInputMode.SelectedItem = AppUser.Current.TVMode.ToString();

            CHK_ShowNetwork.Checked = !string.IsNullOrEmpty(this.ServerAddress);
            CHK_ShowNetwork_CheckedChanged(this, null);
        }

        private void CHK_ShowNetwork_CheckedChanged(object sender, EventArgs e)
        {
            if (TunerName.Equals(VideoInputDeviceList.NetworkDeviceName))
            {
                CHK_ShowNetwork.Checked = true;
            }

            TB_ServerIP.Enabled = CHK_ShowNetwork.Checked;
            B_RefreshSources.Enabled = CHK_ShowNetwork.Checked;
            cbNetworkSources.Enabled = CHK_ShowNetwork.Checked;
        }

        private void CB_TunerDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TunerName.Equals(VideoInputDeviceList.NetworkDeviceName))
            {
                CHK_ShowNetwork.Checked = true;
            }
        }

        protected override void HandleOKClick()
        {
            if (!ValidateServerConfiguration())
            {
                return;
            }

            AppUser.Current.TunerDeviceName = TunerName;
            AppUser.Current.SnapshotInterval = AutoSnapInterval;
            AppUser.Current.SnapshotMaximum = AutoSnapMaximum;
            AppUser.Current.TunerInputType = TunerInputType;
            AppUser.Current.ServerSourceName = ServerSource;
            AppUser.Current.ServerAddress = CHK_ShowNetwork.Checked ? ServerAddress : string.Empty;
            AppUser.Current.TVMode = (TVMode)Enum.Parse(typeof(TVMode), cbInputMode.SelectedItem.ToString());
            AppUser.Current.TVSource = (TVSource)Enum.Parse(typeof(TVSource), cbSourceType.SelectedItem.ToString());

            if (!AppUser.Current.SaveSettings())
            {
                if (DialogResult.No ==
                    FCYesNoMsgBox.ShowDialog("Error while saving!",
                                             "There was an error while saving the settings. Would you like to continue anyway?", this))
                {
                    return;
                }
            }

            this.Close();
        }

        #region [ Properties ]

        private string tunername;
        public string TunerName
        {
            get { return cbTunerDevice.Text; }
            set
            {
                tunername = value;
                cbTunerDevice.Text = tunername;
            }
        }

        public DirectShowLib.TunerInputType TunerInputType
        {
            get
            {
                return (DirectShowLib.TunerInputType)Enum.Parse(typeof(DirectShowLib.TunerInputType), cbTunerInputType.Text);
            }
            set
            {
                cbTunerInputType.Text = value.ToString();
            }
        }

        public TVSource TunerSourceType
        {
            get
            {
                return (TVSource)Enum.Parse(typeof(TVSource), cbSourceType.Text);
            }
            set
            {
                cbSourceType.Text = value.ToString();
            }
        }

        public TVMode TunerInputMode
        {
            get
            {
                return (TVMode)Enum.Parse(typeof(TVMode), cbInputMode.Text);
            }
            set
            {
                cbInputMode.Text = value.ToString();
            }
        }

        public int AutoSnapInterval
        {
            get
            {
                return int.Parse(cbAutoSnapInterval.Text);
            }
            set
            {
                cbAutoSnapInterval.Text = value.ToString();
            }
        }

        public int AutoSnapMaximum
        {
            get
            {
                return (int)udMaximumSnaps.Value;
            }
            set
            {
                udMaximumSnaps.Value = value;
            }
        }

        public List<string> Tuners
        {
            set
            {
                cbTunerDevice.DataSource = value;
            }
        }

        public string ServerAddress
        {
            get
            {
                return TB_ServerIP.Text;
            }
            set
            {
                TB_ServerIP.Text = value;
            }
        }

        public string ServerSource
        {
            get
            {
                return cbNetworkSources.Text;
            }
            set
            {
                cbNetworkSources.Text = value;
            }
        }


        #endregion

        #region Network Configuration stuff

        private void TB_ServerIP_TextChanged(object sender, EventArgs e)
        {
            cbNetworkSources.Items.Clear();
        }

        private void B_RefreshSources_Click(object sender, EventArgs e)
        {
            if (ServerAddress != "")
            {
                try
                {
                    ServerConfig server = new ServerConfig(ServerAddress);
                    ServerInfo info = server.GetServerInfo();

                    cbNetworkSources.Items.Clear();

                    foreach (StreamSourceInfo i in info.StreamSources.Items)
                    {
                        if ((i.SourceType == SourceType.WinTV150) ||
                            (i.SourceType == SourceType.WinTV418) ||
                            (i.SourceType == SourceType.WinTV500) ||
                            (i.SourceType == SourceType.WinTV418ATSC) ||
                            (i.SourceType == SourceType.LogicalGroup))
                        {
                            cbNetworkSources.Items.Add(i.SourceName);
                        }
                    }
                    if (cbNetworkSources.Items.Count == 0)
                    {
                        FCMessageBox.Show("Server has no TV Sources", "The server you entered does not have any TV sources. Try a different server.");
                    }
                    else
                    {
                        cbNetworkSources.DropDownStyle = ComboBoxStyle.DropDownList;
                        cbNetworkSources.DroppedDown = true;
                    }
                }
                catch (Exception ex)
                {
                    FCMessageBox.Show("Could not contact server", ex.Message);
                }
            }
            else
            {
                FCMessageBox.Show("Invalid IP Address", "The IP address you have entered for the Network TV server is invalid. Please correct it before you attempt to get source information.");
            }
        }

        /// <summary>
        /// Returns true if the server configuration is valid, or the user wants to keep it
        /// </summary>
        /// <returns>true if valid and should be saved, false if dialog should stay open</returns>
        private bool ValidateServerConfiguration()
        {
            if (!TunerName.Equals(VideoInputDeviceList.NetworkDeviceName))
            {
                return true;
            }

            ServerConfig server = null;
            try
            {
                server = new ServerConfig(ServerAddress);

                ServerInfo info = server.GetServerInfo();

                bool foundSource = false;

                foreach (StreamSourceInfo i in info.StreamSources.Items)
                {
                    foundSource = (i.SourceName == ServerSource);
                    if (foundSource)
                    {
                        break;
                    }
                }

                if (!foundSource)
                {
                    throw new Exception("The server specified does not have any source called \"" + ServerSource + "\"!");
                }

                return true;
            }
            catch (Exception ex)
            {

                DialogResult r = FCYesNoMsgBox.ShowDialog("Problem with Network TV configuration",
                                        "Your Network TV configuration could not be verified. Would you like ignore this problem and keep these network settings anyway?" + 
                                        Environment.NewLine + 
                                        Environment.NewLine + 
                                        ex.Message,
                                        this);

                return (r == DialogResult.Yes);
            }
            finally
            {
                if (server != null)
                {
                    server.Dispose();
                    server = null;
                }
            }
        }

        #endregion

        private void B_resetUserSettings_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == FCYesNoMsgBox.ShowDialog("Reset User Settings?", "Are you sure you want to reset the user's settings? This includes TV mode, last used channel, volume, and other settings.", this))
            {
                AppUser.Current.SetDefaults();
                if (!AppUser.Current.SaveSettings())
                {
                    if (DialogResult.No == FCYesNoMsgBox.ShowDialog("Save failed!", "Saving the default settings to the disk failed. Continue anyway?", this))
                    {
                        return;
                    }
                }
                this.Close();
            }
        }

        private void btnVideoDecoderSettings_Click(object sender, EventArgs e)
        {
            string decoderName = null;
            SourcesConfig config = SourcesConfig.LoadFromFile();
            Source s = config[cbTunerDevice.SelectedItem as string];
            if (s.Name.Equals(VideoInputDeviceList.NetworkDeviceName))
            {
                decoderName = s[TVSource.Network][FilterType.VideoDecoder].Name;
            }
            else if (s.HasGraphFor(TVSource.LocalDigital))
            {
                decoderName = s[TVSource.LocalDigital][FilterType.VideoDecoder].Name;
            }

            if (string.IsNullOrEmpty(decoderName))
            {
                FCMessageBox.Show("No Decoder Specified!", "No video decoder is specified for this particular tuner.", this);
                return;
            }

            IGraphBuilder g = null;
            IBaseFilter f = null;
            try
            {
                g = (IGraphBuilder)new FilterGraph();
                f = FilterGraphTools.AddFilterByName(g, FilterCategory.LegacyAmFilterCategory, decoderName) as IBaseFilter;

                if (f == null)
                {
                    throw new Exception("Could not instantiate video decoder \"" + decoderName + "\"!");
                }

                FilterPropertyPage propPage = new FilterPropertyPage(this, f);
                propPage.Show();
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Error!", ex.Message, this);
            }
            finally
            {
                if (f != null)
                {
                    Marshal.ReleaseComObject(f);
                }
                if (g != null)
                {
                    Marshal.ReleaseComObject(g);
                }
            }
        }

        private void lblTunerDevice_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                VideoInputDeviceList vidl = new VideoInputDeviceList();
                vidl.PopulateAllDevices();
                System.Diagnostics.Debug.WriteLine("<< Video input devices on system >>");
                foreach (string id in vidl.Items)
                {
                    System.Diagnostics.Debug.WriteLine(id);
                }
            }
            catch (Exception ex)
            {
                FCMessageBox.Show("Error!", ex.ToString(), this);
            }
        }
    }
}