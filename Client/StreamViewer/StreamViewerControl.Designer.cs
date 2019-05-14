using System;
using FutureConcepts.Tools;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace FutureConcepts.Media.Client.StreamViewer
{
    partial class StreamViewerControl
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
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(Dispose), disposing);
                return;
            }

            try
            {
                DisposeGraph();

                if (disposing)
                {
                    iconBusy.Image = null;

                    if (_deviceControl != null)
                    {
                        _deviceControl.Dispose();
                        _deviceControl = null;
                    }

                    if (components != null)
                    {
                        components.Dispose();
                    }
                }

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action<bool>(base.Dispose), disposing);
                }
                else
                {
                    base.Dispose(disposing);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        private void DisposeGraph()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(DisposeGraph));
                return;
            }

            try
            {
                if (_graph != null)
                {
                    _graph.Dispose();
                    _graph = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamViewerControl));
            this.ilPlay = new System.Windows.Forms.ImageList(this.components);
            this.ilStop = new System.Windows.Forms.ImageList(this.components);
            this.P_Video = new System.Windows.Forms.Panel();
            this.ilSingleView = new System.Windows.Forms.ImageList(this.components);
            this.P_FullControls = new System.Windows.Forms.Panel();
            this.L_StreamDescription = new System.Windows.Forms.Label();
            this.B_SingleView = new System.Windows.Forms.Label();
            this.B_FullScreen = new System.Windows.Forms.Label();
            this.ilFullScreen = new System.Windows.Forms.ImageList(this.components);
            this.B_Snapshot = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilSnapshot = new System.Windows.Forms.ImageList(this.components);
            this.B_StopFull = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.B_Record = new System.Windows.Forms.Label();
            this.ilRecord = new System.Windows.Forms.ImageList(this.components);
            this.iconBusy = new System.Windows.Forms.PictureBox();
            this.btnStartStopPTZ = new System.Windows.Forms.Label();
            this.ilStartStopPTZ = new System.Windows.Forms.ImageList(this.components);
            this.btnStartStopMicrowaveControl = new System.Windows.Forms.Label();
            this.ilMicrowaveControl = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.tmrStreamTimeLimit = new System.Windows.Forms.Timer(this.components);
            this.P_FullControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilPlay
            // 
            this.ilPlay.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilPlay.ImageStream")));
            this.ilPlay.TransparentColor = System.Drawing.Color.Transparent;
            this.ilPlay.Images.SetKeyName(0, "play-up.gif");
            this.ilPlay.Images.SetKeyName(1, "play-down.gif");
            // 
            // ilStop
            // 
            this.ilStop.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStop.ImageStream")));
            this.ilStop.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStop.Images.SetKeyName(0, "stop-up.gif");
            this.ilStop.Images.SetKeyName(1, "stop-down.gif");
            // 
            // P_Video
            // 
            this.P_Video.BackColor = System.Drawing.Color.Black;
            this.P_Video.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.P_Video.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Video.Location = new System.Drawing.Point(1, 1);
            this.P_Video.Name = "P_Video";
            this.P_Video.Size = new System.Drawing.Size(354, 242);
            this.P_Video.TabIndex = 2;
            this.P_Video.Click += new System.EventHandler(this.P_Video_Click);
            // 
            // ilSingleView
            // 
            this.ilSingleView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSingleView.ImageStream")));
            this.ilSingleView.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSingleView.Images.SetKeyName(0, "4waytoggle-up.gif");
            this.ilSingleView.Images.SetKeyName(1, "4waytoggle-down.gif");
            // 
            // P_FullControls
            // 
            this.P_FullControls.BackColor = System.Drawing.Color.Black;
            this.P_FullControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.P_FullControls.Controls.Add(this.L_StreamDescription);
            this.P_FullControls.Controls.Add(this.B_SingleView);
            this.P_FullControls.Controls.Add(this.B_FullScreen);
            this.P_FullControls.Controls.Add(this.B_Snapshot);
            this.P_FullControls.Controls.Add(this.B_StopFull);
            this.P_FullControls.Controls.Add(this.B_Record);
            this.P_FullControls.Controls.Add(this.iconBusy);
            this.P_FullControls.Controls.Add(this.btnStartStopPTZ);
            this.P_FullControls.Controls.Add(this.btnStartStopMicrowaveControl);
            this.P_FullControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.P_FullControls.Location = new System.Drawing.Point(1, 243);
            this.P_FullControls.Name = "P_FullControls";
            this.P_FullControls.Size = new System.Drawing.Size(354, 35);
            this.P_FullControls.TabIndex = 3;
            // 
            // L_StreamDescription
            // 
            this.L_StreamDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.L_StreamDescription.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_StreamDescription.ForeColor = System.Drawing.Color.White;
            this.L_StreamDescription.Location = new System.Drawing.Point(160, 0);
            this.L_StreamDescription.Name = "L_StreamDescription";
            this.L_StreamDescription.Size = new System.Drawing.Size(96, 33);
            this.L_StreamDescription.TabIndex = 14;
            this.L_StreamDescription.Text = "---";
            this.L_StreamDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // B_SingleView
            // 
            this.B_SingleView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_SingleView.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_SingleView.ImageIndex = 0;
            this.B_SingleView.ImageList = this.ilSingleView;
            this.B_SingleView.Location = new System.Drawing.Point(128, 0);
            this.B_SingleView.Name = "B_SingleView";
            this.B_SingleView.Size = new System.Drawing.Size(32, 33);
            this.B_SingleView.TabIndex = 0;
            this.tt.SetToolTip(this.B_SingleView, "Switch this stream to single view. Any other streams will be stopped.");
            this.B_SingleView.Click += new System.EventHandler(this.B_SingleView_Click);
            // 
            // B_FullScreen
            // 
            this.B_FullScreen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_FullScreen.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_FullScreen.Enabled = false;
            this.B_FullScreen.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_FullScreen.ImageIndex = 0;
            this.B_FullScreen.ImageList = this.ilFullScreen;
            this.B_FullScreen.Location = new System.Drawing.Point(96, 0);
            this.B_FullScreen.Name = "B_FullScreen";
            this.B_FullScreen.Size = new System.Drawing.Size(32, 33);
            this.B_FullScreen.TabIndex = 21;
            this.tt.SetToolTip(this.B_FullScreen, "Expand this stream to full-screen.");
            this.B_FullScreen.Click += new System.EventHandler(this.B_FullScreen_Click);
            // 
            // ilFullScreen
            // 
            this.ilFullScreen.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilFullScreen.ImageStream")));
            this.ilFullScreen.TransparentColor = System.Drawing.Color.Transparent;
            this.ilFullScreen.Images.SetKeyName(0, "fullscreen2-up.gif");
            this.ilFullScreen.Images.SetKeyName(1, "fullscreen2-down.gif");
            // 
            // B_Snapshot
            // 
            this.B_Snapshot.BackColor = System.Drawing.Color.Black;
            this.B_Snapshot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Snapshot.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_Snapshot.Enabled = false;
            this.B_Snapshot.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_Snapshot.ForeColor = System.Drawing.Color.White;
            this.B_Snapshot.ImageIndex = 0;
            this.B_Snapshot.ImageList = this.ilSnapshot;
            this.B_Snapshot.Location = new System.Drawing.Point(64, 0);
            this.B_Snapshot.Name = "B_Snapshot";
            this.B_Snapshot.Size = new System.Drawing.Size(32, 33);
            this.B_Snapshot.TabIndex = 17;
            this.B_Snapshot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.B_Snapshot.Toggle = false;
            this.tt.SetToolTip(this.B_Snapshot, "Take a snapshot of what you see in the stream");
            this.B_Snapshot.Click += new System.EventHandler(this.B_Snapshot_Click);
            // 
            // ilSnapshot
            // 
            this.ilSnapshot.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSnapshot.ImageStream")));
            this.ilSnapshot.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSnapshot.Images.SetKeyName(0, "snapshot-up.gif");
            this.ilSnapshot.Images.SetKeyName(1, "snapshot-down.gif");
            // 
            // B_StopFull
            // 
            this.B_StopFull.BackColor = System.Drawing.Color.Black;
            this.B_StopFull.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_StopFull.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_StopFull.Enabled = false;
            this.B_StopFull.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_StopFull.ForeColor = System.Drawing.Color.White;
            this.B_StopFull.ImageIndex = 0;
            this.B_StopFull.ImageList = this.ilStop;
            this.B_StopFull.Location = new System.Drawing.Point(32, 0);
            this.B_StopFull.Name = "B_StopFull";
            this.B_StopFull.Size = new System.Drawing.Size(32, 33);
            this.B_StopFull.TabIndex = 13;
            this.B_StopFull.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.B_StopFull.Toggle = false;
            this.tt.SetToolTip(this.B_StopFull, "Stop stream");
            this.B_StopFull.Click += new System.EventHandler(this.B_StopFull_Click);
            // 
            // B_Record
            // 
            this.B_Record.BackColor = System.Drawing.Color.Black;
            this.B_Record.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Record.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_Record.Enabled = false;
            this.B_Record.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_Record.ForeColor = System.Drawing.Color.White;
            this.B_Record.ImageIndex = 0;
            this.B_Record.ImageList = this.ilRecord;
            this.B_Record.Location = new System.Drawing.Point(0, 0);
            this.B_Record.Name = "B_Record";
            this.B_Record.Size = new System.Drawing.Size(32, 33);
            this.B_Record.TabIndex = 18;
            this.B_Record.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.B_Record, "Record the current stream to a local file");
            this.B_Record.Click += new System.EventHandler(this.B_Record_Click);
            // 
            // ilRecord
            // 
            this.ilRecord.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRecord.ImageStream")));
            this.ilRecord.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRecord.Images.SetKeyName(0, "recordsmall-up.gif");
            this.ilRecord.Images.SetKeyName(1, "recordsmall-down.gif");
            // 
            // iconBusy
            // 
            this.iconBusy.Dock = System.Windows.Forms.DockStyle.Right;
            this.iconBusy.Image = global::FutureConcepts.Media.Client.StreamViewer.Properties.Resources.loading;
            this.iconBusy.Location = new System.Drawing.Point(256, 0);
            this.iconBusy.Name = "iconBusy";
            this.iconBusy.Size = new System.Drawing.Size(32, 33);
            this.iconBusy.TabIndex = 24;
            this.iconBusy.Visible = false;
            // 
            // btnStartStopPTZ
            // 
            this.btnStartStopPTZ.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartStopPTZ.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStartStopPTZ.Enabled = false;
            this.btnStartStopPTZ.ImageIndex = 0;
            this.btnStartStopPTZ.ImageList = this.ilStartStopPTZ;
            this.btnStartStopPTZ.Location = new System.Drawing.Point(288, 0);
            this.btnStartStopPTZ.Name = "btnStartStopPTZ";
            this.btnStartStopPTZ.Size = new System.Drawing.Size(32, 33);
            this.btnStartStopPTZ.TabIndex = 22;
            this.tt.SetToolTip(this.btnStartStopPTZ, "Control this Pan/Tilt/Zoom camera.");
            this.btnStartStopPTZ.Visible = false;
            this.btnStartStopPTZ.Click += new System.EventHandler(this.btnStartStopPTZ_Click);
            // 
            // ilStartStopPTZ
            // 
            this.ilStartStopPTZ.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStartStopPTZ.ImageStream")));
            this.ilStartStopPTZ.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStartStopPTZ.Images.SetKeyName(0, "ptzctrl-up.gif");
            this.ilStartStopPTZ.Images.SetKeyName(1, "ptzctrl-down.gif");
            // 
            // btnStartStopMicrowaveControl
            // 
            this.btnStartStopMicrowaveControl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartStopMicrowaveControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStartStopMicrowaveControl.Enabled = false;
            this.btnStartStopMicrowaveControl.ImageIndex = 0;
            this.btnStartStopMicrowaveControl.ImageList = this.ilMicrowaveControl;
            this.btnStartStopMicrowaveControl.Location = new System.Drawing.Point(320, 0);
            this.btnStartStopMicrowaveControl.Name = "btnStartStopMicrowaveControl";
            this.btnStartStopMicrowaveControl.Size = new System.Drawing.Size(32, 33);
            this.btnStartStopMicrowaveControl.TabIndex = 23;
            this.tt.SetToolTip(this.btnStartStopMicrowaveControl, "Control the attached Microwave Receiver.");
            this.btnStartStopMicrowaveControl.Visible = false;
            this.btnStartStopMicrowaveControl.Click += new System.EventHandler(this.btnStartStopMicrowaveControl_Click);
            // 
            // ilMicrowaveControl
            // 
            this.ilMicrowaveControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMicrowaveControl.ImageStream")));
            this.ilMicrowaveControl.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMicrowaveControl.Images.SetKeyName(0, "microwavedl-up.gif");
            this.ilMicrowaveControl.Images.SetKeyName(1, "microwavedl-down.gif");
            // 
            // tt
            // 
            this.tt.AutoPopDelay = 5000;
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.InitialDelay = 50;
            this.tt.IsBalloon = true;
            this.tt.ReshowDelay = 10;
            // 
            // tmrStreamTimeLimit
            // 
            this.tmrStreamTimeLimit.Tick += new System.EventHandler(this.tmrStreamTimeLimit_Tick);
            // 
            // StreamViewerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.Controls.Add(this.P_Video);
            this.Controls.Add(this.P_FullControls);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "StreamViewerControl";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(356, 279);
            this.Load += new System.EventHandler(this.StreamViewerControl_Load);
            this.LocationChanged += new System.EventHandler(this.GraphControlClient_Faulted);
            this.RegionChanged += new System.EventHandler(this.GraphControlClient_Faulted);
            this.Move += new System.EventHandler(this.GraphControlClient_Faulted);
            this.P_FullControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ilPlay;
        private System.Windows.Forms.ImageList ilStop;
        private System.Windows.Forms.Panel P_Video;
        private System.Windows.Forms.Panel P_FullControls;
        private FutureConcepts.Media.CommonControls.FCClickButton B_StopFull;
        private System.Windows.Forms.Label L_StreamDescription;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Snapshot;
        private System.Windows.Forms.ImageList ilSnapshot;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Label B_Record;
        private System.Windows.Forms.ImageList ilRecord;
        private System.Windows.Forms.ImageList ilSingleView;
        private System.Windows.Forms.Label B_SingleView;
        private System.Windows.Forms.Timer tmrStreamTimeLimit;
        private System.Windows.Forms.ImageList ilFullScreen;
        private System.Windows.Forms.Label B_FullScreen;
        private System.Windows.Forms.Label btnStartStopPTZ;
        private System.Windows.Forms.ImageList ilStartStopPTZ;
        private System.Windows.Forms.ImageList ilMicrowaveControl;
        private System.Windows.Forms.Label btnStartStopMicrowaveControl;
        private System.Windows.Forms.PictureBox iconBusy;

    }
}
