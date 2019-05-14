namespace FutureConcepts.Media.SVD.Controls
{
    partial class MicrowaveTuner
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
            System.Windows.Forms.GroupBox gbContent;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MicrowaveTuner));
            this.pMain = new System.Windows.Forms.TableLayoutPanel();
            this.udFrequency = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.pbSNR = new FutureConcepts.Media.CommonControls.GradientProgressBar();
            this.lblSNR = new System.Windows.Forms.Label();
            this.pbRSSI = new FutureConcepts.Media.CommonControls.GradientProgressBar();
            this.lblRSSI = new System.Windows.Forms.Label();
            this.pLEDs = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEnterKey = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.led_tuner = new FutureConcepts.Media.CommonControls.LED();
            this.led_demod = new FutureConcepts.Media.CommonControls.LED();
            this.led_mpeg = new FutureConcepts.Media.CommonControls.LED();
            this.led_fec = new FutureConcepts.Media.CommonControls.LED();
            this.led_decoder = new FutureConcepts.Media.CommonControls.LED();
            this.ico_EncryptionActive = new System.Windows.Forms.PictureBox();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            gbContent = new System.Windows.Forms.GroupBox();
            gbContent.SuspendLayout();
            this.pMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udFrequency)).BeginInit();
            this.pLEDs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ico_EncryptionActive)).BeginInit();
            this.SuspendLayout();
            // 
            // gbContent
            // 
            gbContent.Controls.Add(this.pMain);
            gbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            gbContent.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gbContent.ForeColor = System.Drawing.Color.White;
            gbContent.Location = new System.Drawing.Point(0, 0);
            gbContent.Name = "gbContent";
            gbContent.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            gbContent.Size = new System.Drawing.Size(184, 166);
            gbContent.TabIndex = 0;
            gbContent.TabStop = false;
            gbContent.Text = "Microwave Receiver";
            // 
            // pMain
            // 
            this.pMain.ColumnCount = 2;
            this.pMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 143F));
            this.pMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pMain.Controls.Add(this.udFrequency, 0, 1);
            this.pMain.Controls.Add(this.label1, 0, 0);
            this.pMain.Controls.Add(this.pbSNR, 0, 3);
            this.pMain.Controls.Add(this.lblSNR, 0, 2);
            this.pMain.Controls.Add(this.pbRSSI, 0, 4);
            this.pMain.Controls.Add(this.lblRSSI, 0, 5);
            this.pMain.Controls.Add(this.pLEDs, 1, 2);
            this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMain.Location = new System.Drawing.Point(3, 14);
            this.pMain.Margin = new System.Windows.Forms.Padding(0);
            this.pMain.Name = "pMain";
            this.pMain.RowCount = 6;
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pMain.Size = new System.Drawing.Size(178, 149);
            this.pMain.TabIndex = 14;
            // 
            // udFrequency
            // 
            this.udFrequency.BackColor = System.Drawing.Color.Black;
            this.pMain.SetColumnSpan(this.udFrequency, 2);
            this.udFrequency.DecimalPlaces = 1;
            this.udFrequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.udFrequency.ForeColor = System.Drawing.Color.Yellow;
            this.udFrequency.Location = new System.Drawing.Point(3, 17);
            this.udFrequency.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.udFrequency.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.udFrequency.Name = "udFrequency";
            this.udFrequency.Size = new System.Drawing.Size(172, 62);
            this.udFrequency.TabIndex = 4;
            this.udFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udFrequency.Value = new decimal(new int[] {
            99985,
            0,
            0,
            65536});
            this.udFrequency.ValueChanged += new System.EventHandler(this.udFrequency_ValueChanged);
            this.udFrequency.Enter += new System.EventHandler(this.udFrequency_Enter);
            this.udFrequency.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.udFrequency_KeyPress);
            this.udFrequency.Leave += new System.EventHandler(this.udFrequency_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.pMain.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Frequency (MHz)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pbSNR
            // 
            this.pbSNR.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbSNR.DisplayValue = true;
            this.pbSNR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSNR.EndColor = System.Drawing.Color.Lime;
            this.pbSNR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbSNR.ForeColor = System.Drawing.Color.White;
            this.pbSNR.LabelSuffix = " dB";
            this.pbSNR.Location = new System.Drawing.Point(3, 99);
            this.pbSNR.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.pbSNR.Maximum = 80;
            this.pbSNR.MiddleColor = System.Drawing.Color.Yellow;
            this.pbSNR.Minimum = 0;
            this.pbSNR.Name = "pbSNR";
            this.pbSNR.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.pbSNR.Size = new System.Drawing.Size(137, 16);
            this.pbSNR.StartColor = System.Drawing.Color.Red;
            this.pbSNR.TabIndex = 7;
            this.pbSNR.Value = 40;
            // 
            // lblSNR
            // 
            this.lblSNR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSNR.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSNR.Location = new System.Drawing.Point(0, 82);
            this.lblSNR.Margin = new System.Windows.Forms.Padding(0);
            this.lblSNR.Name = "lblSNR";
            this.lblSNR.Size = new System.Drawing.Size(143, 14);
            this.lblSNR.TabIndex = 6;
            this.lblSNR.Text = "Signal to Noise Ratio";
            this.lblSNR.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pbRSSI
            // 
            this.pbRSSI.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbRSSI.DisplayValue = true;
            this.pbRSSI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbRSSI.EndColor = System.Drawing.Color.Lime;
            this.pbRSSI.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbRSSI.LabelSuffix = " dBm";
            this.pbRSSI.Location = new System.Drawing.Point(3, 115);
            this.pbRSSI.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.pbRSSI.Maximum = 150;
            this.pbRSSI.MiddleColor = System.Drawing.Color.Yellow;
            this.pbRSSI.Minimum = 0;
            this.pbRSSI.Name = "pbRSSI";
            this.pbRSSI.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.pbRSSI.Size = new System.Drawing.Size(137, 16);
            this.pbRSSI.StartColor = System.Drawing.Color.Red;
            this.pbRSSI.TabIndex = 8;
            this.pbRSSI.Value = 150;
            // 
            // lblRSSI
            // 
            this.lblRSSI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRSSI.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRSSI.Location = new System.Drawing.Point(3, 134);
            this.lblRSSI.Name = "lblRSSI";
            this.lblRSSI.Size = new System.Drawing.Size(137, 18);
            this.lblRSSI.TabIndex = 9;
            this.lblRSSI.Text = "Received Carrier Level";
            this.lblRSSI.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pLEDs
            // 
            this.pLEDs.AutoSize = true;
            this.pLEDs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pLEDs.Controls.Add(this.btnEnterKey);
            this.pLEDs.Controls.Add(this.led_tuner);
            this.pLEDs.Controls.Add(this.led_demod);
            this.pLEDs.Controls.Add(this.led_mpeg);
            this.pLEDs.Controls.Add(this.led_fec);
            this.pLEDs.Controls.Add(this.led_decoder);
            this.pLEDs.Controls.Add(this.ico_EncryptionActive);
            this.pLEDs.Location = new System.Drawing.Point(143, 82);
            this.pLEDs.Margin = new System.Windows.Forms.Padding(0);
            this.pLEDs.Name = "pLEDs";
            this.pMain.SetRowSpan(this.pLEDs, 4);
            this.pLEDs.Size = new System.Drawing.Size(32, 66);
            this.pLEDs.TabIndex = 15;
            // 
            // btnEnterKey
            // 
            this.btnEnterKey.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnEnterKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnterKey.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnterKey.ForeColor = System.Drawing.Color.White;
            this.btnEnterKey.Image = ((System.Drawing.Image)(resources.GetObject("btnEnterKey.Image")));
            this.btnEnterKey.Location = new System.Drawing.Point(0, 0);
            this.btnEnterKey.Margin = new System.Windows.Forms.Padding(0);
            this.btnEnterKey.Name = "btnEnterKey";
            this.btnEnterKey.Padding = new System.Windows.Forms.Padding(1);
            this.btnEnterKey.Size = new System.Drawing.Size(32, 18);
            this.btnEnterKey.TabIndex = 16;
            this.btnEnterKey.Text = " ";
            this.tt.SetToolTip(this.btnEnterKey, "Encryption is disabled. Click to change.");
            this.btnEnterKey.UseVisualStyleBackColor = false;
            this.btnEnterKey.Click += new System.EventHandler(this.btnEnterKey_Click);
            // 
            // led_tuner
            // 
            this.led_tuner.BackColor = System.Drawing.Color.Black;
            this.led_tuner.ForeColor = System.Drawing.Color.LimeGreen;
            this.led_tuner.LEDColor = System.Drawing.Color.Lime;
            this.led_tuner.Location = new System.Drawing.Point(2, 20);
            this.led_tuner.Margin = new System.Windows.Forms.Padding(2);
            this.led_tuner.Name = "led_tuner";
            this.led_tuner.Size = new System.Drawing.Size(12, 12);
            this.led_tuner.TabIndex = 12;
            this.tt.SetToolTip(this.led_tuner, "Tuner is locked.");
            // 
            // led_demod
            // 
            this.led_demod.BackColor = System.Drawing.Color.Black;
            this.led_demod.ForeColor = System.Drawing.Color.LimeGreen;
            this.led_demod.LEDColor = System.Drawing.Color.Lime;
            this.led_demod.Location = new System.Drawing.Point(18, 20);
            this.led_demod.Margin = new System.Windows.Forms.Padding(2);
            this.led_demod.Name = "led_demod";
            this.led_demod.Size = new System.Drawing.Size(12, 12);
            this.led_demod.TabIndex = 11;
            this.tt.SetToolTip(this.led_demod, "Demodulator is locked.");
            // 
            // led_mpeg
            // 
            this.led_mpeg.BackColor = System.Drawing.Color.Black;
            this.led_mpeg.ForeColor = System.Drawing.Color.LimeGreen;
            this.led_mpeg.LEDColor = System.Drawing.Color.Lime;
            this.led_mpeg.Location = new System.Drawing.Point(2, 36);
            this.led_mpeg.Margin = new System.Windows.Forms.Padding(2);
            this.led_mpeg.Name = "led_mpeg";
            this.led_mpeg.Size = new System.Drawing.Size(12, 12);
            this.led_mpeg.TabIndex = 10;
            this.tt.SetToolTip(this.led_mpeg, "Transport Stream is locked.");
            // 
            // led_fec
            // 
            this.led_fec.BackColor = System.Drawing.Color.Black;
            this.led_fec.ForeColor = System.Drawing.Color.LimeGreen;
            this.led_fec.LEDColor = System.Drawing.Color.Lime;
            this.led_fec.Location = new System.Drawing.Point(18, 36);
            this.led_fec.Margin = new System.Windows.Forms.Padding(2);
            this.led_fec.Name = "led_fec";
            this.led_fec.Size = new System.Drawing.Size(12, 12);
            this.led_fec.TabIndex = 9;
            this.tt.SetToolTip(this.led_fec, "Error Correction is locked.");
            // 
            // led_decoder
            // 
            this.led_decoder.BackColor = System.Drawing.Color.Black;
            this.led_decoder.ForeColor = System.Drawing.Color.LimeGreen;
            this.led_decoder.LEDColor = System.Drawing.Color.Lime;
            this.led_decoder.Location = new System.Drawing.Point(2, 52);
            this.led_decoder.Margin = new System.Windows.Forms.Padding(2);
            this.led_decoder.Name = "led_decoder";
            this.led_decoder.Size = new System.Drawing.Size(12, 12);
            this.led_decoder.TabIndex = 8;
            this.tt.SetToolTip(this.led_decoder, "Video Decoder is locked.");
            // 
            // ico_EncryptionActive
            // 
            this.ico_EncryptionActive.Image = ((System.Drawing.Image)(resources.GetObject("ico_EncryptionActive.Image")));
            this.ico_EncryptionActive.Location = new System.Drawing.Point(16, 50);
            this.ico_EncryptionActive.Margin = new System.Windows.Forms.Padding(0);
            this.ico_EncryptionActive.Name = "ico_EncryptionActive";
            this.ico_EncryptionActive.Size = new System.Drawing.Size(16, 16);
            this.ico_EncryptionActive.TabIndex = 13;
            this.ico_EncryptionActive.TabStop = false;
            this.tt.SetToolTip(this.ico_EncryptionActive, "Encryption Active");
            // 
            // MicrowaveTuner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(gbContent);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(184, 166);
            this.Name = "MicrowaveTuner";
            this.Size = new System.Drawing.Size(184, 166);
            gbContent.ResumeLayout(false);
            this.pMain.ResumeLayout(false);
            this.pMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udFrequency)).EndInit();
            this.pLEDs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ico_EncryptionActive)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown udFrequency;
        private System.Windows.Forms.Label lblSNR;
        private FutureConcepts.Media.CommonControls.GradientProgressBar pbSNR;
        private FutureConcepts.Media.CommonControls.LED led_decoder;
        private FutureConcepts.Media.CommonControls.LED led_tuner;
        private FutureConcepts.Media.CommonControls.LED led_demod;
        private FutureConcepts.Media.CommonControls.LED led_mpeg;
        private FutureConcepts.Media.CommonControls.LED led_fec;
        private System.Windows.Forms.TableLayoutPanel pMain;
        private System.Windows.Forms.FlowLayoutPanel pLEDs;
        private System.Windows.Forms.PictureBox ico_EncryptionActive;
        private System.Windows.Forms.Label lblRSSI;
        private FutureConcepts.Media.CommonControls.GradientProgressBar pbRSSI;
        private System.Windows.Forms.ToolTip tt;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnEnterKey;
    }
}
