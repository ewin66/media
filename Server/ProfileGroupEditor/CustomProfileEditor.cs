using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    public partial class CustomProfileEditor : UserControl
    {
        protected static double[] FPSValues = { 1, 3, 5, 10, 15, 30 };
        protected static double[] FPMValues = { 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 18, 20, 24, 25, 30, 36, 40, 45, 50, 60 };

        public CustomProfileEditor()
        {
            InitializeComponent();

            general_cbProtocol.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(SinkProtocolType)))
            {
                general_cbProtocol.Items.Add(s);
            }

            //populate video codec combo box
            video_cbCodec.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(VideoCodecType)))
            {
                video_cbCodec.Items.Add(s);
            }

            //populate image size combo box
            video_cbImageSize.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(VideoImageSize)))
            {
                video_cbImageSize.Items.Add(s);
            }

            //populate video quality combo box
            video_cbQuality.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(VideoQuality)))
            {
                video_cbQuality.Items.Add(s);
            }

            //populate frame rate units combo box
            video_cbFramerateUnits.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(VideoFrameRateUnits)))
            {
                video_cbFramerateUnits.Items.Add(s);
            }

            //populate audio codec combo box
            audio_cbCodec.Items.Clear();
            foreach (string s in Enum.GetNames(typeof(AudioCodecType)))
            {
                audio_cbCodec.Items.Add(s);
            }
        }

        private TabPage _lastSelTab;
        /// <summary>
        /// This property is used to track the last selected tab index.
        /// </summary>
        protected TabPage LastSelectedTab
        {
            get { return _lastSelTab; }
            set { _lastSelTab = value; }
        }


        private Profile _profile = null;
        /// <summary>
        /// Attach a profile to be edited.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public Profile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                LoadSettings();
            }
        }

        #region Loading of Settings

        private bool _loading = false;
        /// <summary>
        /// True if settings are being loaded, false if not
        /// </summary>
        private bool Loading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
            }
        }

        /// <summary>
        /// Loads all of the settings for the current profile
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                Loading = true;

                if (Profile == null)
                {
                    Profile = new Profile();
                }

                if (Profile.Name == null)
                {
                    general_txtName.Text = "New Profile";
                }
                else
                {
                    general_txtName.Text = Profile.Name;
                }

                general_chkboxHasVideo.Checked = (Profile.Video != null);
                general_chkboxHasVideo_CheckedChanged(this, null);
                general_chkboxHasAudio.Checked = (Profile.Audio != null);
                general_chkboxHasAudio_CheckedChanged(this, null);

                general_cbProtocol.SelectedItem = Profile.SinkProtocol.ToString();

                if (settingsTabs.Controls.Contains(LastSelectedTab))
                {
                    settingsTabs.SelectedTab = LastSelectedTab;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred.");
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Loads all of the video settings for the current profile.
        /// If Profile.Video is null, it is created with defaults.
        /// </summary>
        protected void LoadVideoSettings()
        {
            if (Profile == null)
                return;

            Loading = true;

            if (Profile.Video == null)
            {
                Profile.Video = new VideoSettings();

                Profile.Video.CodecType = VideoCodecType.H264;
                Profile.Video.Quality = VideoQuality.Undefined;
                Profile.Video.ImageSize = VideoImageSize.Size_CIF;

                Profile.Video.FrameRate = 15;
                Profile.Video.FrameRateUnits = VideoFrameRateUnits.FramesPerSecond;
                Profile.Video.KeyFrameRate = 90;
                Profile.Video.ConstantBitRate = 64;
                Profile.Video.VBR = null;
            }

            video_cbCodec.SelectedItem = Profile.Video.CodecType.ToString();

            video_cbQuality.SelectedItem = Profile.Video.Quality.ToString();

            video_cbImageSize.SelectedItem = Profile.Video.ImageSize.ToString();

            video_cbFramerateUnits.SelectedItem = Profile.Video.FrameRateUnits.ToString();

            video_tbFramerate.Value = (double)Profile.Video.FrameRate;

            video_tbKeyFramerate.Value = (double)Profile.Video.KeyFrameRate;

            video_radioIsVBR.Checked = (Profile.Video.VBR != null);
            video_radioIsCBR.Checked = !video_radioIsVBR.Checked;
            video_radioBitrate_CheckedChanged(this, null);

            video_Bitrate.Value = Profile.Video.ConstantBitRate;

            if (Profile.Video.VBR != null)
            {
                video_MinBitrate.Value = Profile.Video.VBR.MinBitRate;
                video_MaxBitrate.Value = Profile.Video.VBR.MaxBitRate;
            }

            video_cbQuality_SelectedIndexChanged(this, new EventArgs());

            Loading = false;
        }

        /// <summary>
        /// Loads all of the audio settings for the current profile.
        /// If Profile.Audio is null, then it is created with defaults
        /// </summary>
        protected void LoadAudioSettings()
        {
            if (Profile == null)
                return;

            Loading = true;

            if (Profile.Audio == null)
            {
                Profile.Audio = new AudioSettings();
                Profile.Audio.CodecType = AudioCodecType.AMR;

                Profile.Audio.ConstantBitRate = 32;
                Profile.Audio.VBR = null;
            }

            audio_cbCodec.SelectedItem = Profile.Audio.CodecType.ToString();

            audio_Bitrate.Value = Profile.Audio.ConstantBitRate;

            audio_radioIsVBR.Checked = (Profile.Audio.VBR != null);
            audio_radioIsCBR.Checked = !audio_radioIsVBR.Checked;
            audio_radioBitrate_CheckedChanged(this, null);

            if (Profile.Audio.VBR != null)
            {
                audio_MinBitrate.Value = Profile.Audio.VBR.MinBitRate;
                audio_MaxBitrate.Value = Profile.Audio.VBR.MaxBitRate;
            }

            Loading = false;
        }

        #endregion

        #region General Controls Handlers

        private void settingsTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            LastSelectedTab = settingsTabs.SelectedTab;
        }

        private void general_txtName_TextChanged(object sender, EventArgs e)
        {
            general_btnPreview.Text = general_txtName.Text;
            Profile.Name = general_txtName.Text;
            if (ProfileNameChanged != null)
            {
                ProfileNameChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// This event is fired when the profile name has changed.
        /// </summary>
        [Category("Action"), Browsable(true)]
        public event EventHandler ProfileNameChanged;

        private void general_chkboxHasVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (general_chkboxHasVideo.Checked)
            {
                if (!settingsTabs.Controls.Contains(tabVideo))
                {
                    settingsTabs.Controls.Add(tabVideo);
                }
                LoadVideoSettings();
            }
            else if (Profile.Video != null)
            {
                if (DialogResult.Yes == ShowStreamRemovalWarning("Video"))
                {
                    settingsTabs.Controls.Remove(tabVideo);
                    Profile.Video = null;
                }
                else
                {
                    general_chkboxHasVideo.Checked = true;
                }
            }
            else
            {
                settingsTabs.Controls.Remove(tabVideo);
            }
        }

        private void general_chkboxHasAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (general_chkboxHasAudio.Checked)
            {
                if (!settingsTabs.Controls.Contains(tabAudio))
                {
                    settingsTabs.Controls.Add(tabAudio);
                }
                LoadAudioSettings();
            }
            else if (Profile.Audio != null)
            {
                if (DialogResult.Yes == ShowStreamRemovalWarning("Audio"))
                {
                    settingsTabs.Controls.Remove(tabAudio);
                    Profile.Audio = null;
                }
                else
                {
                    general_chkboxHasAudio.Checked = true;
                }
            }
            else
            {
                settingsTabs.Controls.Remove(tabAudio);
            }
        }

        private DialogResult ShowStreamRemovalWarning(string streamType)
        {
            return MessageBox.Show(this, "Indicating that this profile has no " + streamType + 
                                         " stream will also erase any " + streamType + 
                                         " settings you have previously specified for this Profile." +
                                         Environment.NewLine + Environment.NewLine + 
                                         "Do you really want to remove this stream?",

                                         "Remove " + streamType + " Stream Settings?",

                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
        }

        private void general_cbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.SinkProtocol = (SinkProtocolType)Enum.Parse(typeof(SinkProtocolType), general_cbProtocol.SelectedItem.ToString());
            }
            catch { }
        }

        #endregion

        #region Video Control Handlers

        private void video_cbCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.Video.CodecType = (VideoCodecType)Enum.Parse(typeof(VideoCodecType), video_cbCodec.SelectedItem.ToString());

                video_lblKeyFramerate.Enabled = (Profile.Video.CodecType != VideoCodecType.MJ2K);

                bool lastLoading = Loading;
                Loading = true;

                //update acceptable ranges of bitrates:
                if (Profile.Video.CodecType == VideoCodecType.MPEG2)
                {
                    video_Bitrate.Maximum = 16384;
                    video_Bitrate.Minimum = 2048;
                }
                else
                {
                    video_Bitrate.Maximum = 4096;
                    video_Bitrate.Minimum = 32;
                }
                video_MinBitrate.Maximum = video_Bitrate.Maximum;
                video_MinBitrate.Minimum = video_Bitrate.Minimum;
                video_MaxBitrate.Maximum = video_Bitrate.Maximum;
                video_MaxBitrate.Minimum = video_Bitrate.Minimum;

                Loading = lastLoading;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void video_cbQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.Video.Quality = (VideoQuality)Enum.Parse(typeof(VideoQuality), video_cbQuality.SelectedItem.ToString());

                bool enableBitrates = (Profile.Video.Quality == VideoQuality.Undefined);

                video_radioIsCBR.Visible = enableBitrates;
                video_Bitrate.Visible = enableBitrates;

                video_radioIsVBR.Visible = enableBitrates;
                video_MinBitrate.Visible = enableBitrates;
                video_MaxBitrate.Visible = enableBitrates;

                if (enableBitrates)
                {
                    video_radioIsVBR.Checked = (Profile.Video.VBR != null);
                    video_radioIsCBR.Checked = !video_radioIsVBR.Checked;
                    video_radioBitrate_CheckedChanged(this, new EventArgs());
                }
                else
                {
                    Profile.Video.VBR = null;
                }
            }
            catch { }
        }

        private void video_cbImageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.Video.ImageSize = (VideoImageSize)Enum.Parse(typeof(VideoImageSize), video_cbImageSize.SelectedItem.ToString());
            }
            catch { }
        }

        private void video_Bitrate_ValueChanged(object sender, EventArgs e)
        {
            if (!Loading)
            {
                Profile.Video.ConstantBitRate = video_Bitrate.Value;
            }
        }

        private void video_radioBitrate_CheckedChanged(object sender, EventArgs e)
        {
            bool vbr = video_radioIsVBR.Checked;
            if ((vbr) && (Profile.Video.VBR == null))
            {
                Profile.Video.VBR = new VBR();
                video_MinBitrate.Value = Profile.Video.ConstantBitRate;
                video_MaxBitrate.Value = Profile.Video.ConstantBitRate;
            }
            else if (!vbr)
            {
                Profile.Video.VBR = null;
            }

            video_Bitrate.Enabled = !vbr;
            video_MinBitrate.Enabled = vbr;
            video_MaxBitrate.Enabled = vbr;
        }

        private void video_MinBitrate_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            Profile.Video.VBR.MinBitRate = video_MinBitrate.Value;

            if (video_MinBitrate.Value > video_MaxBitrate.Value)
            {
                video_MaxBitrate.Value = video_MinBitrate.Value;
            }
        }

        private void video_MaxBitrate_ValueChanged(object sender, EventArgs e)
        {
            if (Loading)
            {
                return;
            }

            Profile.Video.VBR.MaxBitRate = video_MaxBitrate.Value;

            if (video_MaxBitrate.Value < video_MinBitrate.Value)
            {
                video_MinBitrate.Value = video_MaxBitrate.Value;
            }
        }

        //----- frame rate info -----------------------------------------------------------
        private void tbFramerate_Scroll(object sender, EventArgs e)
        {
            video_UpdateFramerate((int)video_tbFramerate.CurrentValue);
        }

        private void tbFramerate_ValueChanged(object sender, EventArgs e)
        {
            if (!Loading)
            {
                Profile.Video.FrameRate = (int)video_tbFramerate.Value;
            }

            video_UpdateFramerate((int)video_tbFramerate.Value);
            video_UpdateKeyFramerate((int)video_tbKeyFramerate.Value);
        }

        private void video_cbFramerateUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.Video.FrameRateUnits = (VideoFrameRateUnits)Enum.Parse(typeof(VideoFrameRateUnits), video_cbFramerateUnits.SelectedItem.ToString());

                if (Profile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerSecond)
                {
                    video_tbFramerate.Enabled = true;
                    video_lblFramerate.Enabled = video_lblFramerate.Enabled;
                    video_lblFramerateFPS.Enabled = video_lblFramerate.Enabled;

                    video_lblKeyFramerate.Enabled = (Profile.Video.CodecType != VideoCodecType.MJ2K);
                    video_tbKeyFramerate.Enabled = video_lblKeyFramerate.Enabled;
                    video_lblKeyFramerateSeconds.Enabled = video_lblKeyFramerate.Enabled;

                    video_tbFramerate.CustomTicks = FPSValues;
                    video_tbFramerate.Maximum = 30;
                }
                else if (Profile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerMinute)
                {
                    video_tbFramerate.Enabled = true;
                    video_lblFramerate.Enabled = video_tbFramerate.Enabled;
                    video_lblFramerateFPS.Enabled = video_tbFramerate.Enabled;

                    video_lblKeyFramerate.Enabled = false;
                    video_tbKeyFramerate.Enabled = video_lblKeyFramerate.Enabled;
                    video_lblKeyFramerateSeconds.Enabled = video_lblKeyFramerate.Enabled;

                    video_tbFramerate.CustomTicks = FPMValues;
                    video_tbFramerate.Maximum = 60;
                }
                else
                {
                    video_tbFramerate.Enabled = false;
                    video_lblFramerate.Enabled = false;
                    video_lblFramerateFPS.Enabled = false;

                    video_lblKeyFramerate.Enabled = false;
                    video_tbKeyFramerate.Enabled = false;
                    video_lblKeyFramerateSeconds.Enabled = false;
                }
            }
            catch { }
        }

        private void video_UpdateFramerate(int p)
        {
            video_lblFramerateFPS.Text = p.ToString("0");
        }

        private void video_tbKeyFramerate_Scroll(object sender, EventArgs e)
        {
            video_UpdateKeyFramerate((int)video_tbKeyFramerate.CurrentValue);
        }

        private void video_tbKeyFramerate_ValueChanged(object sender, EventArgs e)
        {
            Profile.Video.KeyFrameRate = (int)video_tbKeyFramerate.Value;

            video_UpdateKeyFramerate((int)video_tbKeyFramerate.Value);
        }

        private void video_UpdateKeyFramerate(int p)
        {
            if (Profile.Video.FrameRateUnits != VideoFrameRateUnits.FramesPerSecond)
            {
                return;
            }
            video_lblKeyFramerateSeconds.Text = p.ToString("0 frames / ") + (p / video_tbFramerate.Value).ToString("0.# seconds");
        }

        #endregion

        #region Audio Control Handlers

        private void audio_cbCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Profile.Audio.CodecType = (AudioCodecType)Enum.Parse(typeof(AudioCodecType), audio_cbCodec.SelectedItem.ToString());
            }
            catch { }
        }

        private void audio_Bitrate_ValueChanged(object sender, EventArgs e)
        {
            Profile.Audio.ConstantBitRate = audio_Bitrate.Value;
        }

        private void audio_radioBitrate_CheckedChanged(object sender, EventArgs e)
        {
            bool vbr = audio_radioIsVBR.Checked;
            if ((vbr) && (Profile.Audio.VBR == null))
            {
                Profile.Audio.VBR = new VBR();
                audio_MinBitrate.Value = Profile.Audio.ConstantBitRate;
                audio_MaxBitrate.Value = Profile.Audio.ConstantBitRate;
            }
            else if (!vbr)
            {
                Profile.Audio.VBR = null;
            }

            audio_Bitrate.Enabled = !vbr;
            audio_MinBitrate.Enabled = vbr;
            audio_MaxBitrate.Enabled = vbr;
        }

        private void audio_MinBitrate_ValueChanged(object sender, EventArgs e)
        {
            Profile.Audio.VBR.MinBitRate = audio_MinBitrate.Value;

            if (audio_MinBitrate.Value > audio_MaxBitrate.Value)
            {
                audio_MaxBitrate.Value = audio_MinBitrate.Value;
            }
        }

        private void audio_MaxBitrate_ValueChanged(object sender, EventArgs e)
        {
            Profile.Audio.VBR.MaxBitRate = audio_MaxBitrate.Value;

            if (audio_MaxBitrate.Value < audio_MinBitrate.Value)
            {
                audio_MinBitrate.Value = audio_MaxBitrate.Value;
            }
        }

        #endregion
    }
}
