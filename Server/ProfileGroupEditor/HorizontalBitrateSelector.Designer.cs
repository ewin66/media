namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    partial class HorizontalBitrateSelector
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
            this.label = new System.Windows.Forms.Label();
            this.units = new System.Windows.Forms.Label();
            this.updown = new System.Windows.Forms.NumericUpDown();
            this.trackbar = new FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar();
            ((System.ComponentModel.ISupportInitialize)(this.updown)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.ForeColor = System.Drawing.Color.White;
            this.label.Location = new System.Drawing.Point(3, 6);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(35, 13);
            this.label.TabIndex = 0;
            this.label.Text = "label1";
            // 
            // units
            // 
            this.units.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.units.AutoSize = true;
            this.units.BackColor = System.Drawing.Color.Transparent;
            this.units.ForeColor = System.Drawing.Color.White;
            this.units.Location = new System.Drawing.Point(258, 6);
            this.units.Name = "units";
            this.units.Size = new System.Drawing.Size(30, 13);
            this.units.TabIndex = 1;
            this.units.Text = "kbps";
            // 
            // updown
            // 
            this.updown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updown.BackColor = System.Drawing.Color.Black;
            this.updown.ForeColor = System.Drawing.Color.White;
            this.updown.Location = new System.Drawing.Point(203, 4);
            this.updown.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.updown.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.updown.Name = "updown";
            this.updown.Size = new System.Drawing.Size(51, 20);
            this.updown.TabIndex = 2;
            this.updown.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.updown.ValueChanged += new System.EventHandler(this.updown_ValueChanged);
            // 
            // trackbar
            // 
            this.trackbar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackbar.BackColor = System.Drawing.Color.Transparent;
            this.trackbar.CurrentValue = 5;
            this.trackbar.CustomTicks = null;
            this.trackbar.DisabledColor = System.Drawing.Color.Gray;
            this.trackbar.ForeColor = System.Drawing.Color.White;
            this.trackbar.Location = new System.Drawing.Point(41, 2);
            this.trackbar.Maximum = 10;
            this.trackbar.Minimum = 5;
            this.trackbar.Name = "trackbar";
            this.trackbar.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.trackbar.RestrictToCustomTicks = false;
            this.trackbar.Size = new System.Drawing.Size(156, 24);
            this.trackbar.TabIndex = 3;
            this.trackbar.TickColor = System.Drawing.Color.DarkGray;
            this.trackbar.TickFrequency = 1;
            this.trackbar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackbar.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.trackbar.TrackWidth = 4;
            this.trackbar.Value = 5;
            this.trackbar.Scroll += new System.EventHandler(this.trackbar_Scroll);
            // 
            // HorizontalBitrateSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.trackbar);
            this.Controls.Add(this.updown);
            this.Controls.Add(this.units);
            this.Controls.Add(this.label);
            this.MinimumSize = new System.Drawing.Size(290, 29);
            this.Name = "HorizontalBitrateSelector";
            this.Size = new System.Drawing.Size(290, 29);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.HorizontalBitrateSelector_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.updown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label units;
        private System.Windows.Forms.NumericUpDown updown;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Controls.FCTrackbar trackbar;
    }
}
