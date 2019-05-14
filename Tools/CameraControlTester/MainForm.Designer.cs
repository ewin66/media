namespace FutureConcepts.Media.Tools.CameraControlTester
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
            this.ptzControl = new FutureConcepts.Media.SVD.Controls.PanTiltZoomAbsoluteControl();
            this.btnInq = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnConfig = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnStartStop = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnPelcoDAddressSweep = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.ilOrientation = new System.Windows.Forms.ImageList(this.components);
            this.btnManualInput = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.SuspendLayout();
            // 
            // ptzControl
            // 
            this.ptzControl.BackColor = System.Drawing.Color.Black;
            this.ptzControl.CameraFieldOfView = 0;
            this.ptzControl.Enabled = false;
            this.ptzControl.ForeColor = System.Drawing.Color.White;
            this.ptzControl.Location = new System.Drawing.Point(12, 41);
            this.ptzControl.MinimumSize = new System.Drawing.Size(265, 165);
            this.ptzControl.Name = "ptzControl";
            this.ptzControl.PanAngle = 0;
            this.ptzControl.PanAngleFormatString = "0.##°";
            this.ptzControl.PanAngleOffset = 0;
            this.ptzControl.PanEnabled = true;
            this.ptzControl.Size = new System.Drawing.Size(265, 165);
            this.ptzControl.TabIndex = 0;
            this.ptzControl.TiltAngle = 0;
            this.ptzControl.TiltAngleCustomTicks = null;
            this.ptzControl.TiltAngleFormatString = "0.##°";
            this.ptzControl.TiltAngleMaximum = 90;
            this.ptzControl.TiltAngleMinimum = -90;
            this.ptzControl.TiltAngleTickFrequency = 999999;
            this.ptzControl.TiltEnabled = true;
            this.ptzControl.ZoomEnabled = true;
            this.ptzControl.ZoomLevel = 1;
            this.ptzControl.ZoomLevelCustomTicks = null;
            this.ptzControl.ZoomLevelFormatString = "0.##x";
            this.ptzControl.ZoomLevelMaximum = 40;
            this.ptzControl.ZoomLevelMinimum = 1;
            this.ptzControl.ZoomLevelTickFrequency = 999999999;
            this.ptzControl.ZoomLevelChanged += new System.EventHandler(this.ptzControl_ZoomLevelChanged);
            this.ptzControl.TiltAngleChanged += new System.EventHandler(this.ptzControl_TiltAngleChanged);
            this.ptzControl.PanAngleChanged += new System.EventHandler(this.ptzControl_PanAngleChanged);
            // 
            // btnInq
            // 
            this.btnInq.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnInq.Enabled = false;
            this.btnInq.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInq.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInq.ForeColor = System.Drawing.Color.White;
            this.btnInq.Location = new System.Drawing.Point(93, 212);
            this.btnInq.Name = "btnInq";
            this.btnInq.Size = new System.Drawing.Size(184, 23);
            this.btnInq.TabIndex = 1;
            this.btnInq.Text = "PanTiltZoomPositionInquiry";
            this.btnInq.UseVisualStyleBackColor = false;
            this.btnInq.Click += new System.EventHandler(this.btnInq_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfig.ForeColor = System.Drawing.Color.White;
            this.btnConfig.Location = new System.Drawing.Point(12, 212);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 2;
            this.btnConfig.Text = "Config...";
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.Color.Green;
            this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStop.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.ForeColor = System.Drawing.Color.White;
            this.btnStartStop.Location = new System.Drawing.Point(12, 12);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(265, 23);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnStartStop_MouseClick);
            // 
            // btnPelcoDAddressSweep
            // 
            this.btnPelcoDAddressSweep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnPelcoDAddressSweep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPelcoDAddressSweep.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPelcoDAddressSweep.ForeColor = System.Drawing.Color.White;
            this.btnPelcoDAddressSweep.Location = new System.Drawing.Point(12, 241);
            this.btnPelcoDAddressSweep.Name = "btnPelcoDAddressSweep";
            this.btnPelcoDAddressSweep.Size = new System.Drawing.Size(265, 23);
            this.btnPelcoDAddressSweep.TabIndex = 4;
            this.btnPelcoDAddressSweep.Text = "Do PelcoD Address Sweep";
            this.btnPelcoDAddressSweep.UseVisualStyleBackColor = false;
            this.btnPelcoDAddressSweep.Click += new System.EventHandler(this.btnPelcoDAddressSweep_Click);
            // 
            // ilOrientation
            // 
            this.ilOrientation.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilOrientation.ImageStream")));
            this.ilOrientation.TransparentColor = System.Drawing.Color.Transparent;
            this.ilOrientation.Images.SetKeyName(0, "first-up.gif");
            this.ilOrientation.Images.SetKeyName(1, "first-down.gif");
            // 
            // btnManualInput
            // 
            this.btnManualInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnManualInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualInput.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualInput.ForeColor = System.Drawing.Color.White;
            this.btnManualInput.Location = new System.Drawing.Point(12, 270);
            this.btnManualInput.Name = "btnManualInput";
            this.btnManualInput.Size = new System.Drawing.Size(265, 23);
            this.btnManualInput.TabIndex = 5;
            this.btnManualInput.Text = "Manual Input...";
            this.btnManualInput.UseVisualStyleBackColor = false;
            this.btnManualInput.Click += new System.EventHandler(this.btnManualInput_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(292, 301);
            this.Controls.Add(this.btnManualInput);
            this.Controls.Add(this.btnPelcoDAddressSweep);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnInq);
            this.Controls.Add(this.ptzControl);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera Control Tester";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private FutureConcepts.Media.SVD.Controls.PanTiltZoomAbsoluteControl ptzControl;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnInq;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnConfig;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnStartStop;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnPelcoDAddressSweep;
        private System.Windows.Forms.ImageList ilOrientation;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnManualInput;
    }
}

