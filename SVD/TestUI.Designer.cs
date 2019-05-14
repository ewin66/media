namespace FutureConcepts.Media.SVD
{
    partial class TestUI
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.gradientProgressBar1 = new FutureConcepts.Media.CommonControls.GradientProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(183, 12);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 183);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // gradientProgressBar1
            // 
            this.gradientProgressBar1.BackColor = System.Drawing.Color.Black;
            this.gradientProgressBar1.DisplayValue = true;
            this.gradientProgressBar1.EndColor = System.Drawing.Color.LimeGreen;
            this.gradientProgressBar1.ForeColor = System.Drawing.Color.White;
            this.gradientProgressBar1.LabelSuffix = "%";
            this.gradientProgressBar1.Location = new System.Drawing.Point(43, 12);
            this.gradientProgressBar1.Maximum = 100;
            this.gradientProgressBar1.MiddleColor = System.Drawing.Color.Yellow;
            this.gradientProgressBar1.Minimum = 0;
            this.gradientProgressBar1.Name = "gradientProgressBar1";
            this.gradientProgressBar1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.gradientProgressBar1.Size = new System.Drawing.Size(134, 183);
            this.gradientProgressBar1.StartColor = System.Drawing.Color.Red;
            this.gradientProgressBar1.TabIndex = 0;
            this.gradientProgressBar1.Value = 50;
            // 
            // TestUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 223);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.gradientProgressBar1);
            this.Name = "TestUI";
            this.Text = "TestUI";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FutureConcepts.Media.CommonControls.GradientProgressBar gradientProgressBar1;
        private System.Windows.Forms.TrackBar trackBar1;



    }
}