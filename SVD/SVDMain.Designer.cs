using FutureConcepts.Media.SVD.Controls;
using FutureConcepts.Media.Client.StreamViewer;

namespace FutureConcepts.Media.SVD
{
    partial class SVDMain
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
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch (System.Exception ex)
            {
                FutureConcepts.Tools.ErrorLogger.DumpToDebug(ex);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelPalettes = new System.Windows.Forms.Panel();
            this.panel4Way = new System.Windows.Forms.TableLayoutPanel();
            this.panel_streamA = new System.Windows.Forms.Panel();
            this.StreamA = new FutureConcepts.Media.Client.StreamViewer.StreamViewerControl();
            this.panel_streamC = new System.Windows.Forms.Panel();
            this.StreamC = new FutureConcepts.Media.Client.StreamViewer.StreamViewerControl();
            this.panel_streamB = new System.Windows.Forms.Panel();
            this.StreamB = new FutureConcepts.Media.Client.StreamViewer.StreamViewerControl();
            this.panel_streamD = new System.Windows.Forms.Panel();
            this.StreamD = new FutureConcepts.Media.Client.StreamViewer.StreamViewerControl();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.favoriteStreams = new FutureConcepts.Media.SVD.Controls.FavoriteStreams();
            this.telemetryViewer = new FutureConcepts.Media.SVD.Controls.TelemetryViewer();
            this.panelRight = new System.Windows.Forms.Panel();
            this.profileGroupSelector = new FutureConcepts.Media.SVD.Controls.ProfileGroupSelector();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel4Way.SuspendLayout();
            this.panel_streamA.SuspendLayout();
            this.panel_streamC.SuspendLayout();
            this.panel_streamB.SuspendLayout();
            this.panel_streamD.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.DimGray;
            this.panel6.Controls.Add(this.panel4Way);
            this.panel6.Controls.Add(this.panelRight);
            this.panel6.Controls.Add(this.panelLeft);
            this.panel6.Controls.Add(this.panelPalettes);
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
            // panelPalettes
            // 
            this.panelPalettes.BackColor = System.Drawing.Color.Black;
            this.panelPalettes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPalettes.Location = new System.Drawing.Point(0, 556);
            this.panelPalettes.MinimumSize = new System.Drawing.Size(1, 166);
            this.panelPalettes.Name = "panelPalettes";
            this.panelPalettes.Size = new System.Drawing.Size(1020, 166);
            this.panelPalettes.TabIndex = 5;
            this.panelPalettes.Visible = false;
            // 
            // panel4Way
            // 
            this.panel4Way.BackColor = System.Drawing.Color.Black;
            this.panel4Way.ColumnCount = 2;
            this.panel4Way.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel4Way.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel4Way.Controls.Add(this.panel_streamA, 0, 0);
            this.panel4Way.Controls.Add(this.panel_streamC, 0, 1);
            this.panel4Way.Controls.Add(this.panel_streamB, 1, 0);
            this.panel4Way.Controls.Add(this.panel_streamD, 1, 1);
            this.panel4Way.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4Way.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.panel4Way.Location = new System.Drawing.Point(193, 0);
            this.panel4Way.Margin = new System.Windows.Forms.Padding(0);
            this.panel4Way.Name = "panel4Way";
            this.panel4Way.RowCount = 2;
            this.panel4Way.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel4Way.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panel4Way.Size = new System.Drawing.Size(721, 556);
            this.panel4Way.TabIndex = 6;
            // 
            // panel_streamA
            // 
            this.panel_streamA.Controls.Add(this.StreamA);
            this.panel_streamA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_streamA.Location = new System.Drawing.Point(3, 3);
            this.panel_streamA.Name = "panel_streamA";
            this.panel_streamA.Size = new System.Drawing.Size(354, 272);
            this.panel_streamA.TabIndex = 12;
            // 
            // StreamA
            // 
            this.StreamA.Active = false;
            this.StreamA.BackColor = System.Drawing.Color.DimGray;
            this.StreamA.CurrentProfile = null;
            this.StreamA.CustomProfile = null;
            this.StreamA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StreamA.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StreamA.ForeColor = System.Drawing.Color.White;
            this.StreamA.FullScreen = false;
            this.StreamA.SingleView = false;
            this.StreamA.Location = new System.Drawing.Point(0, 0);
            this.StreamA.Margin = new System.Windows.Forms.Padding(2);
            this.StreamA.Name = "StreamA";
            this.StreamA.Padding = new System.Windows.Forms.Padding(1);
            this.StreamA.PTZOverlayControl = null;
            this.StreamA.RecordingsPath = "c:\\temp\\recordings";
            this.StreamA.Size = new System.Drawing.Size(354, 272);
            this.StreamA.SnapshotPath = "c:\\temp";
            this.StreamA.SnapshotSoundFileName = "c:\\windows\\media\\ding.wav";
            this.StreamA.TabIndex = 0;
            // 
            // panel_streamC
            // 
            this.panel_streamC.Controls.Add(this.StreamC);
            this.panel_streamC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_streamC.Location = new System.Drawing.Point(3, 281);
            this.panel_streamC.Name = "panel_streamC";
            this.panel_streamC.Size = new System.Drawing.Size(354, 272);
            this.panel_streamC.TabIndex = 7;
            // 
            // StreamC
            // 
            this.StreamC.Active = false;
            this.StreamC.BackColor = System.Drawing.Color.DimGray;
            this.StreamC.CurrentProfile = null;
            this.StreamC.CustomProfile = null;
            this.StreamC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StreamC.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StreamC.ForeColor = System.Drawing.Color.White;
            this.StreamC.FullScreen = false;
            this.StreamC.SingleView = false;
            this.StreamC.Location = new System.Drawing.Point(0, 0);
            this.StreamC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StreamC.Name = "StreamC";
            this.StreamC.Padding = new System.Windows.Forms.Padding(1);
            this.StreamC.PTZOverlayControl = null;
            this.StreamC.RecordingsPath = "c:\\temp\\recordings";
            this.StreamC.Size = new System.Drawing.Size(354, 272);
            this.StreamC.SnapshotPath = "c:\\temp";
            this.StreamC.SnapshotSoundFileName = "c:\\windows\\media\\ding.wav";
            this.StreamC.TabIndex = 2;
            // 
            // panel_streamB
            // 
            this.panel_streamB.Controls.Add(this.StreamB);
            this.panel_streamB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_streamB.Location = new System.Drawing.Point(363, 3);
            this.panel_streamB.Name = "panel_streamB";
            this.panel_streamB.Size = new System.Drawing.Size(355, 272);
            this.panel_streamB.TabIndex = 6;
            // 
            // StreamB
            // 
            this.StreamB.Active = false;
            this.StreamB.BackColor = System.Drawing.Color.DimGray;
            this.StreamB.CurrentProfile = null;
            this.StreamB.CustomProfile = null;
            this.StreamB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StreamB.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StreamB.ForeColor = System.Drawing.Color.White;
            this.StreamB.FullScreen = false;
            this.StreamB.SingleView = false;
            this.StreamB.Location = new System.Drawing.Point(0, 0);
            this.StreamB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StreamB.Name = "StreamB";
            this.StreamB.Padding = new System.Windows.Forms.Padding(1);
            this.StreamB.PTZOverlayControl = null;
            this.StreamB.RecordingsPath = "c:\\temp\\recordings";
            this.StreamB.Size = new System.Drawing.Size(355, 272);
            this.StreamB.SnapshotPath = "c:\\temp";
            this.StreamB.SnapshotSoundFileName = "c:\\windows\\media\\ding.wav";
            this.StreamB.TabIndex = 1;
            // 
            // panel_streamD
            // 
            this.panel_streamD.Controls.Add(this.StreamD);
            this.panel_streamD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_streamD.Location = new System.Drawing.Point(363, 281);
            this.panel_streamD.Name = "panel_streamD";
            this.panel_streamD.Size = new System.Drawing.Size(355, 272);
            this.panel_streamD.TabIndex = 5;
            // 
            // StreamD
            // 
            this.StreamD.Active = false;
            this.StreamD.BackColor = System.Drawing.Color.DimGray;
            this.StreamD.CurrentProfile = null;
            this.StreamD.CustomProfile = null;
            this.StreamD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StreamD.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StreamD.ForeColor = System.Drawing.Color.White;
            this.StreamD.FullScreen = false;
            this.StreamD.SingleView = false;
            this.StreamD.Location = new System.Drawing.Point(0, 0);
            this.StreamD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StreamD.Name = "StreamD";
            this.StreamD.Padding = new System.Windows.Forms.Padding(1);
            this.StreamD.PTZOverlayControl = null;
            this.StreamD.RecordingsPath = "c:\\temp\\recordings";
            this.StreamD.Size = new System.Drawing.Size(355, 272);
            this.StreamD.SnapshotPath = "c:\\temp";
            this.StreamD.SnapshotSoundFileName = "c:\\windows\\media\\ding.wav";
            this.StreamD.TabIndex = 3;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Black;
            this.panelLeft.Controls.Add(this.favoriteStreams);
            this.panelLeft.Controls.Add(this.telemetryViewer);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(3);
            this.panelLeft.Size = new System.Drawing.Size(193, 556);
            this.panelLeft.TabIndex = 11;
            // 
            // favoriteStreams
            // 
            this.favoriteStreams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.favoriteStreams.BackColor = System.Drawing.Color.DimGray;
            this.favoriteStreams.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.favoriteStreams.ForeColor = System.Drawing.Color.White;
            this.favoriteStreams.Location = new System.Drawing.Point(3, 3);
            this.favoriteStreams.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.favoriteStreams.Name = "favoriteStreams";
            this.favoriteStreams.Padding = new System.Windows.Forms.Padding(1);
            this.favoriteStreams.Size = new System.Drawing.Size(187, 451);
            this.favoriteStreams.TabIndex = 0;
            // 
            // telemetryViewer
            // 
            this.telemetryViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.telemetryViewer.BackColor = System.Drawing.Color.DimGray;
            this.telemetryViewer.BitRate = 0;
            this.telemetryViewer.Location = new System.Drawing.Point(3, 457);
            this.telemetryViewer.Name = "telemetryViewer";
            this.telemetryViewer.Padding = new System.Windows.Forms.Padding(1);
            this.telemetryViewer.Size = new System.Drawing.Size(187, 96);
            this.telemetryViewer.State = FutureConcepts.Media.Client.StreamViewer.StreamState.Available;
            this.telemetryViewer.TabIndex = 11;
            this.telemetryViewer.Users = null;
            this.telemetryViewer.Viewer = null;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.Black;
            this.panelRight.Controls.Add(this.profileGroupSelector);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(914, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(3);
            this.panelRight.Size = new System.Drawing.Size(106, 556);
            this.panelRight.TabIndex = 13;
            // 
            // profileGroupSelector
            // 
            this.profileGroupSelector.BackColor = System.Drawing.Color.Black;
            this.profileGroupSelector.ChangingSelf = false;
            this.profileGroupSelector.CustomProfile = null;
            this.profileGroupSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.profileGroupSelector.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileGroupSelector.ForeColor = System.Drawing.Color.White;
            this.profileGroupSelector.Location = new System.Drawing.Point(3, 3);
            this.profileGroupSelector.Name = "profileGroupSelector";
            this.profileGroupSelector.SelectedProfile = null;
            this.profileGroupSelector.SelectedProfileGroup = null;
            this.profileGroupSelector.Size = new System.Drawing.Size(100, 550);
            this.profileGroupSelector.StreamViewer = null;
            this.profileGroupSelector.TabIndex = 12;
            this.profileGroupSelector.Visible = false;
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.DimGray;
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // SVDMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.HasCloseButton = true;
            this.HasControlBox = true;
            this.HasVersion = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "SVDMain";
            this.Text = "AntaresX Streaming Video Desktop";
            this.Load += new System.EventHandler(this.SVDMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SVDMain_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SVDMain_KeyDown);
            this.panel6.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4Way.ResumeLayout(false);
            this.panel_streamA.ResumeLayout(false);
            this.panel_streamC.ResumeLayout(false);
            this.panel_streamB.ResumeLayout(false);
            this.panel_streamD.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FavoriteStreams favoriteStreams;
        private System.Windows.Forms.Panel panelPalettes;
        private System.Windows.Forms.TableLayoutPanel panel4Way;
        private Client.StreamViewer.StreamViewerControl StreamD;
        private Client.StreamViewer.StreamViewerControl StreamC;
        private Client.StreamViewer.StreamViewerControl StreamB;
        private Client.StreamViewer.StreamViewerControl StreamA;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Panel panel_streamD;
        private TelemetryViewer telemetryViewer;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panel_streamC;
        private System.Windows.Forms.Panel panel_streamB;
        private ProfileGroupSelector profileGroupSelector;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panel_streamA;
    }
}

