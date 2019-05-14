namespace FutureConcepts.Media.Tools.CameraControlTester
{
    partial class ManualEntry
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
            this.udPan = new System.Windows.Forms.NumericUpDown();
            this.udTilt = new System.Windows.Forms.NumericUpDown();
            this.udZoom = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnYeah = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            ((System.ComponentModel.ISupportInitialize)(this.udPan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udTilt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // udPan
            // 
            this.udPan.DecimalPlaces = 4;
            this.udPan.Location = new System.Drawing.Point(52, 12);
            this.udPan.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.udPan.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.udPan.Name = "udPan";
            this.udPan.Size = new System.Drawing.Size(120, 20);
            this.udPan.TabIndex = 0;
            // 
            // udTilt
            // 
            this.udTilt.DecimalPlaces = 4;
            this.udTilt.Location = new System.Drawing.Point(52, 38);
            this.udTilt.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.udTilt.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.udTilt.Name = "udTilt";
            this.udTilt.Size = new System.Drawing.Size(120, 20);
            this.udTilt.TabIndex = 1;
            // 
            // udZoom
            // 
            this.udZoom.DecimalPlaces = 4;
            this.udZoom.Location = new System.Drawing.Point(52, 64);
            this.udZoom.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udZoom.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.udZoom.Name = "udZoom";
            this.udZoom.Size = new System.Drawing.Size(120, 20);
            this.udZoom.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Pan";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tilt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Zoom";
            // 
            // btnYeah
            // 
            this.btnYeah.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnYeah.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYeah.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYeah.ForeColor = System.Drawing.Color.White;
            this.btnYeah.Location = new System.Drawing.Point(191, 12);
            this.btnYeah.Name = "btnYeah";
            this.btnYeah.Size = new System.Drawing.Size(57, 72);
            this.btnYeah.TabIndex = 6;
            this.btnYeah.Text = "YEAH";
            this.btnYeah.UseVisualStyleBackColor = false;
            this.btnYeah.Click += new System.EventHandler(this.btnYeah_Click);
            // 
            // ManualEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(262, 96);
            this.Controls.Add(this.btnYeah);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udZoom);
            this.Controls.Add(this.udTilt);
            this.Controls.Add(this.udPan);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ManualEntry";
            this.Text = "Manual Entry";
            ((System.ComponentModel.ISupportInitialize)(this.udPan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udTilt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown udPan;
        private System.Windows.Forms.NumericUpDown udTilt;
        private System.Windows.Forms.NumericUpDown udZoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnYeah;
    }
}