namespace FutureConcepts.Media.TV.Scanner
{
    partial class AdvancedSettings
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
            System.Windows.Forms.Label lblTunerDevice;
            System.Windows.Forms.Label lblAutosnapInterval;
            System.Windows.Forms.Label lblSource;
            System.Windows.Forms.Label lblServerIP;
            System.Windows.Forms.Label lblMaxSnaps;
            System.Windows.Forms.Label lblInputType;
            System.Windows.Forms.Label lblSourceType;
            System.Windows.Forms.Label label3;
            this.cbTunerDevice = new System.Windows.Forms.ComboBox();
            this.gbGeneralSettings = new System.Windows.Forms.GroupBox();
            this.B_resetUserSettings = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.udMaximumSnaps = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAutoSnapInterval = new System.Windows.Forms.ComboBox();
            this.gbNetworkSettings = new System.Windows.Forms.GroupBox();
            this.CHK_ShowNetwork = new System.Windows.Forms.CheckBox();
            this.B_RefreshSources = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.cbNetworkSources = new System.Windows.Forms.ComboBox();
            this.TB_ServerIP = new System.Windows.Forms.TextBox();
            this.gbDisplaySettings = new System.Windows.Forms.GroupBox();
            this.cbInputMode = new System.Windows.Forms.ComboBox();
            this.cbSourceType = new System.Windows.Forms.ComboBox();
            this.cbTunerInputType = new System.Windows.Forms.ComboBox();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.btnVideoDecoderSettings = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            lblTunerDevice = new System.Windows.Forms.Label();
            lblAutosnapInterval = new System.Windows.Forms.Label();
            lblSource = new System.Windows.Forms.Label();
            lblServerIP = new System.Windows.Forms.Label();
            lblMaxSnaps = new System.Windows.Forms.Label();
            lblInputType = new System.Windows.Forms.Label();
            lblSourceType = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.gbGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaximumSnaps)).BeginInit();
            this.gbNetworkSettings.SuspendLayout();
            this.gbDisplaySettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.groupBox1);
            this.panel8.Controls.Add(this.gbDisplaySettings);
            this.panel8.Controls.Add(this.gbNetworkSettings);
            this.panel8.Controls.Add(this.gbGeneralSettings);
            this.panel8.Size = new System.Drawing.Size(425, 426);
            // 
            // panel6
            // 
            this.panel6.Size = new System.Drawing.Size(425, 464);
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(389, 0);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(425, 506);
            // 
            // lblTunerDevice
            // 
            lblTunerDevice.AutoSize = true;
            lblTunerDevice.Location = new System.Drawing.Point(25, 25);
            lblTunerDevice.Name = "lblTunerDevice";
            lblTunerDevice.Size = new System.Drawing.Size(88, 16);
            lblTunerDevice.TabIndex = 6;
            lblTunerDevice.Text = "Tuner Device:";
            lblTunerDevice.DoubleClick += new System.EventHandler(this.lblTunerDevice_DoubleClick);
            // 
            // lblAutosnapInterval
            // 
            lblAutosnapInterval.AutoSize = true;
            lblAutosnapInterval.Location = new System.Drawing.Point(6, 21);
            lblAutosnapInterval.Name = "lblAutosnapInterval";
            lblAutosnapInterval.Size = new System.Drawing.Size(113, 16);
            lblAutosnapInterval.TabIndex = 0;
            lblAutosnapInterval.Text = "Autosnap Interval:";
            // 
            // lblSource
            // 
            lblSource.AutoSize = true;
            lblSource.Location = new System.Drawing.Point(60, 80);
            lblSource.Name = "lblSource";
            lblSource.Size = new System.Drawing.Size(53, 16);
            lblSource.TabIndex = 3;
            lblSource.Text = "Source:";
            // 
            // lblServerIP
            // 
            lblServerIP.AutoSize = true;
            lblServerIP.Location = new System.Drawing.Point(47, 51);
            lblServerIP.Name = "lblServerIP";
            lblServerIP.Size = new System.Drawing.Size(66, 16);
            lblServerIP.TabIndex = 0;
            lblServerIP.Text = "Server IP:";
            // 
            // lblMaxSnaps
            // 
            lblMaxSnaps.AutoSize = true;
            lblMaxSnaps.Location = new System.Drawing.Point(233, 21);
            lblMaxSnaps.Name = "lblMaxSnaps";
            lblMaxSnaps.Size = new System.Drawing.Size(75, 16);
            lblMaxSnaps.TabIndex = 4;
            lblMaxSnaps.Text = "Max Snaps:";
            // 
            // lblInputType
            // 
            lblInputType.AutoSize = true;
            lblInputType.Location = new System.Drawing.Point(39, 55);
            lblInputType.Name = "lblInputType";
            lblInputType.Size = new System.Drawing.Size(74, 16);
            lblInputType.TabIndex = 2;
            lblInputType.Text = "Input Type:";
            // 
            // lblSourceType
            // 
            lblSourceType.AutoSize = true;
            lblSourceType.Location = new System.Drawing.Point(25, 85);
            lblSourceType.Name = "lblSourceType";
            lblSourceType.Size = new System.Drawing.Size(88, 16);
            lblSourceType.TabIndex = 9;
            lblSourceType.Text = "Source Mode:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(36, 115);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 16);
            label3.TabIndex = 11;
            label3.Text = "Input Mode:";
            // 
            // cbTunerDevice
            // 
            this.cbTunerDevice.BackColor = System.Drawing.Color.Black;
            this.cbTunerDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTunerDevice.DropDownWidth = 220;
            this.cbTunerDevice.ForeColor = System.Drawing.Color.White;
            this.cbTunerDevice.FormattingEnabled = true;
            this.cbTunerDevice.Location = new System.Drawing.Point(119, 22);
            this.cbTunerDevice.Name = "cbTunerDevice";
            this.cbTunerDevice.Size = new System.Drawing.Size(281, 24);
            this.cbTunerDevice.TabIndex = 7;
            this.tt.SetToolTip(this.cbTunerDevice, "Default Tuner Device that TV Scanner starts with.");
            this.cbTunerDevice.SelectedIndexChanged += new System.EventHandler(this.CB_TunerDevice_SelectedIndexChanged);
            // 
            // gbGeneralSettings
            // 
            this.gbGeneralSettings.Controls.Add(this.B_resetUserSettings);
            this.gbGeneralSettings.Controls.Add(this.udMaximumSnaps);
            this.gbGeneralSettings.Controls.Add(lblMaxSnaps);
            this.gbGeneralSettings.Controls.Add(this.label2);
            this.gbGeneralSettings.Controls.Add(this.cbAutoSnapInterval);
            this.gbGeneralSettings.Controls.Add(lblAutosnapInterval);
            this.gbGeneralSettings.ForeColor = System.Drawing.Color.White;
            this.gbGeneralSettings.Location = new System.Drawing.Point(8, 8);
            this.gbGeneralSettings.Name = "gbGeneralSettings";
            this.gbGeneralSettings.Size = new System.Drawing.Size(406, 79);
            this.gbGeneralSettings.TabIndex = 2;
            this.gbGeneralSettings.TabStop = false;
            this.gbGeneralSettings.Text = "General";
            // 
            // B_resetUserSettings
            // 
            this.B_resetUserSettings.BackColor = System.Drawing.Color.Red;
            this.B_resetUserSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_resetUserSettings.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_resetUserSettings.ForeColor = System.Drawing.Color.White;
            this.B_resetUserSettings.Location = new System.Drawing.Point(9, 48);
            this.B_resetUserSettings.Name = "B_resetUserSettings";
            this.B_resetUserSettings.Size = new System.Drawing.Size(392, 23);
            this.B_resetUserSettings.TabIndex = 0;
            this.B_resetUserSettings.Text = "Reset User Settings";
            this.B_resetUserSettings.UseVisualStyleBackColor = true;
            this.B_resetUserSettings.Click += new System.EventHandler(this.B_resetUserSettings_Click);
            // 
            // udMaximumSnaps
            // 
            this.udMaximumSnaps.BackColor = System.Drawing.Color.Black;
            this.udMaximumSnaps.ForeColor = System.Drawing.Color.White;
            this.udMaximumSnaps.Location = new System.Drawing.Point(314, 18);
            this.udMaximumSnaps.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.udMaximumSnaps.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.udMaximumSnaps.Name = "udMaximumSnaps";
            this.udMaximumSnaps.Size = new System.Drawing.Size(86, 23);
            this.udMaximumSnaps.TabIndex = 5;
            this.tt.SetToolTip(this.udMaximumSnaps, "Total number of auto-snaps per snapping session. Use -1 for unlimited.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "s";
            // 
            // cbAutoSnapInterval
            // 
            this.cbAutoSnapInterval.BackColor = System.Drawing.Color.Black;
            this.cbAutoSnapInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAutoSnapInterval.ForeColor = System.Drawing.Color.White;
            this.cbAutoSnapInterval.FormattingEnabled = true;
            this.cbAutoSnapInterval.Items.AddRange(new object[] {
            "3",
            "5",
            "10",
            "15",
            "30",
            "60"});
            this.cbAutoSnapInterval.Location = new System.Drawing.Point(119, 18);
            this.cbAutoSnapInterval.Name = "cbAutoSnapInterval";
            this.cbAutoSnapInterval.Size = new System.Drawing.Size(69, 24);
            this.cbAutoSnapInterval.TabIndex = 2;
            this.tt.SetToolTip(this.cbAutoSnapInterval, "The interval in seconds between auto-snaps.");
            // 
            // gbNetworkSettings
            // 
            this.gbNetworkSettings.Controls.Add(this.CHK_ShowNetwork);
            this.gbNetworkSettings.Controls.Add(this.B_RefreshSources);
            this.gbNetworkSettings.Controls.Add(lblSource);
            this.gbNetworkSettings.Controls.Add(this.cbNetworkSources);
            this.gbNetworkSettings.Controls.Add(this.TB_ServerIP);
            this.gbNetworkSettings.Controls.Add(lblServerIP);
            this.gbNetworkSettings.ForeColor = System.Drawing.Color.White;
            this.gbNetworkSettings.Location = new System.Drawing.Point(8, 300);
            this.gbNetworkSettings.Name = "gbNetworkSettings";
            this.gbNetworkSettings.Size = new System.Drawing.Size(406, 111);
            this.gbNetworkSettings.TabIndex = 3;
            this.gbNetworkSettings.TabStop = false;
            this.gbNetworkSettings.Text = "Network TV Settings";
            // 
            // CHK_ShowNetwork
            // 
            this.CHK_ShowNetwork.AutoSize = true;
            this.CHK_ShowNetwork.ForeColor = System.Drawing.Color.White;
            this.CHK_ShowNetwork.Location = new System.Drawing.Point(119, 22);
            this.CHK_ShowNetwork.Name = "CHK_ShowNetwork";
            this.CHK_ShowNetwork.Size = new System.Drawing.Size(164, 17);
            this.CHK_ShowNetwork.TabIndex = 5;
            this.CHK_ShowNetwork.Text = "Show Network source button";
            this.tt.SetToolTip(this.CHK_ShowNetwork, "Check this box to have the Network source button visible.");
            this.CHK_ShowNetwork.UseVisualStyleBackColor = true;
            this.CHK_ShowNetwork.CheckedChanged += new System.EventHandler(this.CHK_ShowNetwork_CheckedChanged);
            // 
            // B_RefreshSources
            // 
            this.B_RefreshSources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.B_RefreshSources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_RefreshSources.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.B_RefreshSources.ForeColor = System.Drawing.Color.White;
            this.B_RefreshSources.Location = new System.Drawing.Point(282, 48);
            this.B_RefreshSources.Name = "B_RefreshSources";
            this.B_RefreshSources.Size = new System.Drawing.Size(118, 23);
            this.B_RefreshSources.TabIndex = 4;
            this.B_RefreshSources.Text = "Refresh Sources";
            this.tt.SetToolTip(this.B_RefreshSources, "Click this button to get TV sources from the server.");
            this.B_RefreshSources.UseVisualStyleBackColor = false;
            this.B_RefreshSources.Click += new System.EventHandler(this.B_RefreshSources_Click);
            // 
            // cbNetworkSources
            // 
            this.cbNetworkSources.BackColor = System.Drawing.Color.Black;
            this.cbNetworkSources.ForeColor = System.Drawing.Color.White;
            this.cbNetworkSources.FormattingEnabled = true;
            this.cbNetworkSources.Location = new System.Drawing.Point(119, 77);
            this.cbNetworkSources.Name = "cbNetworkSources";
            this.cbNetworkSources.Size = new System.Drawing.Size(157, 24);
            this.cbNetworkSources.TabIndex = 2;
            this.tt.SetToolTip(this.cbNetworkSources, "Select or type in a source to connect to.");
            // 
            // TB_ServerIP
            // 
            this.TB_ServerIP.BackColor = System.Drawing.Color.Black;
            this.TB_ServerIP.ForeColor = System.Drawing.Color.White;
            this.TB_ServerIP.Location = new System.Drawing.Point(119, 48);
            this.TB_ServerIP.Name = "TB_ServerIP";
            this.TB_ServerIP.Size = new System.Drawing.Size(157, 23);
            this.TB_ServerIP.TabIndex = 1;
            this.tt.SetToolTip(this.TB_ServerIP, "Enter the Server\'s IP address or hostname.");
            this.TB_ServerIP.TextChanged += new System.EventHandler(this.TB_ServerIP_TextChanged);
            // 
            // gbDisplaySettings
            // 
            this.gbDisplaySettings.Controls.Add(label3);
            this.gbDisplaySettings.Controls.Add(this.cbInputMode);
            this.gbDisplaySettings.Controls.Add(lblSourceType);
            this.gbDisplaySettings.Controls.Add(this.cbSourceType);
            this.gbDisplaySettings.Controls.Add(lblTunerDevice);
            this.gbDisplaySettings.Controls.Add(this.cbTunerInputType);
            this.gbDisplaySettings.Controls.Add(this.cbTunerDevice);
            this.gbDisplaySettings.Controls.Add(lblInputType);
            this.gbDisplaySettings.ForeColor = System.Drawing.Color.White;
            this.gbDisplaySettings.Location = new System.Drawing.Point(9, 93);
            this.gbDisplaySettings.Name = "gbDisplaySettings";
            this.gbDisplaySettings.Size = new System.Drawing.Size(406, 147);
            this.gbDisplaySettings.TabIndex = 4;
            this.gbDisplaySettings.TabStop = false;
            this.gbDisplaySettings.Text = "Tuner Settings";
            // 
            // cbInputMode
            // 
            this.cbInputMode.BackColor = System.Drawing.Color.Black;
            this.cbInputMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputMode.ForeColor = System.Drawing.Color.White;
            this.cbInputMode.FormattingEnabled = true;
            this.cbInputMode.Location = new System.Drawing.Point(119, 112);
            this.cbInputMode.Name = "cbInputMode";
            this.cbInputMode.Size = new System.Drawing.Size(157, 24);
            this.cbInputMode.TabIndex = 10;
            this.tt.SetToolTip(this.cbInputMode, "The connected input TV signal.");
            // 
            // cbSourceType
            // 
            this.cbSourceType.BackColor = System.Drawing.Color.Black;
            this.cbSourceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSourceType.ForeColor = System.Drawing.Color.White;
            this.cbSourceType.FormattingEnabled = true;
            this.cbSourceType.Location = new System.Drawing.Point(119, 82);
            this.cbSourceType.Name = "cbSourceType";
            this.cbSourceType.Size = new System.Drawing.Size(157, 24);
            this.cbSourceType.TabIndex = 8;
            this.tt.SetToolTip(this.cbSourceType, "The connected input TV signal.");
            // 
            // cbTunerInputType
            // 
            this.cbTunerInputType.BackColor = System.Drawing.Color.Black;
            this.cbTunerInputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTunerInputType.ForeColor = System.Drawing.Color.White;
            this.cbTunerInputType.FormattingEnabled = true;
            this.cbTunerInputType.Location = new System.Drawing.Point(119, 52);
            this.cbTunerInputType.Name = "cbTunerInputType";
            this.cbTunerInputType.Size = new System.Drawing.Size(86, 24);
            this.cbTunerInputType.TabIndex = 3;
            this.tt.SetToolTip(this.cbTunerInputType, "The connected input TV signal.");
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // btnVideoDecoderSettings
            // 
            this.btnVideoDecoderSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnVideoDecoderSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVideoDecoderSettings.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVideoDecoderSettings.ForeColor = System.Drawing.Color.White;
            this.btnVideoDecoderSettings.Location = new System.Drawing.Point(119, 22);
            this.btnVideoDecoderSettings.Name = "btnVideoDecoderSettings";
            this.btnVideoDecoderSettings.Size = new System.Drawing.Size(157, 23);
            this.btnVideoDecoderSettings.TabIndex = 12;
            this.btnVideoDecoderSettings.Text = "Video Decoder Settings...";
            this.btnVideoDecoderSettings.UseVisualStyleBackColor = false;
            this.btnVideoDecoderSettings.Click += new System.EventHandler(this.btnVideoDecoderSettings_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnVideoDecoderSettings);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(8, 246);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 51);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "HDTV Settings";
            // 
            // AdvancedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 510);
            this.HasCloseButton = true;
            this.HasVersion = true;
            this.Name = "AdvancedSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AntaresX TV Scanner Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AdvancedSettings_Load);
            this.panel8.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.gbGeneralSettings.ResumeLayout(false);
            this.gbGeneralSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udMaximumSnaps)).EndInit();
            this.gbNetworkSettings.ResumeLayout(false);
            this.gbNetworkSettings.PerformLayout();
            this.gbDisplaySettings.ResumeLayout(false);
            this.gbDisplaySettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbTunerDevice;
        private System.Windows.Forms.GroupBox gbGeneralSettings;
        private System.Windows.Forms.ComboBox cbAutoSnapInterval;
        private System.Windows.Forms.GroupBox gbNetworkSettings;
        private System.Windows.Forms.TextBox TB_ServerIP;
        private System.Windows.Forms.ComboBox cbNetworkSources;
        private System.Windows.Forms.GroupBox gbDisplaySettings;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton B_RefreshSources;
        private System.Windows.Forms.NumericUpDown udMaximumSnaps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.CheckBox CHK_ShowNetwork;
        private System.Windows.Forms.ComboBox cbTunerInputType;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton B_resetUserSettings;
        private System.Windows.Forms.ComboBox cbSourceType;
        private System.Windows.Forms.ComboBox cbInputMode;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnVideoDecoderSettings;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}