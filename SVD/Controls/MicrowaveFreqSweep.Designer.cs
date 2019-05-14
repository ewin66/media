namespace FutureConcepts.Media.SVD.Controls
{
    partial class MicrowaveFreqSweep
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
            System.Windows.Forms.GroupBox gbContent;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.udThreshold = new System.Windows.Forms.NumericUpDown();
            this.pbProgress = new FutureConcepts.Media.CommonControls.GradientProgressBar();
            this.btnStartCancel = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.udStopFreq = new System.Windows.Forms.NumericUpDown();
            this.udStartFreq = new System.Windows.Forms.NumericUpDown();
            gbContent = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            gbContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStopFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStartFreq)).BeginInit();
            this.SuspendLayout();
            // 
            // gbContent
            // 
            gbContent.Controls.Add(label5);
            gbContent.Controls.Add(label6);
            gbContent.Controls.Add(this.udThreshold);
            gbContent.Controls.Add(this.pbProgress);
            gbContent.Controls.Add(label4);
            gbContent.Controls.Add(label3);
            gbContent.Controls.Add(label2);
            gbContent.Controls.Add(label1);
            gbContent.Controls.Add(this.btnStartCancel);
            gbContent.Controls.Add(this.udStopFreq);
            gbContent.Controls.Add(this.udStartFreq);
            gbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            gbContent.ForeColor = System.Drawing.Color.White;
            gbContent.Location = new System.Drawing.Point(0, 0);
            gbContent.Name = "gbContent";
            gbContent.Padding = new System.Windows.Forms.Padding(6, 3, 6, 6);
            gbContent.Size = new System.Drawing.Size(159, 166);
            gbContent.TabIndex = 0;
            gbContent.TabStop = false;
            gbContent.Text = "Frequency Sweep";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(121, 73);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(28, 13);
            label5.TabIndex = 12;
            label5.Text = "dBm";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label6.Location = new System.Drawing.Point(6, 73);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(54, 13);
            label6.TabIndex = 11;
            label6.Text = "Threshold";
            // 
            // udThreshold
            // 
            this.udThreshold.BackColor = System.Drawing.Color.Black;
            this.udThreshold.ForeColor = System.Drawing.Color.White;
            this.udThreshold.Location = new System.Drawing.Point(60, 71);
            this.udThreshold.Name = "udThreshold";
            this.udThreshold.Size = new System.Drawing.Size(55, 20);
            this.udThreshold.TabIndex = 10;
            this.udThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // pbProgress
            // 
            this.pbProgress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbProgress.DisplayValue = true;
            this.pbProgress.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(140)))), ((int)(((byte)(79)))));
            this.pbProgress.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pbProgress.LabelSuffix = "%";
            this.pbProgress.Location = new System.Drawing.Point(9, 131);
            this.pbProgress.Maximum = 100;
            this.pbProgress.MiddleColor = System.Drawing.Color.Turquoise;
            this.pbProgress.Minimum = 0;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.pbProgress.Size = new System.Drawing.Size(141, 26);
            this.pbProgress.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.pbProgress.TabIndex = 8;
            this.pbProgress.Value = 100;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(121, 47);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(29, 13);
            label4.TabIndex = 5;
            label4.Text = "MHz";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(121, 21);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(29, 13);
            label3.TabIndex = 4;
            label3.Text = "MHz";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.Location = new System.Drawing.Point(26, 46);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(28, 14);
            label2.TabIndex = 3;
            label2.Text = "End";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(20, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(34, 14);
            label1.TabIndex = 2;
            label1.Text = "Start";
            // 
            // btnStartCancel
            // 
            this.btnStartCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnStartCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartCancel.ForeColor = System.Drawing.Color.White;
            this.btnStartCancel.Location = new System.Drawing.Point(9, 99);
            this.btnStartCancel.Name = "btnStartCancel";
            this.btnStartCancel.Size = new System.Drawing.Size(141, 23);
            this.btnStartCancel.TabIndex = 6;
            this.btnStartCancel.Text = "Start";
            this.btnStartCancel.UseVisualStyleBackColor = false;
            this.btnStartCancel.Click += new System.EventHandler(this.btnStartCancel_Click);
            // 
            // udStopFreq
            // 
            this.udStopFreq.BackColor = System.Drawing.Color.Black;
            this.udStopFreq.ForeColor = System.Drawing.Color.White;
            this.udStopFreq.Location = new System.Drawing.Point(60, 45);
            this.udStopFreq.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.udStopFreq.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.udStopFreq.Name = "udStopFreq";
            this.udStopFreq.Size = new System.Drawing.Size(55, 20);
            this.udStopFreq.TabIndex = 1;
            this.udStopFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udStopFreq.Value = new decimal(new int[] {
            2700,
            0,
            0,
            0});
            this.udStopFreq.ValueChanged += new System.EventHandler(this.udStopFreq_ValueChanged);
            // 
            // udStartFreq
            // 
            this.udStartFreq.BackColor = System.Drawing.Color.Black;
            this.udStartFreq.ForeColor = System.Drawing.Color.White;
            this.udStartFreq.Location = new System.Drawing.Point(60, 19);
            this.udStartFreq.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.udStartFreq.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.udStartFreq.Name = "udStartFreq";
            this.udStartFreq.Size = new System.Drawing.Size(55, 20);
            this.udStartFreq.TabIndex = 0;
            this.udStartFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udStartFreq.Value = new decimal(new int[] {
            1700,
            0,
            0,
            0});
            this.udStartFreq.ValueChanged += new System.EventHandler(this.udStartFreq_ValueChanged);
            // 
            // MicrowaveFreqSweep
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(gbContent);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "MicrowaveFreqSweep";
            this.Size = new System.Drawing.Size(159, 166);
            gbContent.ResumeLayout(false);
            gbContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStopFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStartFreq)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FutureConcepts.Media.CommonControls.RedFlatButton btnStartCancel;
        private System.Windows.Forms.NumericUpDown udStopFreq;
        private System.Windows.Forms.NumericUpDown udStartFreq;
        private FutureConcepts.Media.CommonControls.GradientProgressBar pbProgress;
        private System.Windows.Forms.NumericUpDown udThreshold;
    }
}
