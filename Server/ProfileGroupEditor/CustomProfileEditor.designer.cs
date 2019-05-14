namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    partial class CustomProfileEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pContent = new System.Windows.Forms.Panel();
            this.settingsTabs = new FutureConcepts.Controls.Common.FlatTabControl.FlatTabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.general_cbProtocol = new System.Windows.Forms.ComboBox();
            this.general_lblProtocol = new System.Windows.Forms.Label();
            this.general_chkboxHasAudio = new System.Windows.Forms.CheckBox();
            this.general_chkboxHasVideo = new System.Windows.Forms.CheckBox();
            this.general_btnPreview = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.general_txtName = new System.Windows.Forms.TextBox();
            this.general_lblName = new System.Windows.Forms.Label();
            this.tabVideo = new System.Windows.Forms.TabPage();
            this.video_cbFramerateUnits = new System.Windows.Forms.ComboBox();
            this.video_lblQuality = new System.Windows.Forms.Label();
            this.video_cbQuality = new System.Windows.Forms.ComboBox();
            this.video_radioIsVBR = new System.Windows.Forms.RadioButton();
            this.video_radioIsCBR = new System.Windows.Forms.RadioButton();
            this.video_lblCodec = new System.Windows.Forms.Label();
            this.video_cbCodec = new System.Windows.Forms.ComboBox();
            this.video_lblFramerate = new System.Windows.Forms.Label();
            this.video_lblKeyFramerate = new System.Windows.Forms.Label();
            this.video_lblImageSize = new System.Windows.Forms.Label();
            this.video_cbImageSize = new System.Windows.Forms.ComboBox();
            this.video_lblKeyFramerateSeconds = new System.Windows.Forms.Label();
            this.video_tbFramerate = new FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar();
            this.video_tbKeyFramerate = new FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar();
            this.video_lblFramerateFPS = new System.Windows.Forms.Label();
            this.video_MaxBitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.video_MinBitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.video_Bitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.tabAudio = new System.Windows.Forms.TabPage();
            this.audio_MaxBitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.audio_MinBitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.audio_Bitrate = new FutureConcepts.Media.Server.ProfileGroupEditor.HorizontalBitrateSelector();
            this.audio_radioIsVBR = new System.Windows.Forms.RadioButton();
            this.audio_radioIsCBR = new System.Windows.Forms.RadioButton();
            this.audio_lblCodec = new System.Windows.Forms.Label();
            this.audio_cbCodec = new System.Windows.Forms.ComboBox();
            this.ttMain = new System.Windows.Forms.ToolTip(this.components);
            this.pContent.SuspendLayout();
            this.settingsTabs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabVideo.SuspendLayout();
            this.tabAudio.SuspendLayout();
            this.SuspendLayout();
            // 
            // pContent
            // 
            this.pContent.BackColor = System.Drawing.Color.Black;
            this.pContent.Controls.Add(this.settingsTabs);
            this.pContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pContent.Location = new System.Drawing.Point(0, 0);
            this.pContent.Name = "pContent";
            this.pContent.Size = new System.Drawing.Size(442, 314);
            this.pContent.TabIndex = 0;
            // 
            // settingsTabs
            // 
            this.settingsTabs.Controls.Add(this.tabGeneral);
            this.settingsTabs.Controls.Add(this.tabVideo);
            this.settingsTabs.Controls.Add(this.tabAudio);
            this.settingsTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTabs.Location = new System.Drawing.Point(0, 0);
            this.settingsTabs.myBackColor = System.Drawing.Color.Black;
            this.settingsTabs.Name = "settingsTabs";
            this.settingsTabs.SelectedIndex = 0;
            this.settingsTabs.Size = new System.Drawing.Size(442, 314);
            this.settingsTabs.TabIndex = 19;
            this.settingsTabs.SelectedIndexChanged += new System.EventHandler(this.settingsTabs_SelectedIndexChanged);
            // 
            // tabGeneral
            // 
            this.tabGeneral.BackColor = System.Drawing.Color.Black;
            this.tabGeneral.Controls.Add(this.general_cbProtocol);
            this.tabGeneral.Controls.Add(this.general_lblProtocol);
            this.tabGeneral.Controls.Add(this.general_chkboxHasAudio);
            this.tabGeneral.Controls.Add(this.general_chkboxHasVideo);
            this.tabGeneral.Controls.Add(this.general_btnPreview);
            this.tabGeneral.Controls.Add(this.general_txtName);
            this.tabGeneral.Controls.Add(this.general_lblName);
            this.tabGeneral.ForeColor = System.Drawing.Color.White;
            this.tabGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(434, 285);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            // 
            // general_cbProtocol
            // 
            this.general_cbProtocol.BackColor = System.Drawing.Color.Black;
            this.general_cbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.general_cbProtocol.ForeColor = System.Drawing.Color.White;
            this.general_cbProtocol.FormattingEnabled = true;
            this.general_cbProtocol.Location = new System.Drawing.Point(92, 89);
            this.general_cbProtocol.Name = "general_cbProtocol";
            this.general_cbProtocol.Size = new System.Drawing.Size(172, 21);
            this.general_cbProtocol.TabIndex = 7;
            this.ttMain.SetToolTip(this.general_cbProtocol, "Select the protocol to use for this profile. Switching protocols while streaming " +
                    "causes a reconnect.");
            this.general_cbProtocol.SelectedIndexChanged += new System.EventHandler(this.general_cbProtocol_SelectedIndexChanged);
            // 
            // general_lblProtocol
            // 
            this.general_lblProtocol.AutoSize = true;
            this.general_lblProtocol.Location = new System.Drawing.Point(13, 92);
            this.general_lblProtocol.Name = "general_lblProtocol";
            this.general_lblProtocol.Size = new System.Drawing.Size(73, 13);
            this.general_lblProtocol.TabIndex = 6;
            this.general_lblProtocol.Text = "Sink Protocol:";
            // 
            // general_chkboxHasAudio
            // 
            this.general_chkboxHasAudio.AutoSize = true;
            this.general_chkboxHasAudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.general_chkboxHasAudio.Location = new System.Drawing.Point(92, 62);
            this.general_chkboxHasAudio.Name = "general_chkboxHasAudio";
            this.general_chkboxHasAudio.Size = new System.Drawing.Size(108, 17);
            this.general_chkboxHasAudio.TabIndex = 5;
            this.general_chkboxHasAudio.Text = "Has Audio Stream";
            this.ttMain.SetToolTip(this.general_chkboxHasAudio, "Check this box if this profile will include Audio.");
            this.general_chkboxHasAudio.UseVisualStyleBackColor = true;
            this.general_chkboxHasAudio.CheckedChanged += new System.EventHandler(this.general_chkboxHasAudio_CheckedChanged);
            // 
            // general_chkboxHasVideo
            // 
            this.general_chkboxHasVideo.AutoSize = true;
            this.general_chkboxHasVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.general_chkboxHasVideo.Location = new System.Drawing.Point(92, 39);
            this.general_chkboxHasVideo.Name = "general_chkboxHasVideo";
            this.general_chkboxHasVideo.Size = new System.Drawing.Size(108, 17);
            this.general_chkboxHasVideo.TabIndex = 4;
            this.general_chkboxHasVideo.Text = "Has Video Stream";
            this.ttMain.SetToolTip(this.general_chkboxHasVideo, "Check this box if this profile will include Video.");
            this.general_chkboxHasVideo.UseVisualStyleBackColor = true;
            this.general_chkboxHasVideo.CheckedChanged += new System.EventHandler(this.general_chkboxHasVideo_CheckedChanged);
            // 
            // general_btnPreview
            // 
            this.general_btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.general_btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.general_btnPreview.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.general_btnPreview.ForeColor = System.Drawing.Color.White;
            this.general_btnPreview.Location = new System.Drawing.Point(270, 11);
            this.general_btnPreview.Name = "general_btnPreview";
            this.general_btnPreview.Size = new System.Drawing.Size(95, 23);
            this.general_btnPreview.TabIndex = 2;
            this.general_btnPreview.Text = " ";
            this.ttMain.SetToolTip(this.general_btnPreview, "This button is a preview of how the Profile selector button will look in Streamin" +
                    "g Video Desktop.");
            this.general_btnPreview.UseVisualStyleBackColor = false;
            // 
            // general_txtName
            // 
            this.general_txtName.BackColor = System.Drawing.Color.Black;
            this.general_txtName.ForeColor = System.Drawing.Color.White;
            this.general_txtName.Location = new System.Drawing.Point(92, 13);
            this.general_txtName.Name = "general_txtName";
            this.general_txtName.Size = new System.Drawing.Size(172, 20);
            this.general_txtName.TabIndex = 1;
            this.ttMain.SetToolTip(this.general_txtName, "The name of the profile. Within a group, there cannot be more than one profile wi" +
                    "th the same name.");
            this.general_txtName.TextChanged += new System.EventHandler(this.general_txtName_TextChanged);
            // 
            // general_lblName
            // 
            this.general_lblName.AutoSize = true;
            this.general_lblName.Location = new System.Drawing.Point(16, 16);
            this.general_lblName.Name = "general_lblName";
            this.general_lblName.Size = new System.Drawing.Size(70, 13);
            this.general_lblName.TabIndex = 0;
            this.general_lblName.Text = "Profile Name:";
            // 
            // tabVideo
            // 
            this.tabVideo.AutoScroll = true;
            this.tabVideo.AutoScrollMinSize = new System.Drawing.Size(300, 100);
            this.tabVideo.BackColor = System.Drawing.Color.Black;
            this.tabVideo.Controls.Add(this.video_cbFramerateUnits);
            this.tabVideo.Controls.Add(this.video_lblQuality);
            this.tabVideo.Controls.Add(this.video_cbQuality);
            this.tabVideo.Controls.Add(this.video_radioIsVBR);
            this.tabVideo.Controls.Add(this.video_radioIsCBR);
            this.tabVideo.Controls.Add(this.video_lblCodec);
            this.tabVideo.Controls.Add(this.video_cbCodec);
            this.tabVideo.Controls.Add(this.video_lblFramerate);
            this.tabVideo.Controls.Add(this.video_lblKeyFramerate);
            this.tabVideo.Controls.Add(this.video_lblImageSize);
            this.tabVideo.Controls.Add(this.video_cbImageSize);
            this.tabVideo.Controls.Add(this.video_lblKeyFramerateSeconds);
            this.tabVideo.Controls.Add(this.video_tbFramerate);
            this.tabVideo.Controls.Add(this.video_tbKeyFramerate);
            this.tabVideo.Controls.Add(this.video_lblFramerateFPS);
            this.tabVideo.Controls.Add(this.video_MaxBitrate);
            this.tabVideo.Controls.Add(this.video_MinBitrate);
            this.tabVideo.Controls.Add(this.video_Bitrate);
            this.tabVideo.ForeColor = System.Drawing.Color.White;
            this.tabVideo.Location = new System.Drawing.Point(4, 25);
            this.tabVideo.Name = "tabVideo";
            this.tabVideo.Padding = new System.Windows.Forms.Padding(3);
            this.tabVideo.Size = new System.Drawing.Size(434, 285);
            this.tabVideo.TabIndex = 0;
            this.tabVideo.Text = "Video";
            // 
            // video_cbFramerateUnits
            // 
            this.video_cbFramerateUnits.BackColor = System.Drawing.Color.Black;
            this.video_cbFramerateUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.video_cbFramerateUnits.ForeColor = System.Drawing.Color.White;
            this.video_cbFramerateUnits.FormattingEnabled = true;
            this.video_cbFramerateUnits.Items.AddRange(new object[] {
            "FramesPerSecond",
            "FramesPerMinute"});
            this.video_cbFramerateUnits.Location = new System.Drawing.Point(302, 93);
            this.video_cbFramerateUnits.Name = "video_cbFramerateUnits";
            this.video_cbFramerateUnits.Size = new System.Drawing.Size(114, 21);
            this.video_cbFramerateUnits.TabIndex = 34;
            this.video_cbFramerateUnits.SelectedIndexChanged += new System.EventHandler(this.video_cbFramerateUnits_SelectedIndexChanged);
            // 
            // video_lblQuality
            // 
            this.video_lblQuality.AutoSize = true;
            this.video_lblQuality.BackColor = System.Drawing.Color.Transparent;
            this.video_lblQuality.ForeColor = System.Drawing.Color.White;
            this.video_lblQuality.Location = new System.Drawing.Point(6, 36);
            this.video_lblQuality.Name = "video_lblQuality";
            this.video_lblQuality.Size = new System.Drawing.Size(100, 13);
            this.video_lblQuality.TabIndex = 32;
            this.video_lblQuality.Text = "Compressor Quality:";
            // 
            // video_cbQuality
            // 
            this.video_cbQuality.BackColor = System.Drawing.Color.Black;
            this.video_cbQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.video_cbQuality.ForeColor = System.Drawing.Color.White;
            this.video_cbQuality.FormattingEnabled = true;
            this.video_cbQuality.Location = new System.Drawing.Point(112, 33);
            this.video_cbQuality.MaxDropDownItems = 30;
            this.video_cbQuality.Name = "video_cbQuality";
            this.video_cbQuality.Size = new System.Drawing.Size(214, 21);
            this.video_cbQuality.TabIndex = 33;
            this.video_cbQuality.SelectedIndexChanged += new System.EventHandler(this.video_cbQuality_SelectedIndexChanged);
            // 
            // video_radioIsVBR
            // 
            this.video_radioIsVBR.AutoSize = true;
            this.video_radioIsVBR.BackColor = System.Drawing.Color.Black;
            this.video_radioIsVBR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.video_radioIsVBR.Location = new System.Drawing.Point(6, 196);
            this.video_radioIsVBR.Name = "video_radioIsVBR";
            this.video_radioIsVBR.Size = new System.Drawing.Size(95, 17);
            this.video_radioIsVBR.TabIndex = 28;
            this.video_radioIsVBR.TabStop = true;
            this.video_radioIsVBR.Text = "Variable Bitrate";
            this.video_radioIsVBR.UseVisualStyleBackColor = false;
            this.video_radioIsVBR.CheckedChanged += new System.EventHandler(this.video_radioBitrate_CheckedChanged);
            // 
            // video_radioIsCBR
            // 
            this.video_radioIsCBR.AutoSize = true;
            this.video_radioIsCBR.BackColor = System.Drawing.Color.Black;
            this.video_radioIsCBR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.video_radioIsCBR.Location = new System.Drawing.Point(6, 167);
            this.video_radioIsCBR.Name = "video_radioIsCBR";
            this.video_radioIsCBR.Size = new System.Drawing.Size(102, 17);
            this.video_radioIsCBR.TabIndex = 27;
            this.video_radioIsCBR.TabStop = true;
            this.video_radioIsCBR.Text = "Constant Bitrate:";
            this.video_radioIsCBR.UseVisualStyleBackColor = false;
            this.video_radioIsCBR.CheckedChanged += new System.EventHandler(this.video_radioBitrate_CheckedChanged);
            // 
            // video_lblCodec
            // 
            this.video_lblCodec.AutoSize = true;
            this.video_lblCodec.BackColor = System.Drawing.Color.Transparent;
            this.video_lblCodec.ForeColor = System.Drawing.Color.White;
            this.video_lblCodec.Location = new System.Drawing.Point(65, 9);
            this.video_lblCodec.Name = "video_lblCodec";
            this.video_lblCodec.Size = new System.Drawing.Size(41, 13);
            this.video_lblCodec.TabIndex = 14;
            this.video_lblCodec.Text = "Codec:";
            // 
            // video_cbCodec
            // 
            this.video_cbCodec.BackColor = System.Drawing.Color.Black;
            this.video_cbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.video_cbCodec.ForeColor = System.Drawing.Color.White;
            this.video_cbCodec.FormattingEnabled = true;
            this.video_cbCodec.Items.AddRange(new object[] {
            "H.264",
            "MPEG2"});
            this.video_cbCodec.Location = new System.Drawing.Point(112, 6);
            this.video_cbCodec.MaxDropDownItems = 30;
            this.video_cbCodec.Name = "video_cbCodec";
            this.video_cbCodec.Size = new System.Drawing.Size(214, 21);
            this.video_cbCodec.TabIndex = 15;
            this.video_cbCodec.SelectedIndexChanged += new System.EventHandler(this.video_cbCodec_SelectedIndexChanged);
            // 
            // video_lblFramerate
            // 
            this.video_lblFramerate.AutoSize = true;
            this.video_lblFramerate.BackColor = System.Drawing.Color.Transparent;
            this.video_lblFramerate.ForeColor = System.Drawing.Color.White;
            this.video_lblFramerate.Location = new System.Drawing.Point(41, 96);
            this.video_lblFramerate.Name = "video_lblFramerate";
            this.video_lblFramerate.Size = new System.Drawing.Size(65, 13);
            this.video_lblFramerate.TabIndex = 4;
            this.video_lblFramerate.Text = "Frame Rate:";
            // 
            // video_lblKeyFramerate
            // 
            this.video_lblKeyFramerate.AutoSize = true;
            this.video_lblKeyFramerate.BackColor = System.Drawing.Color.Transparent;
            this.video_lblKeyFramerate.ForeColor = System.Drawing.Color.White;
            this.video_lblKeyFramerate.Location = new System.Drawing.Point(20, 127);
            this.video_lblKeyFramerate.Name = "video_lblKeyFramerate";
            this.video_lblKeyFramerate.Size = new System.Drawing.Size(86, 13);
            this.video_lblKeyFramerate.TabIndex = 5;
            this.video_lblKeyFramerate.Text = "Key-Frame every";
            // 
            // video_lblImageSize
            // 
            this.video_lblImageSize.AutoSize = true;
            this.video_lblImageSize.BackColor = System.Drawing.Color.Transparent;
            this.video_lblImageSize.ForeColor = System.Drawing.Color.White;
            this.video_lblImageSize.Location = new System.Drawing.Point(46, 63);
            this.video_lblImageSize.Name = "video_lblImageSize";
            this.video_lblImageSize.Size = new System.Drawing.Size(60, 13);
            this.video_lblImageSize.TabIndex = 6;
            this.video_lblImageSize.Text = "Resolution:";
            // 
            // video_cbImageSize
            // 
            this.video_cbImageSize.BackColor = System.Drawing.Color.Black;
            this.video_cbImageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.video_cbImageSize.ForeColor = System.Drawing.Color.White;
            this.video_cbImageSize.FormattingEnabled = true;
            this.video_cbImageSize.Location = new System.Drawing.Point(112, 60);
            this.video_cbImageSize.MaxDropDownItems = 30;
            this.video_cbImageSize.Name = "video_cbImageSize";
            this.video_cbImageSize.Size = new System.Drawing.Size(214, 21);
            this.video_cbImageSize.TabIndex = 7;
            this.video_cbImageSize.SelectedIndexChanged += new System.EventHandler(this.video_cbImageSize_SelectedIndexChanged);
            // 
            // video_lblKeyFramerateSeconds
            // 
            this.video_lblKeyFramerateSeconds.AutoSize = true;
            this.video_lblKeyFramerateSeconds.BackColor = System.Drawing.Color.Transparent;
            this.video_lblKeyFramerateSeconds.ForeColor = System.Drawing.Color.White;
            this.video_lblKeyFramerateSeconds.Location = new System.Drawing.Point(277, 127);
            this.video_lblKeyFramerateSeconds.Name = "video_lblKeyFramerateSeconds";
            this.video_lblKeyFramerateSeconds.Size = new System.Drawing.Size(56, 13);
            this.video_lblKeyFramerateSeconds.TabIndex = 11;
            this.video_lblKeyFramerateSeconds.Text = "3 seconds";
            // 
            // video_tbFramerate
            // 
            this.video_tbFramerate.BackColor = System.Drawing.Color.Transparent;
            this.video_tbFramerate.CurrentValue = 1;
            this.video_tbFramerate.CustomTicks = new double[] {
        1,
        3,
        5,
        10,
        15,
        30};
            this.video_tbFramerate.DisabledColor = System.Drawing.Color.Gray;
            this.video_tbFramerate.ForeColor = System.Drawing.Color.White;
            this.video_tbFramerate.Location = new System.Drawing.Point(112, 91);
            this.video_tbFramerate.Maximum = 30;
            this.video_tbFramerate.Minimum = 1;
            this.video_tbFramerate.Name = "video_tbFramerate";
            this.video_tbFramerate.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.video_tbFramerate.RestrictToCustomTicks = true;
            this.video_tbFramerate.Size = new System.Drawing.Size(156, 24);
            this.video_tbFramerate.TabIndex = 8;
            this.video_tbFramerate.TickColor = System.Drawing.Color.DarkGray;
            this.video_tbFramerate.TickFrequency = 1;
            this.video_tbFramerate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.video_tbFramerate.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.video_tbFramerate.TrackWidth = 4;
            this.video_tbFramerate.Value = 1;
            this.video_tbFramerate.ValueChanged += new System.EventHandler(this.tbFramerate_ValueChanged);
            this.video_tbFramerate.Scroll += new System.EventHandler(this.tbFramerate_Scroll);
            // 
            // video_tbKeyFramerate
            // 
            this.video_tbKeyFramerate.BackColor = System.Drawing.Color.Transparent;
            this.video_tbKeyFramerate.CurrentValue = 1;
            this.video_tbKeyFramerate.CustomTicks = null;
            this.video_tbKeyFramerate.DisabledColor = System.Drawing.Color.Gray;
            this.video_tbKeyFramerate.ForeColor = System.Drawing.Color.White;
            this.video_tbKeyFramerate.Location = new System.Drawing.Point(112, 121);
            this.video_tbKeyFramerate.Maximum = 180;
            this.video_tbKeyFramerate.Minimum = 1;
            this.video_tbKeyFramerate.Name = "video_tbKeyFramerate";
            this.video_tbKeyFramerate.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.video_tbKeyFramerate.RestrictToCustomTicks = false;
            this.video_tbKeyFramerate.Size = new System.Drawing.Size(156, 24);
            this.video_tbKeyFramerate.TabIndex = 10;
            this.video_tbKeyFramerate.TickColor = System.Drawing.Color.DarkGray;
            this.video_tbKeyFramerate.TickFrequency = 30;
            this.video_tbKeyFramerate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.video_tbKeyFramerate.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.video_tbKeyFramerate.TrackWidth = 4;
            this.video_tbKeyFramerate.Value = 1;
            this.video_tbKeyFramerate.ValueChanged += new System.EventHandler(this.video_tbKeyFramerate_ValueChanged);
            this.video_tbKeyFramerate.Scroll += new System.EventHandler(this.video_tbKeyFramerate_Scroll);
            // 
            // video_lblFramerateFPS
            // 
            this.video_lblFramerateFPS.AutoSize = true;
            this.video_lblFramerateFPS.BackColor = System.Drawing.Color.Transparent;
            this.video_lblFramerateFPS.ForeColor = System.Drawing.Color.White;
            this.video_lblFramerateFPS.Location = new System.Drawing.Point(277, 96);
            this.video_lblFramerateFPS.Name = "video_lblFramerateFPS";
            this.video_lblFramerateFPS.Size = new System.Drawing.Size(19, 13);
            this.video_lblFramerateFPS.TabIndex = 9;
            this.video_lblFramerateFPS.Text = "00";
            // 
            // video_MaxBitrate
            // 
            this.video_MaxBitrate.BackColor = System.Drawing.Color.Black;
            this.video_MaxBitrate.Label = "Maximum Bitrate:";
            this.video_MaxBitrate.Location = new System.Drawing.Point(18, 252);
            this.video_MaxBitrate.Maximum = 10240;
            this.video_MaxBitrate.Minimum = 32;
            this.video_MaxBitrate.MinimumSize = new System.Drawing.Size(290, 29);
            this.video_MaxBitrate.Name = "video_MaxBitrate";
            this.video_MaxBitrate.RestrictToPowersOfTwo = false;
            this.video_MaxBitrate.Size = new System.Drawing.Size(343, 29);
            this.video_MaxBitrate.TabIndex = 31;
            this.video_MaxBitrate.Value = 32;
            this.video_MaxBitrate.ValueChanged += new System.EventHandler(this.video_MaxBitrate_ValueChanged);
            // 
            // video_MinBitrate
            // 
            this.video_MinBitrate.BackColor = System.Drawing.Color.Black;
            this.video_MinBitrate.Label = "Minimum Bitrate:";
            this.video_MinBitrate.Location = new System.Drawing.Point(20, 220);
            this.video_MinBitrate.Maximum = 10240;
            this.video_MinBitrate.Minimum = 32;
            this.video_MinBitrate.MinimumSize = new System.Drawing.Size(290, 29);
            this.video_MinBitrate.Name = "video_MinBitrate";
            this.video_MinBitrate.RestrictToPowersOfTwo = false;
            this.video_MinBitrate.Size = new System.Drawing.Size(341, 29);
            this.video_MinBitrate.TabIndex = 30;
            this.video_MinBitrate.Value = 32;
            this.video_MinBitrate.ValueChanged += new System.EventHandler(this.video_MinBitrate_ValueChanged);
            // 
            // video_Bitrate
            // 
            this.video_Bitrate.BackColor = System.Drawing.Color.Black;
            this.video_Bitrate.Label = "";
            this.video_Bitrate.Location = new System.Drawing.Point(107, 162);
            this.video_Bitrate.Maximum = 10240;
            this.video_Bitrate.Minimum = 32;
            this.video_Bitrate.MinimumSize = new System.Drawing.Size(240, 29);
            this.video_Bitrate.Name = "video_Bitrate";
            this.video_Bitrate.RestrictToPowersOfTwo = false;
            this.video_Bitrate.Size = new System.Drawing.Size(254, 29);
            this.video_Bitrate.TabIndex = 29;
            this.video_Bitrate.Value = 32;
            this.video_Bitrate.ValueChanged += new System.EventHandler(this.video_Bitrate_ValueChanged);
            // 
            // tabAudio
            // 
            this.tabAudio.BackColor = System.Drawing.Color.Black;
            this.tabAudio.Controls.Add(this.audio_MaxBitrate);
            this.tabAudio.Controls.Add(this.audio_MinBitrate);
            this.tabAudio.Controls.Add(this.audio_Bitrate);
            this.tabAudio.Controls.Add(this.audio_radioIsVBR);
            this.tabAudio.Controls.Add(this.audio_radioIsCBR);
            this.tabAudio.Controls.Add(this.audio_lblCodec);
            this.tabAudio.Controls.Add(this.audio_cbCodec);
            this.tabAudio.ForeColor = System.Drawing.Color.White;
            this.tabAudio.Location = new System.Drawing.Point(4, 25);
            this.tabAudio.Name = "tabAudio";
            this.tabAudio.Padding = new System.Windows.Forms.Padding(3);
            this.tabAudio.Size = new System.Drawing.Size(434, 285);
            this.tabAudio.TabIndex = 1;
            this.tabAudio.Text = "Audio";
            // 
            // audio_MaxBitrate
            // 
            this.audio_MaxBitrate.BackColor = System.Drawing.Color.Black;
            this.audio_MaxBitrate.Label = "Maximum Bitrate:";
            this.audio_MaxBitrate.Location = new System.Drawing.Point(18, 123);
            this.audio_MaxBitrate.Maximum = 512;
            this.audio_MaxBitrate.Minimum = 4;
            this.audio_MaxBitrate.MinimumSize = new System.Drawing.Size(290, 29);
            this.audio_MaxBitrate.Name = "audio_MaxBitrate";
            this.audio_MaxBitrate.RestrictToPowersOfTwo = false;
            this.audio_MaxBitrate.Size = new System.Drawing.Size(343, 29);
            this.audio_MaxBitrate.TabIndex = 36;
            this.audio_MaxBitrate.Value = 32;
            this.audio_MaxBitrate.ValueChanged += new System.EventHandler(this.audio_MaxBitrate_ValueChanged);
            // 
            // audio_MinBitrate
            // 
            this.audio_MinBitrate.BackColor = System.Drawing.Color.Black;
            this.audio_MinBitrate.Label = "Minimum Bitrate:";
            this.audio_MinBitrate.Location = new System.Drawing.Point(20, 91);
            this.audio_MinBitrate.Maximum = 512;
            this.audio_MinBitrate.Minimum = 4;
            this.audio_MinBitrate.MinimumSize = new System.Drawing.Size(290, 29);
            this.audio_MinBitrate.Name = "audio_MinBitrate";
            this.audio_MinBitrate.RestrictToPowersOfTwo = false;
            this.audio_MinBitrate.Size = new System.Drawing.Size(341, 29);
            this.audio_MinBitrate.TabIndex = 35;
            this.audio_MinBitrate.Value = 32;
            this.audio_MinBitrate.ValueChanged += new System.EventHandler(this.audio_MinBitrate_ValueChanged);
            // 
            // audio_Bitrate
            // 
            this.audio_Bitrate.BackColor = System.Drawing.Color.Black;
            this.audio_Bitrate.Label = "";
            this.audio_Bitrate.Location = new System.Drawing.Point(107, 33);
            this.audio_Bitrate.Maximum = 512;
            this.audio_Bitrate.Minimum = 4;
            this.audio_Bitrate.MinimumSize = new System.Drawing.Size(240, 29);
            this.audio_Bitrate.Name = "audio_Bitrate";
            this.audio_Bitrate.RestrictToPowersOfTwo = false;
            this.audio_Bitrate.Size = new System.Drawing.Size(254, 29);
            this.audio_Bitrate.TabIndex = 34;
            this.audio_Bitrate.Value = 32;
            this.audio_Bitrate.ValueChanged += new System.EventHandler(this.audio_Bitrate_ValueChanged);
            // 
            // audio_radioIsVBR
            // 
            this.audio_radioIsVBR.AutoSize = true;
            this.audio_radioIsVBR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.audio_radioIsVBR.Location = new System.Drawing.Point(6, 67);
            this.audio_radioIsVBR.Name = "audio_radioIsVBR";
            this.audio_radioIsVBR.Size = new System.Drawing.Size(95, 17);
            this.audio_radioIsVBR.TabIndex = 33;
            this.audio_radioIsVBR.TabStop = true;
            this.audio_radioIsVBR.Text = "Variable Bitrate";
            this.audio_radioIsVBR.UseVisualStyleBackColor = true;
            this.audio_radioIsVBR.CheckedChanged += new System.EventHandler(this.audio_radioBitrate_CheckedChanged);
            // 
            // audio_radioIsCBR
            // 
            this.audio_radioIsCBR.AutoSize = true;
            this.audio_radioIsCBR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.audio_radioIsCBR.Location = new System.Drawing.Point(6, 38);
            this.audio_radioIsCBR.Name = "audio_radioIsCBR";
            this.audio_radioIsCBR.Size = new System.Drawing.Size(102, 17);
            this.audio_radioIsCBR.TabIndex = 32;
            this.audio_radioIsCBR.TabStop = true;
            this.audio_radioIsCBR.Text = "Constant Bitrate:";
            this.audio_radioIsCBR.UseVisualStyleBackColor = true;
            this.audio_radioIsCBR.CheckedChanged += new System.EventHandler(this.audio_radioBitrate_CheckedChanged);
            // 
            // audio_lblCodec
            // 
            this.audio_lblCodec.AutoSize = true;
            this.audio_lblCodec.BackColor = System.Drawing.Color.Transparent;
            this.audio_lblCodec.ForeColor = System.Drawing.Color.White;
            this.audio_lblCodec.Location = new System.Drawing.Point(65, 9);
            this.audio_lblCodec.Name = "audio_lblCodec";
            this.audio_lblCodec.Size = new System.Drawing.Size(41, 13);
            this.audio_lblCodec.TabIndex = 25;
            this.audio_lblCodec.Text = "Codec:";
            // 
            // audio_cbCodec
            // 
            this.audio_cbCodec.BackColor = System.Drawing.Color.Black;
            this.audio_cbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audio_cbCodec.ForeColor = System.Drawing.Color.White;
            this.audio_cbCodec.FormattingEnabled = true;
            this.audio_cbCodec.Items.AddRange(new object[] {
            "AMR"});
            this.audio_cbCodec.Location = new System.Drawing.Point(112, 6);
            this.audio_cbCodec.MaxDropDownItems = 30;
            this.audio_cbCodec.Name = "audio_cbCodec";
            this.audio_cbCodec.Size = new System.Drawing.Size(214, 21);
            this.audio_cbCodec.TabIndex = 26;
            this.audio_cbCodec.SelectedIndexChanged += new System.EventHandler(this.audio_cbCodec_SelectedIndexChanged);
            // 
            // CustomProfileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pContent);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CustomProfileEditor";
            this.Size = new System.Drawing.Size(442, 314);
            this.pContent.ResumeLayout(false);
            this.settingsTabs.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabVideo.ResumeLayout(false);
            this.tabVideo.PerformLayout();
            this.tabAudio.ResumeLayout(false);
            this.tabAudio.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pContent;
        private System.Windows.Forms.Label video_lblImageSize;
        private System.Windows.Forms.Label video_lblKeyFramerate;
        private System.Windows.Forms.Label video_lblFramerate;
        private System.Windows.Forms.ComboBox video_cbImageSize;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar video_tbFramerate;
        private System.Windows.Forms.Label video_lblFramerateFPS;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar video_tbKeyFramerate;
        private System.Windows.Forms.Label video_lblKeyFramerateSeconds;
        private System.Windows.Forms.ComboBox video_cbCodec;
        private System.Windows.Forms.Label video_lblCodec;
        private FutureConcepts.Controls.Common.FlatTabControl.FlatTabControl settingsTabs;
        private System.Windows.Forms.TabPage tabVideo;
        private System.Windows.Forms.TabPage tabAudio;
        private System.Windows.Forms.Label audio_lblCodec;
        private System.Windows.Forms.ComboBox audio_cbCodec;
        private System.Windows.Forms.TabPage tabGeneral;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton general_btnPreview;
        private System.Windows.Forms.TextBox general_txtName;
        private System.Windows.Forms.Label general_lblName;
        private System.Windows.Forms.CheckBox general_chkboxHasAudio;
        private System.Windows.Forms.CheckBox general_chkboxHasVideo;
        private System.Windows.Forms.ToolTip ttMain;
        private System.Windows.Forms.RadioButton video_radioIsVBR;
        private System.Windows.Forms.RadioButton video_radioIsCBR;
        private HorizontalBitrateSelector video_Bitrate;
        private HorizontalBitrateSelector video_MaxBitrate;
        private HorizontalBitrateSelector video_MinBitrate;
        private HorizontalBitrateSelector audio_MaxBitrate;
        private HorizontalBitrateSelector audio_MinBitrate;
        private HorizontalBitrateSelector audio_Bitrate;
        private System.Windows.Forms.RadioButton audio_radioIsVBR;
        private System.Windows.Forms.RadioButton audio_radioIsCBR;
        private System.Windows.Forms.Label video_lblQuality;
        private System.Windows.Forms.ComboBox video_cbQuality;
        private System.Windows.Forms.ComboBox video_cbFramerateUnits;
        private System.Windows.Forms.ComboBox general_cbProtocol;
        private System.Windows.Forms.Label general_lblProtocol;
    }
}
