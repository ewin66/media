using System.Drawing;
namespace FutureConcepts.Media.TV.Scanner
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ilSelectButton = new System.Windows.Forms.ImageList(this.components);
            this.panelMainPreview = new System.Windows.Forms.Panel();
            this.gbCurrentChannel = new System.Windows.Forms.GroupBox();
            this.lblCurrentChannel = new System.Windows.Forms.Label();
            this.gbVolume = new System.Windows.Forms.GroupBox();
            this.chkMute = new System.Windows.Forms.CheckBox();
            this.volDown = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilMinus = new System.Windows.Forms.ImageList(this.components);
            this.volUp = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilPlus = new System.Windows.Forms.ImageList(this.components);
            this.trackVolume = new FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar();
            this.ilMediumButton = new System.Windows.Forms.ImageList(this.components);
            this.btnChannelUp = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilChanUp = new System.Windows.Forms.ImageList(this.components);
            this.btnChannelDown = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilChanDown = new System.Windows.Forms.ImageList(this.components);
            this.btnChannelExplorer = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilFindChannels = new System.Windows.Forms.ImageList(this.components);
            this.ilSSMgr = new System.Windows.Forms.ImageList(this.components);
            this.btnSnapshot = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilSnapshot = new System.Windows.Forms.ImageList(this.components);
            this.btnAutoSnap = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilAutoSnap = new System.Windows.Forms.ImageList(this.components);
            this.ilScan = new System.Windows.Forms.ImageList(this.components);
            this.ilClear = new System.Windows.Forms.ImageList(this.components);
            this.timerAutoSnap = new System.Windows.Forms.Timer(this.components);
            this.lblAutoSnapping = new System.Windows.Forms.Label();
            this.btnRecord = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilRecord = new System.Windows.Forms.ImageList(this.components);
            this.ilAdvConfig = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.ilInfo = new System.Windows.Forms.ImageList(this.components);
            this.ilSatellite = new System.Windows.Forms.ImageList(this.components);
            this.ilBroadcast = new System.Windows.Forms.ImageList(this.components);
            this.ilNetwork = new System.Windows.Forms.ImageList(this.components);
            this.ilDirect = new System.Windows.Forms.ImageList(this.components);
            this.ilRecordMgr = new System.Windows.Forms.ImageList(this.components);
            this.panelBottomBar = new System.Windows.Forms.Panel();
            this.sourceInputSelector = new FutureConcepts.Media.TV.Scanner.SourceInputSelector();
            this.panelRecordingControls = new System.Windows.Forms.Panel();
            this.channelKeypad = new FutureConcepts.Media.TV.Scanner.ChannelKeypad();
            this.panelRightSidebar = new System.Windows.Forms.Panel();
            this.panelStatusText = new System.Windows.Forms.Panel();
            this.lblRecording = new System.Windows.Forms.Label();
            this.lblScanning = new System.Windows.Forms.Label();
            this.panelFavoriteChannelsHeader = new System.Windows.Forms.Panel();
            this.btnShowHideFavoriteChannels = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilHideFavoriteChannels = new System.Windows.Forms.ImageList(this.components);
            this.lblFavoriteChannelsHeader = new System.Windows.Forms.Label();
            this.tmrBlink = new System.Windows.Forms.Timer(this.components);
            this.ilStopRec = new System.Windows.Forms.ImageList(this.components);
            this.panelCenterFrame = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ilShowFavoriteChannels = new System.Windows.Forms.ImageList(this.components);
            this.ttChannel = new System.Windows.Forms.ToolTip(this.components);
            this.ttPerformanceWarning = new System.Windows.Forms.ToolTip(this.components);
            this.favoriteChannels = new FutureConcepts.Media.TV.Scanner.FavoriteChannels();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.BasePanel.SuspendLayout();
            this.gbCurrentChannel.SuspendLayout();
            this.gbVolume.SuspendLayout();
            this.panelBottomBar.SuspendLayout();
            this.panelRecordingControls.SuspendLayout();
            this.panelRightSidebar.SuspendLayout();
            this.panelStatusText.SuspendLayout();
            this.panelFavoriteChannelsHeader.SuspendLayout();
            this.panelCenterFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Size = new System.Drawing.Size(1020, 722);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panelCenterFrame);
            this.panel6.Controls.Add(this.favoriteChannels);
            this.panel6.Controls.Add(this.panelStatusText);
            this.panel6.Controls.Add(this.panelRightSidebar);
            this.panel6.Controls.Add(this.panelBottomBar);
            this.panel6.Size = new System.Drawing.Size(1020, 722);
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(982, 0);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(1020, 764);
            // 
            // ilSelectButton
            // 
            this.ilSelectButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSelectButton.ImageStream")));
            this.ilSelectButton.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSelectButton.Images.SetKeyName(0, "select-up.gif");
            this.ilSelectButton.Images.SetKeyName(1, "select-down.gif");
            // 
            // panelMainPreview
            // 
            this.panelMainPreview.BackColor = System.Drawing.Color.Black;
            this.panelMainPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainPreview.Location = new System.Drawing.Point(0, 0);
            this.panelMainPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelMainPreview.Name = "panelMainPreview";
            this.panelMainPreview.Size = new System.Drawing.Size(759, 611);
            this.panelMainPreview.TabIndex = 1;
            this.panelMainPreview.SizeChanged += new System.EventHandler(this.panelMainPreview_SizeChanged);
            // 
            // gbCurrentChannel
            // 
            this.gbCurrentChannel.Controls.Add(this.lblCurrentChannel);
            this.gbCurrentChannel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbCurrentChannel.ForeColor = System.Drawing.Color.White;
            this.gbCurrentChannel.Location = new System.Drawing.Point(3, 1);
            this.gbCurrentChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbCurrentChannel.Name = "gbCurrentChannel";
            this.gbCurrentChannel.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbCurrentChannel.Size = new System.Drawing.Size(79, 74);
            this.gbCurrentChannel.TabIndex = 2;
            this.gbCurrentChannel.TabStop = false;
            this.gbCurrentChannel.Text = "Current";
            // 
            // lblCurrentChannel
            // 
            this.lblCurrentChannel.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentChannel.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentChannel.ForeColor = System.Drawing.Color.Yellow;
            this.lblCurrentChannel.Location = new System.Drawing.Point(0, 20);
            this.lblCurrentChannel.Name = "lblCurrentChannel";
            this.lblCurrentChannel.Size = new System.Drawing.Size(79, 50);
            this.lblCurrentChannel.TabIndex = 0;
            this.lblCurrentChannel.Text = "99.9";
            this.lblCurrentChannel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbVolume
            // 
            this.gbVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbVolume.Controls.Add(this.chkMute);
            this.gbVolume.Controls.Add(this.volDown);
            this.gbVolume.Controls.Add(this.volUp);
            this.gbVolume.Controls.Add(this.trackVolume);
            this.gbVolume.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbVolume.ForeColor = System.Drawing.Color.White;
            this.gbVolume.Location = new System.Drawing.Point(8, 17);
            this.gbVolume.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbVolume.Name = "gbVolume";
            this.gbVolume.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbVolume.Size = new System.Drawing.Size(49, 269);
            this.gbVolume.TabIndex = 5;
            this.gbVolume.TabStop = false;
            this.gbVolume.Text = "Vol";
            // 
            // chkMute
            // 
            this.chkMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMute.AutoSize = true;
            this.chkMute.BackColor = System.Drawing.Color.Transparent;
            this.chkMute.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMute.ForeColor = System.Drawing.Color.White;
            this.chkMute.Location = new System.Drawing.Point(6, 251);
            this.chkMute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkMute.Name = "chkMute";
            this.chkMute.Size = new System.Drawing.Size(45, 15);
            this.chkMute.TabIndex = 7;
            this.chkMute.Text = "Mute";
            this.chkMute.UseVisualStyleBackColor = false;
            this.chkMute.CheckedChanged += new System.EventHandler(this.chkMute_CheckedChanged);
            // 
            // volDown
            // 
            this.volDown.ImageIndex = 0;
            this.volDown.ImageList = this.ilMinus;
            this.volDown.Location = new System.Drawing.Point(14, 225);
            this.volDown.Name = "volDown";
            this.volDown.Size = new System.Drawing.Size(22, 22);
            this.volDown.TabIndex = 2;
            this.volDown.Toggle = false;
            this.volDown.Click += new System.EventHandler(this.volDown_Click);
            // 
            // ilMinus
            // 
            this.ilMinus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMinus.ImageStream")));
            this.ilMinus.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMinus.Images.SetKeyName(0, "volumedown-up.gif");
            this.ilMinus.Images.SetKeyName(1, "volumedown-down.gif");
            // 
            // volUp
            // 
            this.volUp.ImageIndex = 0;
            this.volUp.ImageList = this.ilPlus;
            this.volUp.Location = new System.Drawing.Point(14, 19);
            this.volUp.Name = "volUp";
            this.volUp.Size = new System.Drawing.Size(22, 22);
            this.volUp.TabIndex = 3;
            this.volUp.Toggle = false;
            this.volUp.Click += new System.EventHandler(this.volUp_Click);
            // 
            // ilPlus
            // 
            this.ilPlus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilPlus.ImageStream")));
            this.ilPlus.TransparentColor = System.Drawing.Color.Transparent;
            this.ilPlus.Images.SetKeyName(0, "volumeup-up.gif");
            this.ilPlus.Images.SetKeyName(1, "volumeup-down.gif");
            // 
            // trackVolume
            // 
            this.trackVolume.BackColor = System.Drawing.Color.Transparent;
            this.trackVolume.CurrentValue = 0D;
            this.trackVolume.CustomTicks = null;
            this.trackVolume.DisabledColor = System.Drawing.Color.Gray;
            this.trackVolume.ForeColor = System.Drawing.Color.White;
            this.trackVolume.Location = new System.Drawing.Point(11, 44);
            this.trackVolume.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackVolume.Maximum = 0D;
            this.trackVolume.Minimum = -10D;
            this.trackVolume.Name = "trackVolume";
            this.trackVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackVolume.RestrictToCustomTicks = true;
            this.trackVolume.Size = new System.Drawing.Size(28, 178);
            this.trackVolume.TabIndex = 1;
            this.trackVolume.TickColor = System.Drawing.Color.DarkGray;
            this.trackVolume.TickFrequency = 0D;
            this.trackVolume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackVolume.TrackClickStepValue = 1D;
            this.trackVolume.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.trackVolume.TrackWidth = 5;
            this.trackVolume.Value = 0D;
            this.trackVolume.Scroll += new System.EventHandler(this.trackVolume_Scroll);
            this.trackVolume.ValueChanged += new System.EventHandler(this.trackVolume_ValueChanged);
            // 
            // ilMediumButton
            // 
            this.ilMediumButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMediumButton.ImageStream")));
            this.ilMediumButton.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMediumButton.Images.SetKeyName(0, "mediumbutton-up.GIF");
            this.ilMediumButton.Images.SetKeyName(1, "mediumbutton-down.GIF");
            // 
            // btnChannelUp
            // 
            this.btnChannelUp.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChannelUp.ForeColor = System.Drawing.Color.White;
            this.btnChannelUp.ImageIndex = 0;
            this.btnChannelUp.ImageList = this.ilChanUp;
            this.btnChannelUp.Location = new System.Drawing.Point(92, 9);
            this.btnChannelUp.Margin = new System.Windows.Forms.Padding(1);
            this.btnChannelUp.Name = "btnChannelUp";
            this.btnChannelUp.Size = new System.Drawing.Size(90, 30);
            this.btnChannelUp.TabIndex = 27;
            this.btnChannelUp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnChannelUp.Toggle = false;
            this.tt.SetToolTip(this.btnChannelUp, "Go up to the next available channel");
            this.btnChannelUp.Click += new System.EventHandler(this.btnChannelUp_Click);
            // 
            // ilChanUp
            // 
            this.ilChanUp.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilChanUp.ImageStream")));
            this.ilChanUp.TransparentColor = System.Drawing.Color.Transparent;
            this.ilChanUp.Images.SetKeyName(0, "channelup-up.gif");
            this.ilChanUp.Images.SetKeyName(1, "channelup-down.gif");
            // 
            // btnChannelDown
            // 
            this.btnChannelDown.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChannelDown.ForeColor = System.Drawing.Color.White;
            this.btnChannelDown.ImageIndex = 0;
            this.btnChannelDown.ImageList = this.ilChanDown;
            this.btnChannelDown.Location = new System.Drawing.Point(92, 45);
            this.btnChannelDown.Margin = new System.Windows.Forms.Padding(1);
            this.btnChannelDown.Name = "btnChannelDown";
            this.btnChannelDown.Size = new System.Drawing.Size(90, 30);
            this.btnChannelDown.TabIndex = 28;
            this.btnChannelDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnChannelDown.Toggle = false;
            this.tt.SetToolTip(this.btnChannelDown, "Go down to the next available channel");
            this.btnChannelDown.Click += new System.EventHandler(this.btnChannelDown_Click);
            // 
            // ilChanDown
            // 
            this.ilChanDown.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilChanDown.ImageStream")));
            this.ilChanDown.TransparentColor = System.Drawing.Color.Transparent;
            this.ilChanDown.Images.SetKeyName(0, "channeldown-up.gif");
            this.ilChanDown.Images.SetKeyName(1, "channeldown-down.gif");
            // 
            // btnChannelExplorer
            // 
            this.btnChannelExplorer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChannelExplorer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChannelExplorer.ForeColor = System.Drawing.Color.White;
            this.btnChannelExplorer.ImageIndex = 0;
            this.btnChannelExplorer.ImageList = this.ilFindChannels;
            this.btnChannelExplorer.Location = new System.Drawing.Point(3, 291);
            this.btnChannelExplorer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChannelExplorer.Name = "btnChannelExplorer";
            this.btnChannelExplorer.Size = new System.Drawing.Size(60, 30);
            this.btnChannelExplorer.TabIndex = 29;
            this.btnChannelExplorer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnChannelExplorer.Toggle = false;
            this.tt.SetToolTip(this.btnChannelExplorer, "Automatically scan for channels\r\navailable in the area.");
            this.btnChannelExplorer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnChannelExplorer_Click);
            // 
            // ilFindChannels
            // 
            this.ilFindChannels.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFindChannels.ImageStream")));
            this.ilFindChannels.TransparentColor = System.Drawing.Color.Transparent;
            this.ilFindChannels.Images.SetKeyName(0, "findchannels-up.gif");
            this.ilFindChannels.Images.SetKeyName(1, "findchannels-down.gif");
            // 
            // ilSSMgr
            // 
            this.ilSSMgr.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSSMgr.ImageStream")));
            this.ilSSMgr.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSSMgr.Images.SetKeyName(0, "snapshotmgr-up.gif");
            this.ilSSMgr.Images.SetKeyName(1, "snapshotmgr-down.gif");
            // 
            // btnSnapshot
            // 
            this.btnSnapshot.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSnapshot.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSnapshot.ForeColor = System.Drawing.Color.White;
            this.btnSnapshot.ImageIndex = 0;
            this.btnSnapshot.ImageList = this.ilSnapshot;
            this.btnSnapshot.Location = new System.Drawing.Point(61, 0);
            this.btnSnapshot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSnapshot.Name = "btnSnapshot";
            this.btnSnapshot.Size = new System.Drawing.Size(61, 69);
            this.btnSnapshot.TabIndex = 32;
            this.btnSnapshot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSnapshot.Toggle = false;
            this.tt.SetToolTip(this.btnSnapshot, "Take a snapshot of the current video display");
            this.btnSnapshot.Click += new System.EventHandler(this.btnSnapshot_Click);
            // 
            // ilSnapshot
            // 
            this.ilSnapshot.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSnapshot.ImageStream")));
            this.ilSnapshot.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSnapshot.Images.SetKeyName(0, "snapshot-up.gif");
            this.ilSnapshot.Images.SetKeyName(1, "snapshot-down.gif");
            // 
            // btnAutoSnap
            // 
            this.btnAutoSnap.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAutoSnap.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoSnap.ForeColor = System.Drawing.Color.White;
            this.btnAutoSnap.ImageIndex = 0;
            this.btnAutoSnap.ImageList = this.ilAutoSnap;
            this.btnAutoSnap.Location = new System.Drawing.Point(122, 0);
            this.btnAutoSnap.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoSnap.Name = "btnAutoSnap";
            this.btnAutoSnap.Size = new System.Drawing.Size(61, 69);
            this.btnAutoSnap.TabIndex = 33;
            this.btnAutoSnap.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnAutoSnap.Toggle = true;
            this.tt.SetToolTip(this.btnAutoSnap, "Automatically take snapshots at a set interval");
            this.btnAutoSnap.Click += new System.EventHandler(this.btnAutoSnap_Click);
            // 
            // ilAutoSnap
            // 
            this.ilAutoSnap.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilAutoSnap.ImageStream")));
            this.ilAutoSnap.TransparentColor = System.Drawing.Color.Transparent;
            this.ilAutoSnap.Images.SetKeyName(0, "autosnap-up.gif");
            this.ilAutoSnap.Images.SetKeyName(1, "autosnap-down.gif");
            // 
            // ilScan
            // 
            this.ilScan.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilScan.ImageStream")));
            this.ilScan.TransparentColor = System.Drawing.Color.Transparent;
            this.ilScan.Images.SetKeyName(0, "scanfavorites-up.gif");
            this.ilScan.Images.SetKeyName(1, "scanfavorites-down.gif");
            // 
            // ilClear
            // 
            this.ilClear.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilClear.ImageStream")));
            this.ilClear.TransparentColor = System.Drawing.Color.Transparent;
            this.ilClear.Images.SetKeyName(0, "clearfavorites-up.gif");
            this.ilClear.Images.SetKeyName(1, "clearfavorites-down.gif");
            // 
            // timerAutoSnap
            // 
            this.timerAutoSnap.Interval = 1000;
            this.timerAutoSnap.Tick += new System.EventHandler(this.timerAutoSnap_Tick);
            // 
            // lblAutoSnapping
            // 
            this.lblAutoSnapping.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblAutoSnapping.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoSnapping.ForeColor = System.Drawing.Color.Lime;
            this.lblAutoSnapping.Location = new System.Drawing.Point(824, 0);
            this.lblAutoSnapping.Name = "lblAutoSnapping";
            this.lblAutoSnapping.Size = new System.Drawing.Size(129, 26);
            this.lblAutoSnapping.TabIndex = 43;
            this.lblAutoSnapping.Text = "Auto Snapping...";
            this.lblAutoSnapping.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAutoSnapping.Visible = false;
            // 
            // btnRecord
            // 
            this.btnRecord.BackColor = System.Drawing.Color.Black;
            this.btnRecord.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRecord.ForeColor = System.Drawing.Color.White;
            this.btnRecord.ImageIndex = 0;
            this.btnRecord.ImageList = this.ilRecord;
            this.btnRecord.Location = new System.Drawing.Point(0, 0);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(61, 69);
            this.btnRecord.TabIndex = 49;
            this.btnRecord.Tag = "Record";
            this.btnRecord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnRecord.Toggle = false;
            this.tt.SetToolTip(this.btnRecord, "Record the current channel to a file for later viewing");
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // ilRecord
            // 
            this.ilRecord.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRecord.ImageStream")));
            this.ilRecord.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRecord.Images.SetKeyName(0, "record-up.gif");
            this.ilRecord.Images.SetKeyName(1, "record-down.gif");
            // 
            // ilAdvConfig
            // 
            this.ilAdvConfig.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilAdvConfig.ImageStream")));
            this.ilAdvConfig.TransparentColor = System.Drawing.Color.Transparent;
            this.ilAdvConfig.Images.SetKeyName(0, "advconfig-up.gif");
            this.ilAdvConfig.Images.SetKeyName(1, "advconfig-down.gif");
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // ilInfo
            // 
            this.ilInfo.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilInfo.ImageStream")));
            this.ilInfo.TransparentColor = System.Drawing.Color.Transparent;
            this.ilInfo.Images.SetKeyName(0, "chinfo-up.gif");
            this.ilInfo.Images.SetKeyName(1, "chinfo-down.gif");
            // 
            // ilSatellite
            // 
            this.ilSatellite.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSatellite.ImageStream")));
            this.ilSatellite.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSatellite.Images.SetKeyName(0, "satellite-up.gif");
            this.ilSatellite.Images.SetKeyName(1, "satellite-down.gif");
            this.ilSatellite.Images.SetKeyName(2, "satellite-down-disabled.gif");
            // 
            // ilBroadcast
            // 
            this.ilBroadcast.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilBroadcast.ImageStream")));
            this.ilBroadcast.TransparentColor = System.Drawing.Color.Transparent;
            this.ilBroadcast.Images.SetKeyName(0, "broadcast-up.gif");
            this.ilBroadcast.Images.SetKeyName(1, "broadcast-down.gif");
            this.ilBroadcast.Images.SetKeyName(2, "broadcast-down-disabled.gif");
            // 
            // ilNetwork
            // 
            this.ilNetwork.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNetwork.ImageStream")));
            this.ilNetwork.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNetwork.Images.SetKeyName(0, "network-up.gif");
            this.ilNetwork.Images.SetKeyName(1, "network-down.gif");
            this.ilNetwork.Images.SetKeyName(2, "network-down-disabled.gif");
            // 
            // ilDirect
            // 
            this.ilDirect.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilDirect.ImageStream")));
            this.ilDirect.TransparentColor = System.Drawing.Color.Transparent;
            this.ilDirect.Images.SetKeyName(0, "direct-up.gif");
            this.ilDirect.Images.SetKeyName(1, "direct-down.gif");
            this.ilDirect.Images.SetKeyName(2, "local-down-disabled.gif");
            // 
            // ilRecordMgr
            // 
            this.ilRecordMgr.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRecordMgr.ImageStream")));
            this.ilRecordMgr.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRecordMgr.Images.SetKeyName(0, "media-up.gif");
            this.ilRecordMgr.Images.SetKeyName(1, "media-down.gif");
            // 
            // panelBottomBar
            // 
            this.panelBottomBar.Controls.Add(this.sourceInputSelector);
            this.panelBottomBar.Controls.Add(this.panelRecordingControls);
            this.panelBottomBar.Controls.Add(this.gbCurrentChannel);
            this.panelBottomBar.Controls.Add(this.channelKeypad);
            this.panelBottomBar.Controls.Add(this.btnChannelDown);
            this.panelBottomBar.Controls.Add(this.btnChannelUp);
            this.panelBottomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomBar.Location = new System.Drawing.Point(0, 641);
            this.panelBottomBar.Name = "panelBottomBar";
            this.panelBottomBar.Size = new System.Drawing.Size(1020, 81);
            this.panelBottomBar.TabIndex = 51;
            // 
            // sourceInputSelector
            // 
            this.sourceInputSelector.BackColor = System.Drawing.Color.Black;
            this.sourceInputSelector.Enabled = false;
            this.sourceInputSelector.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceInputSelector.ForeColor = System.Drawing.Color.White;
            this.sourceInputSelector.Location = new System.Drawing.Point(409, 6);
            this.sourceInputSelector.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.sourceInputSelector.Name = "sourceInputSelector";
            this.sourceInputSelector.SelectedInput = FutureConcepts.Media.TVMode.Broadcast;
            this.sourceInputSelector.SelectedSource = FutureConcepts.Media.TVSource.LocalAnalog;
            this.sourceInputSelector.Size = new System.Drawing.Size(246, 67);
            this.sourceInputSelector.TabIndex = 57;
            this.sourceInputSelector.SelectedSourceChanged += new System.EventHandler(this.sourceInputSelector_SelectedSourceChanged);
            this.sourceInputSelector.SelectedInputChanged += new System.EventHandler(this.sourceInputSelector_SelectedInputChanged);
            // 
            // panelRecordingControls
            // 
            this.panelRecordingControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRecordingControls.Controls.Add(this.btnAutoSnap);
            this.panelRecordingControls.Controls.Add(this.btnSnapshot);
            this.panelRecordingControls.Controls.Add(this.btnRecord);
            this.panelRecordingControls.Location = new System.Drawing.Point(768, 6);
            this.panelRecordingControls.Name = "panelRecordingControls";
            this.panelRecordingControls.Size = new System.Drawing.Size(185, 69);
            this.panelRecordingControls.TabIndex = 55;
            // 
            // channelKeypad
            // 
            this.channelKeypad.BackColor = System.Drawing.Color.White;
            this.channelKeypad.Channel = null;
            this.channelKeypad.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelKeypad.ForeColor = System.Drawing.Color.White;
            this.channelKeypad.Location = new System.Drawing.Point(190, 7);
            this.channelKeypad.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.channelKeypad.Name = "channelKeypad";
            this.channelKeypad.Padding = new System.Windows.Forms.Padding(2);
            this.channelKeypad.Size = new System.Drawing.Size(202, 67);
            this.channelKeypad.TabIndex = 37;
            this.channelKeypad.ChannelEntered += new System.EventHandler(this.channelKeypad_ChannelEntered);
            this.channelKeypad.ChannelChanging += new System.EventHandler(this.channelKeypad_ChannelChanging);
            // 
            // panelRightSidebar
            // 
            this.panelRightSidebar.Controls.Add(this.gbVolume);
            this.panelRightSidebar.Controls.Add(this.btnChannelExplorer);
            this.panelRightSidebar.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRightSidebar.Location = new System.Drawing.Point(953, 0);
            this.panelRightSidebar.Name = "panelRightSidebar";
            this.panelRightSidebar.Size = new System.Drawing.Size(67, 641);
            this.panelRightSidebar.TabIndex = 53;
            // 
            // panelStatusText
            // 
            this.panelStatusText.Controls.Add(this.lblRecording);
            this.panelStatusText.Controls.Add(this.lblScanning);
            this.panelStatusText.Controls.Add(this.lblAutoSnapping);
            this.panelStatusText.Controls.Add(this.panelFavoriteChannelsHeader);
            this.panelStatusText.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatusText.Location = new System.Drawing.Point(0, 0);
            this.panelStatusText.Name = "panelStatusText";
            this.panelStatusText.Size = new System.Drawing.Size(953, 26);
            this.panelStatusText.TabIndex = 54;
            // 
            // lblRecording
            // 
            this.lblRecording.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRecording.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecording.ForeColor = System.Drawing.Color.Red;
            this.lblRecording.Location = new System.Drawing.Point(718, 0);
            this.lblRecording.Name = "lblRecording";
            this.lblRecording.Size = new System.Drawing.Size(106, 26);
            this.lblRecording.TabIndex = 45;
            this.lblRecording.Text = "Recording...";
            this.lblRecording.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblRecording.Visible = false;
            // 
            // lblScanning
            // 
            this.lblScanning.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblScanning.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScanning.ForeColor = System.Drawing.Color.Lime;
            this.lblScanning.Location = new System.Drawing.Point(190, 0);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(218, 26);
            this.lblScanning.TabIndex = 44;
            this.lblScanning.Text = "Scanning Favorite Channels...";
            this.lblScanning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblScanning.Visible = false;
            // 
            // panelFavoriteChannelsHeader
            // 
            this.panelFavoriteChannelsHeader.Controls.Add(this.btnShowHideFavoriteChannels);
            this.panelFavoriteChannelsHeader.Controls.Add(this.lblFavoriteChannelsHeader);
            this.panelFavoriteChannelsHeader.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelFavoriteChannelsHeader.Location = new System.Drawing.Point(0, 0);
            this.panelFavoriteChannelsHeader.Name = "panelFavoriteChannelsHeader";
            this.panelFavoriteChannelsHeader.Padding = new System.Windows.Forms.Padding(3);
            this.panelFavoriteChannelsHeader.Size = new System.Drawing.Size(190, 26);
            this.panelFavoriteChannelsHeader.TabIndex = 0;
            // 
            // btnShowHideFavoriteChannels
            // 
            this.btnShowHideFavoriteChannels.ImageIndex = 0;
            this.btnShowHideFavoriteChannels.ImageList = this.ilHideFavoriteChannels;
            this.btnShowHideFavoriteChannels.Location = new System.Drawing.Point(149, 3);
            this.btnShowHideFavoriteChannels.Name = "btnShowHideFavoriteChannels";
            this.btnShowHideFavoriteChannels.Size = new System.Drawing.Size(20, 20);
            this.btnShowHideFavoriteChannels.TabIndex = 1;
            this.btnShowHideFavoriteChannels.Toggle = false;
            this.btnShowHideFavoriteChannels.Click += new System.EventHandler(this.btnShowHideFavoriteChannels_Click);
            // 
            // ilHideFavoriteChannels
            // 
            this.ilHideFavoriteChannels.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilHideFavoriteChannels.ImageStream")));
            this.ilHideFavoriteChannels.TransparentColor = System.Drawing.Color.Transparent;
            this.ilHideFavoriteChannels.Images.SetKeyName(0, "hidechannels-up.gif");
            this.ilHideFavoriteChannels.Images.SetKeyName(1, "hidechannels-down.gif");
            // 
            // lblFavoriteChannelsHeader
            // 
            this.lblFavoriteChannelsHeader.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFavoriteChannelsHeader.Location = new System.Drawing.Point(3, 1);
            this.lblFavoriteChannelsHeader.Name = "lblFavoriteChannelsHeader";
            this.lblFavoriteChannelsHeader.Size = new System.Drawing.Size(145, 20);
            this.lblFavoriteChannelsHeader.TabIndex = 0;
            this.lblFavoriteChannelsHeader.Text = "Favorite Channels";
            this.lblFavoriteChannelsHeader.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tmrBlink
            // 
            this.tmrBlink.Interval = 750;
            this.tmrBlink.Tick += new System.EventHandler(this.tmrBlink_Tick);
            // 
            // ilStopRec
            // 
            this.ilStopRec.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStopRec.ImageStream")));
            this.ilStopRec.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStopRec.Images.SetKeyName(0, "stoprec-up.gif");
            this.ilStopRec.Images.SetKeyName(1, "stoprec-down.gif");
            // 
            // panelCenterFrame
            // 
            this.panelCenterFrame.BackColor = System.Drawing.Color.Black;
            this.panelCenterFrame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelCenterFrame.Controls.Add(this.panelMainPreview);
            this.panelCenterFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenterFrame.Location = new System.Drawing.Point(190, 26);
            this.panelCenterFrame.Name = "panelCenterFrame";
            this.panelCenterFrame.Size = new System.Drawing.Size(763, 615);
            this.panelCenterFrame.TabIndex = 55;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 0;
            // 
            // ilShowFavoriteChannels
            // 
            this.ilShowFavoriteChannels.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilShowFavoriteChannels.ImageStream")));
            this.ilShowFavoriteChannels.TransparentColor = System.Drawing.Color.Transparent;
            this.ilShowFavoriteChannels.Images.SetKeyName(0, "showchannels-up.gif");
            this.ilShowFavoriteChannels.Images.SetKeyName(1, "showchannels-down.gif");
            // 
            // ttChannel
            // 
            this.ttChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ttChannel.ForeColor = System.Drawing.Color.White;
            this.ttChannel.OwnerDraw = true;
            this.ttChannel.ShowAlways = true;
            this.ttChannel.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ttChannel_Draw);
            this.ttChannel.Popup += new System.Windows.Forms.PopupEventHandler(this.ttChannel_Popup);
            // 
            // ttPerformanceWarning
            // 
            this.ttPerformanceWarning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ttPerformanceWarning.ForeColor = System.Drawing.Color.White;
            this.ttPerformanceWarning.ShowAlways = true;
            this.ttPerformanceWarning.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Warning;
            this.ttPerformanceWarning.ToolTipTitle = "Performance Warning";
            // 
            // favoriteChannels
            // 
            this.favoriteChannels.BackColor = System.Drawing.Color.Black;
            this.favoriteChannels.Channel = null;
            this.favoriteChannels.Dock = System.Windows.Forms.DockStyle.Left;
            this.favoriteChannels.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.favoriteChannels.ForeColor = System.Drawing.Color.White;
            this.favoriteChannels.Location = new System.Drawing.Point(0, 26);
            this.favoriteChannels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.favoriteChannels.Name = "favoriteChannels";
            this.favoriteChannels.Scanning = false;
            this.favoriteChannels.Size = new System.Drawing.Size(190, 615);
            this.favoriteChannels.TabIndex = 52;
            this.favoriteChannels.AddCurrentAsFavorite += new System.EventHandler(this.favoriteChannels_AddCurrentAsFavorite);
            this.favoriteChannels.TuneFavoriteChannel += new FutureConcepts.Media.TV.Scanner.FavoriteChannels.TuneFavoriteChannelHandler(this.favoriteChannels_TuneFavoriteChannel);
            this.favoriteChannels.ScanningStateChanged += new System.EventHandler(this.favoriteChannels_ScanningStateChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.HasCloseButton = true;
            this.HasControlBox = true;
            this.HasVersion = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "MainForm";
            this.Text = "AntaresX Television Scanner";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.BasePanel.ResumeLayout(false);
            this.gbCurrentChannel.ResumeLayout(false);
            this.gbVolume.ResumeLayout(false);
            this.gbVolume.PerformLayout();
            this.panelBottomBar.ResumeLayout(false);
            this.panelRecordingControls.ResumeLayout(false);
            this.panelRightSidebar.ResumeLayout(false);
            this.panelStatusText.ResumeLayout(false);
            this.panelFavoriteChannelsHeader.ResumeLayout(false);
            this.panelCenterFrame.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMainPreview;
        private System.Windows.Forms.GroupBox gbCurrentChannel;
        private System.Windows.Forms.GroupBox gbVolume;
        private System.Windows.Forms.CheckBox chkMute;
        private System.Windows.Forms.Label lblCurrentChannel;
        private System.Windows.Forms.ImageList ilSelectButton;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnChannelUp;
        private System.Windows.Forms.ImageList ilMediumButton;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnChannelDown;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnChannelExplorer;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnSnapshot;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnAutoSnap;
        private ChannelKeypad channelKeypad;
        private System.Windows.Forms.Timer timerAutoSnap;
        private System.Windows.Forms.Label lblAutoSnapping;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnRecord;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Panel panelBottomBar;
        private System.Windows.Forms.Panel panelStatusText;
        private System.Windows.Forms.Panel panelRightSidebar;
        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Timer tmrBlink;
        private System.Windows.Forms.ImageList ilRecord;
        private System.Windows.Forms.Label lblRecording;
        private System.Windows.Forms.ImageList ilChanDown;
        private System.Windows.Forms.ImageList ilChanUp;
        private System.Windows.Forms.ImageList ilAutoSnap;
        private System.Windows.Forms.ImageList ilSnapshot;
        private System.Windows.Forms.ImageList ilClear;
        private System.Windows.Forms.ImageList ilScan;
        private System.Windows.Forms.ImageList ilSSMgr;
        private System.Windows.Forms.ImageList ilAdvConfig;
        private System.Windows.Forms.ImageList ilFindChannels;
        private System.Windows.Forms.ImageList ilRecordMgr;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar trackVolume;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton volDown;
        private System.Windows.Forms.ImageList ilMinus;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton volUp;
        private System.Windows.Forms.ImageList ilPlus;
        private System.Windows.Forms.ImageList ilStopRec;
        private System.Windows.Forms.Panel panelCenterFrame;
        private System.Windows.Forms.ImageList ilSatellite;
        private System.Windows.Forms.ImageList ilBroadcast;
        private System.Windows.Forms.ImageList ilNetwork;
        private System.Windows.Forms.Panel panelRecordingControls;
        private System.Windows.Forms.ImageList ilInfo;
        private System.Windows.Forms.ImageList ilDirect;
        private SourceInputSelector sourceInputSelector;
        private FavoriteChannels favoriteChannels;



        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelFavoriteChannelsHeader;
        private System.Windows.Forms.Label lblFavoriteChannelsHeader;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnShowHideFavoriteChannels;
        private System.Windows.Forms.ImageList ilHideFavoriteChannels;
        private System.Windows.Forms.ImageList ilShowFavoriteChannels;
        private System.Windows.Forms.ToolTip ttChannel;
        private System.Windows.Forms.ToolTip ttPerformanceWarning;
    }
}

