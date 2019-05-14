namespace FutureConcepts.Media.SVD.Controls
{
    partial class PanTiltZoomAbsoluteControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanTiltZoomAbsoluteControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.L_zoomValue = new System.Windows.Forms.Label();
            this.L_zoom = new System.Windows.Forms.Label();
            this.ilNudgePanCounterclockwise = new System.Windows.Forms.ImageList(this.components);
            this.ilNudgePanClockwise = new System.Windows.Forms.ImageList(this.components);
            this.L_panAngle = new System.Windows.Forms.Label();
            this.L_tiltValue = new System.Windows.Forms.Label();
            this.L_tiltAngle = new System.Windows.Forms.Label();
            this.TB_zoomLevel = new FutureConcepts.Media.CommonControls.FCTrackbar();
            this.TB_tiltAngle = new FutureConcepts.Media.CommonControls.FCTrackbar();
            this.B_nudgePanCounterclockwise = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.B_nudgePanClockwise = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.TB_panAngle = new FutureConcepts.Media.CommonControls.FCCircularTrackbar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Black;
            this.groupBox1.Controls.Add(this.TB_zoomLevel);
            this.groupBox1.Controls.Add(this.TB_tiltAngle);
            this.groupBox1.Controls.Add(this.L_zoomValue);
            this.groupBox1.Controls.Add(this.L_zoom);
            this.groupBox1.Controls.Add(this.B_nudgePanCounterclockwise);
            this.groupBox1.Controls.Add(this.B_nudgePanClockwise);
            this.groupBox1.Controls.Add(this.L_panAngle);
            this.groupBox1.Controls.Add(this.L_tiltValue);
            this.groupBox1.Controls.Add(this.L_tiltAngle);
            this.groupBox1.Controls.Add(this.TB_panAngle);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.MinimumSize = new System.Drawing.Size(265, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 165);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // L_zoomValue
            // 
            this.L_zoomValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.L_zoomValue.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_zoomValue.Location = new System.Drawing.Point(214, 148);
            this.L_zoomValue.Name = "L_zoomValue";
            this.L_zoomValue.Size = new System.Drawing.Size(45, 13);
            this.L_zoomValue.TabIndex = 9;
            this.L_zoomValue.Text = "1x";
            this.L_zoomValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // L_zoom
            // 
            this.L_zoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.L_zoom.AutoSize = true;
            this.L_zoom.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_zoom.Location = new System.Drawing.Point(220, 1);
            this.L_zoom.Name = "L_zoom";
            this.L_zoom.Size = new System.Drawing.Size(33, 13);
            this.L_zoom.TabIndex = 8;
            this.L_zoom.Text = "Zoom";
            // 
            // ilNudgePanCounterclockwise
            // 
            this.ilNudgePanCounterclockwise.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNudgePanCounterclockwise.ImageStream")));
            this.ilNudgePanCounterclockwise.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNudgePanCounterclockwise.Images.SetKeyName(0, "nudge_anticlockwise-up.gif");
            this.ilNudgePanCounterclockwise.Images.SetKeyName(1, "nudge_anticlockwise-down.gif");
            // 
            // ilNudgePanClockwise
            // 
            this.ilNudgePanClockwise.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilNudgePanClockwise.ImageStream")));
            this.ilNudgePanClockwise.TransparentColor = System.Drawing.Color.Transparent;
            this.ilNudgePanClockwise.Images.SetKeyName(0, "nudge_clockwise-up.gif");
            this.ilNudgePanClockwise.Images.SetKeyName(1, "nudge_clockwise-down.gif");
            // 
            // L_panAngle
            // 
            this.L_panAngle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.L_panAngle.AutoSize = true;
            this.L_panAngle.Location = new System.Drawing.Point(50, 1);
            this.L_panAngle.Name = "L_panAngle";
            this.L_panAngle.Size = new System.Drawing.Size(55, 13);
            this.L_panAngle.TabIndex = 3;
            this.L_panAngle.Text = "Pan Angle";
            // 
            // L_tiltValue
            // 
            this.L_tiltValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.L_tiltValue.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_tiltValue.Location = new System.Drawing.Point(158, 148);
            this.L_tiltValue.Name = "L_tiltValue";
            this.L_tiltValue.Size = new System.Drawing.Size(48, 13);
            this.L_tiltValue.TabIndex = 6;
            this.L_tiltValue.Text = "0°";
            this.L_tiltValue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // L_tiltAngle
            // 
            this.L_tiltAngle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.L_tiltAngle.AutoSize = true;
            this.L_tiltAngle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.L_tiltAngle.Location = new System.Drawing.Point(155, 1);
            this.L_tiltAngle.Name = "L_tiltAngle";
            this.L_tiltAngle.Size = new System.Drawing.Size(51, 13);
            this.L_tiltAngle.TabIndex = 2;
            this.L_tiltAngle.Text = "Tilt Angle";
            // 
            // TB_zoomLevel
            // 
            this.TB_zoomLevel.BackColor = System.Drawing.Color.Transparent;
            this.TB_zoomLevel.CurrentValue = 1;
            this.TB_zoomLevel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TB_zoomLevel.CustomTicks = null;
            this.TB_zoomLevel.DisabledColor = System.Drawing.Color.Gray;
            this.TB_zoomLevel.ForeColor = System.Drawing.Color.White;
            this.TB_zoomLevel.Location = new System.Drawing.Point(221, 17);
            this.TB_zoomLevel.Maximum = 40;
            this.TB_zoomLevel.Minimum = 1;
            this.TB_zoomLevel.Name = "TB_zoomLevel";
            this.TB_zoomLevel.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.TB_zoomLevel.RestrictToCustomTicks = false;
            this.TB_zoomLevel.Size = new System.Drawing.Size(30, 128);
            this.TB_zoomLevel.TabIndex = 11;
            this.TB_zoomLevel.TickColor = System.Drawing.Color.DarkGray;
            this.TB_zoomLevel.TickFrequency = 999999999;
            this.TB_zoomLevel.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TB_zoomLevel.TrackClickStepValue = 0;
            this.TB_zoomLevel.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.TB_zoomLevel.TrackWidth = 5;
            this.TB_zoomLevel.Value = 1;
            this.TB_zoomLevel.ValueChanged += new System.EventHandler(this.TB_zoomLevel_ValueChanged);
            this.TB_zoomLevel.Scroll += new System.EventHandler(this.TB_zoomLevel_Scroll);
            // 
            // TB_tiltAngle
            // 
            this.TB_tiltAngle.BackColor = System.Drawing.Color.Transparent;
            this.TB_tiltAngle.CurrentValue = 0;
            this.TB_tiltAngle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TB_tiltAngle.CustomTicks = null;
            this.TB_tiltAngle.DisabledColor = System.Drawing.Color.Gray;
            this.TB_tiltAngle.ForeColor = System.Drawing.Color.White;
            this.TB_tiltAngle.Location = new System.Drawing.Point(165, 17);
            this.TB_tiltAngle.Maximum = 90;
            this.TB_tiltAngle.Minimum = -90;
            this.TB_tiltAngle.Name = "TB_tiltAngle";
            this.TB_tiltAngle.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.TB_tiltAngle.RestrictToCustomTicks = false;
            this.TB_tiltAngle.Size = new System.Drawing.Size(30, 128);
            this.TB_tiltAngle.TabIndex = 10;
            this.TB_tiltAngle.TickColor = System.Drawing.Color.DarkGray;
            this.TB_tiltAngle.TickFrequency = 999999;
            this.TB_tiltAngle.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.TB_tiltAngle.TrackClickStepValue = 0;
            this.TB_tiltAngle.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.TB_tiltAngle.TrackWidth = 5;
            this.TB_tiltAngle.Value = 0;
            this.TB_tiltAngle.ValueChanged += new System.EventHandler(this.TB_tiltAngle_ValueChanged);
            this.TB_tiltAngle.Scroll += new System.EventHandler(this.TB_tiltAngle_Scroll);
            // 
            // B_nudgePanCounterclockwise
            // 
            this.B_nudgePanCounterclockwise.BackColor = System.Drawing.Color.Transparent;
            this.B_nudgePanCounterclockwise.ImageIndex = 0;
            this.B_nudgePanCounterclockwise.ImageList = this.ilNudgePanCounterclockwise;
            this.B_nudgePanCounterclockwise.Location = new System.Drawing.Point(6, 12);
            this.B_nudgePanCounterclockwise.Name = "B_nudgePanCounterclockwise";
            this.B_nudgePanCounterclockwise.Size = new System.Drawing.Size(20, 20);
            this.B_nudgePanCounterclockwise.TabIndex = 5;
            this.B_nudgePanCounterclockwise.Toggle = false;
            this.B_nudgePanCounterclockwise.MouseLeave += new System.EventHandler(this.B_nudgePan_MouseLeave);
            this.B_nudgePanCounterclockwise.MouseMove += new System.Windows.Forms.MouseEventHandler(this.B_nudgePan_MouseMove);
            this.B_nudgePanCounterclockwise.Click += new System.EventHandler(this.B_nudgePan_Click);
            this.B_nudgePanCounterclockwise.MouseEnter += new System.EventHandler(this.B_nudgePan_MouseEnter);
            // 
            // B_nudgePanClockwise
            // 
            this.B_nudgePanClockwise.BackColor = System.Drawing.Color.Transparent;
            this.B_nudgePanClockwise.ImageIndex = 0;
            this.B_nudgePanClockwise.ImageList = this.ilNudgePanClockwise;
            this.B_nudgePanClockwise.Location = new System.Drawing.Point(133, 11);
            this.B_nudgePanClockwise.Name = "B_nudgePanClockwise";
            this.B_nudgePanClockwise.Size = new System.Drawing.Size(20, 20);
            this.B_nudgePanClockwise.TabIndex = 4;
            this.B_nudgePanClockwise.Toggle = false;
            this.B_nudgePanClockwise.MouseLeave += new System.EventHandler(this.B_nudgePan_MouseLeave);
            this.B_nudgePanClockwise.MouseMove += new System.Windows.Forms.MouseEventHandler(this.B_nudgePan_MouseMove);
            this.B_nudgePanClockwise.Click += new System.EventHandler(this.B_nudgePan_Click);
            this.B_nudgePanClockwise.MouseEnter += new System.EventHandler(this.B_nudgePan_MouseEnter);
            // 
            // TB_panAngle
            // 
            this.TB_panAngle.BackColor = System.Drawing.Color.Transparent;
            this.TB_panAngle.DisabledColor = System.Drawing.Color.DarkGray;
            this.TB_panAngle.DisplayValue = true;
            this.TB_panAngle.DisplayValue180 = true;
            this.TB_panAngle.DisplayValueFormatString = "0°";
            this.TB_panAngle.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_panAngle.ForeColor = System.Drawing.Color.White;
            this.TB_panAngle.Location = new System.Drawing.Point(3, 11);
            this.TB_panAngle.Margin = new System.Windows.Forms.Padding(0);
            this.TB_panAngle.Name = "TB_panAngle";
            this.TB_panAngle.Padding = new System.Windows.Forms.Padding(10);
            this.TB_panAngle.Size = new System.Drawing.Size(150, 150);
            this.TB_panAngle.TabIndex = 0;
            this.TB_panAngle.ThumbAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.TB_panAngle.ThumbImage = null;
            this.TB_panAngle.TrackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.TB_panAngle.TrackDiameter = 118;
            this.TB_panAngle.TrackImage = null;
            this.TB_panAngle.TrackWidth = 6;
            this.TB_panAngle.Value = 0;
            this.TB_panAngle.ValueChanged += new System.EventHandler(this.TB_panAngle_ValueChanged);
            // 
            // PanTiltZoomAbsoluteControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(265, 165);
            this.Name = "PanTiltZoomAbsoluteControl";
            this.Size = new System.Drawing.Size(265, 165);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private FutureConcepts.Media.CommonControls.FCCircularTrackbar TB_panAngle;
        private System.Windows.Forms.Label L_tiltAngle;
        private System.Windows.Forms.Label L_panAngle;
        private FutureConcepts.Media.CommonControls.FCClickButton B_nudgePanCounterclockwise;
        private FutureConcepts.Media.CommonControls.FCClickButton B_nudgePanClockwise;
        private System.Windows.Forms.Label L_tiltValue;
        private System.Windows.Forms.ImageList ilNudgePanClockwise;
        private System.Windows.Forms.ImageList ilNudgePanCounterclockwise;
        private System.Windows.Forms.Label L_zoomValue;
        private System.Windows.Forms.Label L_zoom;
        private FutureConcepts.Media.CommonControls.FCTrackbar TB_zoomLevel;
        private FutureConcepts.Media.CommonControls.FCTrackbar TB_tiltAngle;
    }
}
