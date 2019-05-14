namespace FutureConcepts.Media.TV.Scanner
{
    partial class SourceInputSelector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceInputSelector));
            this.tbPanelSources = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCToolbarButtonPanel();
            this.tbPanelInputs = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCToolbarButtonPanel();
            this.sourceInputBtn_Composite = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton();
            this.ilSatellite = new System.Windows.Forms.ImageList(this.components);
            this.sourceInputBtn_Antenna = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton();
            this.ilBroadcast = new System.Windows.Forms.ImageList(this.components);
            this.sourceBtn_Network = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton();
            this.ilNetwork = new System.Windows.Forms.ImageList(this.components);
            this.sourceBtn_LocalAnalog = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton();
            this.ilLocalAnalog = new System.Windows.Forms.ImageList(this.components);
            this.sourceBtn_LocalDigital = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton();
            this.ilLocalDigital = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.tbPanelSources.SuspendLayout();
            this.tbPanelInputs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPanelSources
            // 
            this.tbPanelSources.Controls.Add(this.tbPanelInputs);
            this.tbPanelSources.Controls.Add(this.sourceBtn_Network);
            this.tbPanelSources.Controls.Add(this.sourceBtn_LocalAnalog);
            this.tbPanelSources.Controls.Add(this.sourceBtn_LocalDigital);
            this.tbPanelSources.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbPanelSources.Location = new System.Drawing.Point(0, 0);
            this.tbPanelSources.Name = "tbPanelSources";
            this.tbPanelSources.Size = new System.Drawing.Size(246, 67);
            this.tbPanelSources.TabIndex = 58;
            // 
            // tbPanelInputs
            // 
            this.tbPanelInputs.Controls.Add(this.sourceInputBtn_Composite);
            this.tbPanelInputs.Controls.Add(this.sourceInputBtn_Antenna);
            this.tbPanelInputs.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbPanelInputs.Location = new System.Drawing.Point(183, 0);
            this.tbPanelInputs.Margin = new System.Windows.Forms.Padding(0);
            this.tbPanelInputs.Name = "tbPanelInputs";
            this.tbPanelInputs.Size = new System.Drawing.Size(62, 67);
            this.tbPanelInputs.TabIndex = 54;
            // 
            // sourceInputBtn_Composite
            // 
            this.sourceInputBtn_Composite.BackColor = System.Drawing.Color.Black;
            this.sourceInputBtn_Composite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceInputBtn_Composite.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceInputBtn_Composite.ForeColor = System.Drawing.Color.White;
            this.sourceInputBtn_Composite.ImageIndex = 0;
            this.sourceInputBtn_Composite.ImageList = this.ilSatellite;
            this.sourceInputBtn_Composite.Location = new System.Drawing.Point(0, 35);
            this.sourceInputBtn_Composite.Margin = new System.Windows.Forms.Padding(3);
            this.sourceInputBtn_Composite.MinimumSize = new System.Drawing.Size(60, 30);
            this.sourceInputBtn_Composite.Name = "sourceInputBtn_Composite";
            this.sourceInputBtn_Composite.Size = new System.Drawing.Size(62, 31);
            this.sourceInputBtn_Composite.TabIndex = 52;
            this.sourceInputBtn_Composite.Tag = "Record";
            this.sourceInputBtn_Composite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.sourceInputBtn_Composite, "Watch television from a satellite receiver.");
            this.sourceInputBtn_Composite.Click += new System.EventHandler(this.sourceInputBtn_Composite_Click);
            // 
            // ilSatellite
            // 
            this.ilSatellite.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSatellite.ImageStream")));
            this.ilSatellite.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSatellite.Images.SetKeyName(0, "satellite-up.gif");
            this.ilSatellite.Images.SetKeyName(1, "satellite-down.gif");
            this.ilSatellite.Images.SetKeyName(2, "satellite-down-disabled.gif");
            // 
            // sourceInputBtn_Antenna
            // 
            this.sourceInputBtn_Antenna.BackColor = System.Drawing.Color.Black;
            this.sourceInputBtn_Antenna.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceInputBtn_Antenna.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceInputBtn_Antenna.ForeColor = System.Drawing.Color.White;
            this.sourceInputBtn_Antenna.ImageIndex = 0;
            this.sourceInputBtn_Antenna.ImageList = this.ilBroadcast;
            this.sourceInputBtn_Antenna.Location = new System.Drawing.Point(0, 1);
            this.sourceInputBtn_Antenna.Margin = new System.Windows.Forms.Padding(3);
            this.sourceInputBtn_Antenna.MinimumSize = new System.Drawing.Size(60, 33);
            this.sourceInputBtn_Antenna.Name = "sourceInputBtn_Antenna";
            this.sourceInputBtn_Antenna.Size = new System.Drawing.Size(62, 33);
            this.sourceInputBtn_Antenna.TabIndex = 53;
            this.sourceInputBtn_Antenna.Tag = "Record";
            this.sourceInputBtn_Antenna.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.sourceInputBtn_Antenna, "Watch broadcast (over-the-air) television.");
            this.sourceInputBtn_Antenna.Click += new System.EventHandler(this.sourceInputBtn_Antenna_Click);
            // 
            // ilBroadcast
            // 
            this.ilBroadcast.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilBroadcast.ImageStream")));
            this.ilBroadcast.TransparentColor = System.Drawing.Color.Transparent;
            this.ilBroadcast.Images.SetKeyName(0, "broadcast-up.gif");
            this.ilBroadcast.Images.SetKeyName(1, "broadcast-down.gif");
            this.ilBroadcast.Images.SetKeyName(2, "broadcast-down-disabled.gif");
            // 
            // sourceBtn_Network
            // 
            this.sourceBtn_Network.BackColor = System.Drawing.Color.Black;
            this.sourceBtn_Network.Dock = System.Windows.Forms.DockStyle.Left;
            this.sourceBtn_Network.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceBtn_Network.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceBtn_Network.ForeColor = System.Drawing.Color.White;
            this.sourceBtn_Network.ImageIndex = 0;
            this.sourceBtn_Network.ImageList = this.ilNetwork;
            this.sourceBtn_Network.Location = new System.Drawing.Point(122, 0);
            this.sourceBtn_Network.Margin = new System.Windows.Forms.Padding(0);
            this.sourceBtn_Network.MinimumSize = new System.Drawing.Size(60, 50);
            this.sourceBtn_Network.Name = "sourceBtn_Network";
            this.sourceBtn_Network.Size = new System.Drawing.Size(61, 67);
            this.sourceBtn_Network.TabIndex = 54;
            this.sourceBtn_Network.Tag = "Record";
            this.sourceBtn_Network.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.sourceBtn_Network, "Switch to Network television.");
            this.sourceBtn_Network.DoubleClick += new System.EventHandler(this.sourceBtn_DoubleClick);
            this.sourceBtn_Network.Click += new System.EventHandler(this.sourceBtn_Network_Click);
            // 
            // ilNetwork
            // 
            this.ilNetwork.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNetwork.ImageStream")));
            this.ilNetwork.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNetwork.Images.SetKeyName(0, "network-up.gif");
            this.ilNetwork.Images.SetKeyName(1, "network-down.gif");
            this.ilNetwork.Images.SetKeyName(2, "network-down-disabled.gif");
            // 
            // sourceBtn_LocalAnalog
            // 
            this.sourceBtn_LocalAnalog.BackColor = System.Drawing.Color.Black;
            this.sourceBtn_LocalAnalog.Dock = System.Windows.Forms.DockStyle.Left;
            this.sourceBtn_LocalAnalog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceBtn_LocalAnalog.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceBtn_LocalAnalog.ForeColor = System.Drawing.Color.White;
            this.sourceBtn_LocalAnalog.ImageIndex = 0;
            this.sourceBtn_LocalAnalog.ImageList = this.ilLocalAnalog;
            this.sourceBtn_LocalAnalog.Location = new System.Drawing.Point(61, 0);
            this.sourceBtn_LocalAnalog.Margin = new System.Windows.Forms.Padding(0);
            this.sourceBtn_LocalAnalog.MinimumSize = new System.Drawing.Size(60, 50);
            this.sourceBtn_LocalAnalog.Name = "sourceBtn_LocalAnalog";
            this.sourceBtn_LocalAnalog.Size = new System.Drawing.Size(61, 67);
            this.sourceBtn_LocalAnalog.TabIndex = 55;
            this.sourceBtn_LocalAnalog.Tag = "";
            this.sourceBtn_LocalAnalog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.sourceBtn_LocalAnalog, "Switch to the Local Tuner, and watch Analog television.");
            this.sourceBtn_LocalAnalog.Click += new System.EventHandler(this.sourceBtn_LocalAnalog_Click);
            // 
            // ilLocalAnalog
            // 
            this.ilLocalAnalog.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilLocalAnalog.ImageStream")));
            this.ilLocalAnalog.TransparentColor = System.Drawing.Color.Transparent;
            this.ilLocalAnalog.Images.SetKeyName(0, "analoglocal-up.png");
            this.ilLocalAnalog.Images.SetKeyName(1, "analoglocal-down.png");
            this.ilLocalAnalog.Images.SetKeyName(2, "analoglocal-down-disabled.png");
            // 
            // sourceBtn_LocalDigital
            // 
            this.sourceBtn_LocalDigital.BackColor = System.Drawing.Color.Black;
            this.sourceBtn_LocalDigital.Dock = System.Windows.Forms.DockStyle.Left;
            this.sourceBtn_LocalDigital.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sourceBtn_LocalDigital.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceBtn_LocalDigital.ForeColor = System.Drawing.Color.White;
            this.sourceBtn_LocalDigital.ImageIndex = 0;
            this.sourceBtn_LocalDigital.ImageList = this.ilLocalDigital;
            this.sourceBtn_LocalDigital.Location = new System.Drawing.Point(0, 0);
            this.sourceBtn_LocalDigital.Margin = new System.Windows.Forms.Padding(0);
            this.sourceBtn_LocalDigital.MinimumSize = new System.Drawing.Size(60, 50);
            this.sourceBtn_LocalDigital.Name = "sourceBtn_LocalDigital";
            this.sourceBtn_LocalDigital.Size = new System.Drawing.Size(61, 67);
            this.sourceBtn_LocalDigital.TabIndex = 56;
            this.sourceBtn_LocalDigital.Tag = "";
            this.sourceBtn_LocalDigital.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tt.SetToolTip(this.sourceBtn_LocalDigital, "Switch to the Local Tuner, and watch Digital television.");
            this.sourceBtn_LocalDigital.Click += new System.EventHandler(this.sourceBtn_LocalDigital_Click);
            // 
            // ilLocalDigital
            // 
            this.ilLocalDigital.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilLocalDigital.ImageStream")));
            this.ilLocalDigital.TransparentColor = System.Drawing.Color.Transparent;
            this.ilLocalDigital.Images.SetKeyName(0, "digilocal-up.png");
            this.ilLocalDigital.Images.SetKeyName(1, "digilocal-down.png");
            this.ilLocalDigital.Images.SetKeyName(2, "digilocal-down-disabled.png");
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // SourceInputSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.tbPanelSources);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SourceInputSelector";
            this.Size = new System.Drawing.Size(246, 67);
            this.tbPanelSources.ResumeLayout(false);
            this.tbPanelInputs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCToolbarButtonPanel tbPanelSources;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton sourceBtn_LocalDigital;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCToolbarButtonPanel tbPanelInputs;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton sourceInputBtn_Composite;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton sourceInputBtn_Antenna;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton sourceBtn_Network;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCButton sourceBtn_LocalAnalog;
        private System.Windows.Forms.ImageList ilNetwork;
        private System.Windows.Forms.ImageList ilBroadcast;
        private System.Windows.Forms.ImageList ilLocalAnalog;
        private System.Windows.Forms.ImageList ilSatellite;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.ImageList ilLocalDigital;
    }
}
