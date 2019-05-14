namespace FutureConcepts.Media.TV.Scanner
{
    partial class FavoriteChannels
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
            System.Windows.Forms.Label lblSeconds;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FavoriteChannels));
            this.panelThumbnails = new System.Windows.Forms.TableLayoutPanel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnAddFavorite = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FlatBlueButton();
            this.gbScanInterval = new System.Windows.Forms.GroupBox();
            this.cbInterval = new System.Windows.Forms.ComboBox();
            this.btnClear = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilClear = new System.Windows.Forms.ImageList(this.components);
            this.btnScan = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilScan = new System.Windows.Forms.ImageList(this.components);
            this.timerScanCycle = new System.Windows.Forms.Timer(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.panelScroller = new System.Windows.Forms.Panel();
            lblSeconds = new System.Windows.Forms.Label();
            this.panelControls.SuspendLayout();
            this.gbScanInterval.SuspendLayout();
            this.panelScroller.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSeconds
            // 
            lblSeconds.AutoSize = true;
            lblSeconds.Location = new System.Drawing.Point(26, 43);
            lblSeconds.Name = "lblSeconds";
            lblSeconds.Size = new System.Drawing.Size(46, 13);
            lblSeconds.TabIndex = 1;
            lblSeconds.Text = "seconds";
            // 
            // panelThumbnails
            // 
            this.panelThumbnails.AutoSize = true;
            this.panelThumbnails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelThumbnails.ColumnCount = 1;
            this.panelThumbnails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.panelThumbnails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelThumbnails.Location = new System.Drawing.Point(0, 0);
            this.panelThumbnails.Margin = new System.Windows.Forms.Padding(0);
            this.panelThumbnails.Name = "panelThumbnails";
            this.panelThumbnails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelThumbnails.Size = new System.Drawing.Size(190, 0);
            this.panelThumbnails.TabIndex = 50;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.btnAddFavorite);
            this.panelControls.Controls.Add(this.gbScanInterval);
            this.panelControls.Controls.Add(this.btnClear);
            this.panelControls.Controls.Add(this.btnScan);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControls.Location = new System.Drawing.Point(0, 539);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(190, 102);
            this.panelControls.TabIndex = 49;
            // 
            // btnAddFavorite
            // 
            this.btnAddFavorite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnAddFavorite.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddFavorite.Editable = true;
            this.btnAddFavorite.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAddFavorite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(140)))), ((int)(((byte)(79)))));
            this.btnAddFavorite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(56)))), ((int)(((byte)(130)))));
            this.btnAddFavorite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddFavorite.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddFavorite.ForeColor = System.Drawing.Color.White;
            this.btnAddFavorite.Location = new System.Drawing.Point(0, 0);
            this.btnAddFavorite.Name = "btnAddFavorite";
            this.btnAddFavorite.Size = new System.Drawing.Size(190, 30);
            this.btnAddFavorite.TabIndex = 48;
            this.btnAddFavorite.Text = "Add Favorite Channel";
            this.tt.SetToolTip(this.btnAddFavorite, "Add the current channel as a favorite.");
            this.btnAddFavorite.UseVisualStyleBackColor = true;
            this.btnAddFavorite.Click += new System.EventHandler(this.btnAddFavorite_Click);
            // 
            // gbScanInterval
            // 
            this.gbScanInterval.Controls.Add(lblSeconds);
            this.gbScanInterval.Controls.Add(this.cbInterval);
            this.gbScanInterval.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbScanInterval.ForeColor = System.Drawing.Color.White;
            this.gbScanInterval.Location = new System.Drawing.Point(3, 33);
            this.gbScanInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbScanInterval.Name = "gbScanInterval";
            this.gbScanInterval.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbScanInterval.Size = new System.Drawing.Size(78, 65);
            this.gbScanInterval.TabIndex = 10;
            this.gbScanInterval.TabStop = false;
            this.gbScanInterval.Text = "Interval";
            // 
            // cbInterval
            // 
            this.cbInterval.BackColor = System.Drawing.Color.Black;
            this.cbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInterval.ForeColor = System.Drawing.Color.White;
            this.cbInterval.FormattingEnabled = true;
            this.cbInterval.Items.AddRange(new object[] {
            "3",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "40",
            "50",
            "60"});
            this.cbInterval.Location = new System.Drawing.Point(7, 18);
            this.cbInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbInterval.Name = "cbInterval";
            this.cbInterval.Size = new System.Drawing.Size(63, 21);
            this.cbInterval.TabIndex = 0;
            this.tt.SetToolTip(this.cbInterval, "Amount of time to pause on each channel.");
            this.cbInterval.SelectedIndexChanged += new System.EventHandler(this.cbInterval_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.ImageIndex = 0;
            this.btnClear.ImageList = this.ilClear;
            this.btnClear.Location = new System.Drawing.Point(92, 70);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 30);
            this.btnClear.TabIndex = 36;
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnClear.Toggle = false;
            this.tt.SetToolTip(this.btnClear, "Click to clear all of the listed channels.");
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ilClear
            // 
            this.ilClear.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilClear.ImageStream")));
            this.ilClear.TransparentColor = System.Drawing.Color.Transparent;
            this.ilClear.Images.SetKeyName(0, "clearfavorites-up.gif");
            this.ilClear.Images.SetKeyName(1, "clearfavorites-down.gif");
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.ForeColor = System.Drawing.Color.White;
            this.btnScan.ImageIndex = 0;
            this.btnScan.ImageList = this.ilScan;
            this.btnScan.Location = new System.Drawing.Point(92, 36);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(92, 30);
            this.btnScan.TabIndex = 35;
            this.btnScan.Tag = "Scan";
            this.btnScan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnScan.Toggle = true;
            this.tt.SetToolTip(this.btnScan, "Click to begin scanning favorite channels.");
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // ilScan
            // 
            this.ilScan.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilScan.ImageStream")));
            this.ilScan.TransparentColor = System.Drawing.Color.Transparent;
            this.ilScan.Images.SetKeyName(0, "scanfavorites-up.gif");
            this.ilScan.Images.SetKeyName(1, "scanfavorites-down.gif");
            // 
            // timerScanCycle
            // 
            this.timerScanCycle.Tick += new System.EventHandler(this.timerScanCycle_Tick);
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // panelScroller
            // 
            this.panelScroller.AutoScroll = true;
            this.panelScroller.Controls.Add(this.panelThumbnails);
            this.panelScroller.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScroller.Location = new System.Drawing.Point(0, 0);
            this.panelScroller.Name = "panelScroller";
            this.panelScroller.Size = new System.Drawing.Size(190, 539);
            this.panelScroller.TabIndex = 0;
            // 
            // FavoriteChannels
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.panelScroller);
            this.Controls.Add(this.panelControls);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FavoriteChannels";
            this.Size = new System.Drawing.Size(190, 641);
            this.Load += new System.EventHandler(this.FavoriteChannels_Load);
            this.panelControls.ResumeLayout(false);
            this.gbScanInterval.ResumeLayout(false);
            this.gbScanInterval.PerformLayout();
            this.panelScroller.ResumeLayout(false);
            this.panelScroller.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel panelThumbnails;
        private System.Windows.Forms.Panel panelControls;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FlatBlueButton btnAddFavorite;
        private System.Windows.Forms.GroupBox gbScanInterval;
        private System.Windows.Forms.ComboBox cbInterval;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnClear;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnScan;
        private System.Windows.Forms.ImageList ilScan;
        private System.Windows.Forms.ImageList ilClear;
        private System.Windows.Forms.Timer timerScanCycle;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.Panel panelScroller;
    }
}
