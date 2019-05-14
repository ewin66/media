namespace FutureConcepts.Media.SVD.Controls
{
    partial class ProfileGroupSelector
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
            this.gbContent = new System.Windows.Forms.GroupBox();
            this.pProfileButtons = new System.Windows.Forms.Panel();
            this.btnCustomProfile = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.pTrackbars = new System.Windows.Forms.Panel();
            this.lblFramerateFPS = new System.Windows.Forms.Label();
            this.lblFramerate = new System.Windows.Forms.Label();
            this.tbFramerate = new FutureConcepts.Media.CommonControls.FCTrackbar();
            this.lblBitrateKBPS = new System.Windows.Forms.Label();
            this.lblBitrate = new System.Windows.Forms.Label();
            this.tbBitrate = new FutureConcepts.Media.CommonControls.FCTrackbar();
            this.cbProfileGroups = new System.Windows.Forms.ComboBox();
            this.gbContent.SuspendLayout();
            this.pTrackbars.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbContent
            // 
            this.gbContent.Controls.Add(this.pProfileButtons);
            this.gbContent.Controls.Add(this.btnCustomProfile);
            this.gbContent.Controls.Add(this.pTrackbars);
            this.gbContent.Controls.Add(this.cbProfileGroups);
            this.gbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbContent.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbContent.ForeColor = System.Drawing.Color.White;
            this.gbContent.Location = new System.Drawing.Point(0, 0);
            this.gbContent.Name = "gbContent";
            this.gbContent.Padding = new System.Windows.Forms.Padding(1);
            this.gbContent.Size = new System.Drawing.Size(100, 431);
            this.gbContent.TabIndex = 0;
            this.gbContent.TabStop = false;
            this.gbContent.Text = "Profile Selector";
            // 
            // pProfileButtons
            // 
            this.pProfileButtons.AutoScroll = true;
            this.pProfileButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pProfileButtons.Location = new System.Drawing.Point(1, 36);
            this.pProfileButtons.Name = "pProfileButtons";
            this.pProfileButtons.Size = new System.Drawing.Size(98, 199);
            this.pProfileButtons.TabIndex = 2;
            // 
            // btnCustomProfile
            // 
            this.btnCustomProfile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnCustomProfile.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCustomProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomProfile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomProfile.ForeColor = System.Drawing.Color.White;
            this.btnCustomProfile.Location = new System.Drawing.Point(1, 235);
            this.btnCustomProfile.Name = "btnCustomProfile";
            this.btnCustomProfile.Size = new System.Drawing.Size(98, 23);
            this.btnCustomProfile.TabIndex = 6;
            this.btnCustomProfile.Text = "[ Custom ]";
            this.btnCustomProfile.UseVisualStyleBackColor = false;
            this.btnCustomProfile.Visible = false;
            this.btnCustomProfile.Click += new System.EventHandler(this.ProfileButton_Click);
            // 
            // pTrackbars
            // 
            this.pTrackbars.Controls.Add(this.lblFramerateFPS);
            this.pTrackbars.Controls.Add(this.lblFramerate);
            this.pTrackbars.Controls.Add(this.tbFramerate);
            this.pTrackbars.Controls.Add(this.lblBitrateKBPS);
            this.pTrackbars.Controls.Add(this.lblBitrate);
            this.pTrackbars.Controls.Add(this.tbBitrate);
            this.pTrackbars.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pTrackbars.Location = new System.Drawing.Point(1, 258);
            this.pTrackbars.Name = "pTrackbars";
            this.pTrackbars.Size = new System.Drawing.Size(98, 172);
            this.pTrackbars.TabIndex = 1;
            // 
            // lblFramerateFPS
            // 
            this.lblFramerateFPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFramerateFPS.Location = new System.Drawing.Point(54, 140);
            this.lblFramerateFPS.Name = "lblFramerateFPS";
            this.lblFramerateFPS.Size = new System.Drawing.Size(34, 32);
            this.lblFramerateFPS.TabIndex = 5;
            this.lblFramerateFPS.Text = "fps";
            this.lblFramerateFPS.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblFramerate
            // 
            this.lblFramerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFramerate.AutoSize = true;
            this.lblFramerate.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFramerate.Location = new System.Drawing.Point(48, 3);
            this.lblFramerate.Name = "lblFramerate";
            this.lblFramerate.Size = new System.Drawing.Size(48, 11);
            this.lblFramerate.TabIndex = 4;
            this.lblFramerate.Text = "Framerate";
            // 
            // tbFramerate
            // 
            this.tbFramerate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFramerate.BackColor = System.Drawing.Color.Transparent;
            this.tbFramerate.CurrentValue = 1;
            this.tbFramerate.CustomTicks = new double[] {
        1,
        3,
        5,
        10,
        15,
        30};
            this.tbFramerate.DisabledColor = System.Drawing.Color.DimGray;
            this.tbFramerate.Enabled = false;
            this.tbFramerate.ForeColor = System.Drawing.Color.White;
            this.tbFramerate.Location = new System.Drawing.Point(59, 17);
            this.tbFramerate.Maximum = 30;
            this.tbFramerate.Minimum = 1;
            this.tbFramerate.Name = "tbFramerate";
            this.tbFramerate.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbFramerate.RestrictToCustomTicks = true;
            this.tbFramerate.Size = new System.Drawing.Size(24, 119);
            this.tbFramerate.TabIndex = 3;
            this.tbFramerate.TickColor = System.Drawing.Color.Gray;
            this.tbFramerate.TickFrequency = 1;
            this.tbFramerate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbFramerate.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.tbFramerate.TrackWidth = 4;
            this.tbFramerate.Value = 1;
            this.tbFramerate.ValueChanged += new System.EventHandler(this.tbFramerate_ValueChanged);
            this.tbFramerate.Scroll += new System.EventHandler(this.tbFramerate_Scroll);
            // 
            // lblBitrateKBPS
            // 
            this.lblBitrateKBPS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBitrateKBPS.Location = new System.Drawing.Point(4, 140);
            this.lblBitrateKBPS.Name = "lblBitrateKBPS";
            this.lblBitrateKBPS.Size = new System.Drawing.Size(43, 32);
            this.lblBitrateKBPS.TabIndex = 2;
            this.lblBitrateKBPS.Text = "kbps";
            this.lblBitrateKBPS.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblBitrate
            // 
            this.lblBitrate.AutoSize = true;
            this.lblBitrate.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBitrate.Location = new System.Drawing.Point(10, 3);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(32, 11);
            this.lblBitrate.TabIndex = 1;
            this.lblBitrate.Text = "Bitrate";
            // 
            // tbBitrate
            // 
            this.tbBitrate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tbBitrate.BackColor = System.Drawing.Color.Transparent;
            this.tbBitrate.CurrentValue = 0;
            this.tbBitrate.CustomTicks = null;
            this.tbBitrate.DisabledColor = System.Drawing.Color.DimGray;
            this.tbBitrate.Enabled = false;
            this.tbBitrate.ForeColor = System.Drawing.Color.White;
            this.tbBitrate.Location = new System.Drawing.Point(13, 17);
            this.tbBitrate.Maximum = 10;
            this.tbBitrate.Minimum = 0;
            this.tbBitrate.Name = "tbBitrate";
            this.tbBitrate.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tbBitrate.RestrictToCustomTicks = false;
            this.tbBitrate.Size = new System.Drawing.Size(24, 119);
            this.tbBitrate.TabIndex = 0;
            this.tbBitrate.TickColor = System.Drawing.Color.Gray;
            this.tbBitrate.TickFrequency = 1;
            this.tbBitrate.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbBitrate.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.tbBitrate.TrackWidth = 4;
            this.tbBitrate.Value = 0;
            this.tbBitrate.ValueChanged += new System.EventHandler(this.tbBitrate_ValueChanged);
            this.tbBitrate.Scroll += new System.EventHandler(this.tbBitrate_Scroll);
            // 
            // cbProfileGroups
            // 
            this.cbProfileGroups.BackColor = System.Drawing.Color.Black;
            this.cbProfileGroups.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbProfileGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfileGroups.ForeColor = System.Drawing.Color.White;
            this.cbProfileGroups.FormattingEnabled = true;
            this.cbProfileGroups.Location = new System.Drawing.Point(1, 15);
            this.cbProfileGroups.Name = "cbProfileGroups";
            this.cbProfileGroups.Size = new System.Drawing.Size(98, 21);
            this.cbProfileGroups.TabIndex = 0;
            this.cbProfileGroups.SelectedIndexChanged += new System.EventHandler(this.cbProfileGroups_SelectedIndexChanged);
            // 
            // ProfileGroupSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.gbContent);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ProfileGroupSelector";
            this.Size = new System.Drawing.Size(100, 431);
            this.gbContent.ResumeLayout(false);
            this.pTrackbars.ResumeLayout(false);
            this.pTrackbars.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbContent;
        private System.Windows.Forms.ComboBox cbProfileGroups;
        private System.Windows.Forms.Panel pProfileButtons;
        private System.Windows.Forms.Panel pTrackbars;
        private System.Windows.Forms.Label lblBitrate;
        private FutureConcepts.Media.CommonControls.FCTrackbar tbBitrate;
        private System.Windows.Forms.Label lblBitrateKBPS;
        private System.Windows.Forms.Label lblFramerateFPS;
        private System.Windows.Forms.Label lblFramerate;
        private FutureConcepts.Media.CommonControls.FCTrackbar tbFramerate;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnCustomProfile;
    }
}
