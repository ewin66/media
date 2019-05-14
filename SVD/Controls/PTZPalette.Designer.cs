using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD.Controls
{
    partial class PTZPalette
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PTZPalette));
            this.GB_PTZOptions = new System.Windows.Forms.GroupBox();
            this.B_Wiper = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilWiper = new System.Windows.Forms.ImageList(this.components);
            this.B_Invert = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilInvert = new System.Windows.Forms.ImageList(this.components);
            this.B_Stabilizer = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilStabilizer = new System.Windows.Forms.ImageList(this.components);
            this.B_Infrared = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilInfrared = new System.Windows.Forms.ImageList(this.components);
            this.B_Emitter = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.ilEmitter = new System.Windows.Forms.ImageList(this.components);
            this.ptzFavoritesControl = new FutureConcepts.Media.SVD.Controls.UserPresetsControl();
            this.ptzControl = new FutureConcepts.Media.SVD.Controls.PanTiltZoomAbsoluteControl();
            this.GB_PTZOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // GB_PTZOptions
            // 
            this.GB_PTZOptions.Controls.Add(this.B_Wiper);
            this.GB_PTZOptions.Controls.Add(this.B_Invert);
            this.GB_PTZOptions.Controls.Add(this.B_Stabilizer);
            this.GB_PTZOptions.Controls.Add(this.B_Infrared);
            this.GB_PTZOptions.Controls.Add(this.B_Emitter);
            this.GB_PTZOptions.Dock = System.Windows.Forms.DockStyle.Left;
            this.GB_PTZOptions.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GB_PTZOptions.ForeColor = System.Drawing.Color.White;
            this.GB_PTZOptions.Location = new System.Drawing.Point(265, 0);
            this.GB_PTZOptions.MinimumSize = new System.Drawing.Size(110, 166);
            this.GB_PTZOptions.Name = "GB_PTZOptions";
            this.GB_PTZOptions.Size = new System.Drawing.Size(110, 166);
            this.GB_PTZOptions.TabIndex = 37;
            this.GB_PTZOptions.TabStop = false;
            this.GB_PTZOptions.Text = "PTZ Options";
            // 
            // B_Wiper
            // 
            this.B_Wiper.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Wiper.ImageIndex = 0;
            this.B_Wiper.ImageList = this.ilWiper;
            this.B_Wiper.Location = new System.Drawing.Point(4, 15);
            this.B_Wiper.Name = "B_Wiper";
            this.B_Wiper.Size = new System.Drawing.Size(48, 48);
            this.B_Wiper.TabIndex = 28;
            this.B_Wiper.Toggle = true;
            this.B_Wiper.Click += new System.EventHandler(this.Wiper_Click);
            // 
            // ilWiper
            // 
            this.ilWiper.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilWiper.ImageStream")));
            this.ilWiper.TransparentColor = System.Drawing.Color.Transparent;
            this.ilWiper.Images.SetKeyName(0, "wiper-up.gif");
            this.ilWiper.Images.SetKeyName(1, "wiper-down.gif");
            // 
            // B_Invert
            // 
            this.B_Invert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Invert.ImageIndex = 0;
            this.B_Invert.ImageList = this.ilInvert;
            this.B_Invert.Location = new System.Drawing.Point(4, 115);
            this.B_Invert.Name = "B_Invert";
            this.B_Invert.Size = new System.Drawing.Size(48, 48);
            this.B_Invert.TabIndex = 32;
            this.B_Invert.Toggle = true;
            this.B_Invert.Click += new System.EventHandler(this.Invert_Click);
            // 
            // ilInvert
            // 
            this.ilInvert.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilInvert.ImageStream")));
            this.ilInvert.TransparentColor = System.Drawing.Color.Transparent;
            this.ilInvert.Images.SetKeyName(0, "invert-up.gif");
            this.ilInvert.Images.SetKeyName(1, "invert-down.gif");
            // 
            // B_Stabilizer
            // 
            this.B_Stabilizer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Stabilizer.ImageIndex = 0;
            this.B_Stabilizer.ImageList = this.ilStabilizer;
            this.B_Stabilizer.Location = new System.Drawing.Point(55, 15);
            this.B_Stabilizer.Name = "B_Stabilizer";
            this.B_Stabilizer.Size = new System.Drawing.Size(48, 48);
            this.B_Stabilizer.TabIndex = 29;
            this.B_Stabilizer.Toggle = true;
            this.B_Stabilizer.Click += new System.EventHandler(this.Stabilizer_Click);
            // 
            // ilStabilizer
            // 
            this.ilStabilizer.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStabilizer.ImageStream")));
            this.ilStabilizer.TransparentColor = System.Drawing.Color.Transparent;
            this.ilStabilizer.Images.SetKeyName(0, "stabilizer-up.gif");
            this.ilStabilizer.Images.SetKeyName(1, "stabilizer-down.gif");
            // 
            // B_Infrared
            // 
            this.B_Infrared.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Infrared.ImageIndex = 0;
            this.B_Infrared.ImageList = this.ilInfrared;
            this.B_Infrared.Location = new System.Drawing.Point(55, 65);
            this.B_Infrared.Name = "B_Infrared";
            this.B_Infrared.Size = new System.Drawing.Size(48, 48);
            this.B_Infrared.TabIndex = 31;
            this.B_Infrared.Toggle = true;
            this.B_Infrared.Click += new System.EventHandler(this.Infrared_Click);
            // 
            // ilInfrared
            // 
            this.ilInfrared.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilInfrared.ImageStream")));
            this.ilInfrared.TransparentColor = System.Drawing.Color.Transparent;
            this.ilInfrared.Images.SetKeyName(0, "infrared-up.gif");
            this.ilInfrared.Images.SetKeyName(1, "infrared-down.gif");
            // 
            // B_Emitter
            // 
            this.B_Emitter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.B_Emitter.ImageIndex = 0;
            this.B_Emitter.ImageList = this.ilEmitter;
            this.B_Emitter.Location = new System.Drawing.Point(4, 65);
            this.B_Emitter.Name = "B_Emitter";
            this.B_Emitter.Size = new System.Drawing.Size(48, 48);
            this.B_Emitter.TabIndex = 30;
            this.B_Emitter.Toggle = true;
            this.B_Emitter.Click += new System.EventHandler(this.Emitter_Click);
            // 
            // ilEmitter
            // 
            this.ilEmitter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilEmitter.ImageStream")));
            this.ilEmitter.TransparentColor = System.Drawing.Color.Transparent;
            this.ilEmitter.Images.SetKeyName(0, "emitter-up.gif");
            this.ilEmitter.Images.SetKeyName(1, "emitter-down.gif");
            // 
            // ptzFavoritesControl
            // 
            this.ptzFavoritesControl.BackColor = System.Drawing.Color.Black;
            this.ptzFavoritesControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.ptzFavoritesControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ptzFavoritesControl.ForeColor = System.Drawing.Color.White;
            this.ptzFavoritesControl.Location = new System.Drawing.Point(375, 0);
            this.ptzFavoritesControl.MinimumSize = new System.Drawing.Size(333, 166);
            this.ptzFavoritesControl.Name = "ptzFavoritesControl";
            this.ptzFavoritesControl.Size = new System.Drawing.Size(333, 166);
            this.ptzFavoritesControl.TabIndex = 38;
            this.ptzFavoritesControl.Title = "PTZ Favorites";
            this.ptzFavoritesControl.PresetDeleted += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.ptzFavoritesControl_PresetDeleted);
            this.ptzFavoritesControl.AddPreset += new System.EventHandler(this.ptzFavoritesControl_AddFavorite);
            this.ptzFavoritesControl.PresetsCleared += new System.EventHandler(this.ptzFavoritesControl_PresetsCleared);
            this.ptzFavoritesControl.RestorePreset += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.ptzFavoritesControl_RestorePreset);
            this.ptzFavoritesControl.PresetRenamed += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.ptzFavoritesControl_PresetRenamed);
            // 
            // ptzControl
            // 
            this.ptzControl.BackColor = System.Drawing.Color.Black;
            this.ptzControl.CameraFieldOfView = 0;
            this.ptzControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.ptzControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.ptzControl.ForeColor = System.Drawing.Color.White;
            this.ptzControl.Location = new System.Drawing.Point(0, 0);
            this.ptzControl.MinimumSize = new System.Drawing.Size(265, 165);
            this.ptzControl.Name = "ptzControl";
            this.ptzControl.PanAngle = 0;
            this.ptzControl.PanAngleFormatString = "0.#°";
            this.ptzControl.PanAngleOffset = 0;
            this.ptzControl.PanEnabled = true;
            this.ptzControl.Size = new System.Drawing.Size(265, 166);
            this.ptzControl.TabIndex = 36;
            this.ptzControl.TiltAngle = 0;
            this.ptzControl.TiltAngleCustomTicks = null;
            this.ptzControl.TiltAngleFormatString = "0.#°";
            this.ptzControl.TiltAngleMaximum = 127;
            this.ptzControl.TiltAngleMinimum = 0;
            this.ptzControl.TiltAngleTickFrequency = 999999;
            this.ptzControl.TiltEnabled = true;
            this.ptzControl.ZoomEnabled = true;
            this.ptzControl.ZoomLevel = 1;
            this.ptzControl.ZoomLevelCustomTicks = null;
            this.ptzControl.ZoomLevelFormatString = "0.#x";
            this.ptzControl.ZoomLevelMaximum = 7000;
            this.ptzControl.ZoomLevelMinimum = 0;
            this.ptzControl.ZoomLevelTickFrequency = 999999999;
            this.ptzControl.ZoomLevelChanged += new System.EventHandler(this.ptzControl_ZoomLevelChanged);
            this.ptzControl.TiltAngleChanged += new System.EventHandler(this.ptzControl_TiltChanged);
            this.ptzControl.PanAngleChanged += new System.EventHandler(this.ptzControl_PanChanged);
            // 
            // PTZPalette
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.ptzFavoritesControl);
            this.Controls.Add(this.GB_PTZOptions);
            this.Controls.Add(this.ptzControl);
            this.Name = "PTZPalette";
            this.Size = new System.Drawing.Size(710, 166);
            this.VisibleChanged += new System.EventHandler(this.PTZPalette_VisibleChanged);
            this.GB_PTZOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PanTiltZoomAbsoluteControl ptzControl;
        private System.Windows.Forms.GroupBox GB_PTZOptions;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Wiper;
        private System.Windows.Forms.ImageList ilWiper;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Invert;
        private System.Windows.Forms.ImageList ilInvert;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Stabilizer;
        private System.Windows.Forms.ImageList ilStabilizer;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Infrared;
        private System.Windows.Forms.ImageList ilInfrared;
        private FutureConcepts.Media.CommonControls.FCClickButton B_Emitter;
        private System.Windows.Forms.ImageList ilEmitter;
        private UserPresetsControl ptzFavoritesControl;

    }
}
