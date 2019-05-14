namespace FutureConcepts.Media.CommonControls
{
    partial class FCCircularTrackbar
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
            this.SuspendLayout();
            // 
            // FCCircularTrackbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FCCircularTrackbar";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FCCircularTrackbar_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FCCircularTrackbar_MouseMove);
            this.PaddingChanged += new System.EventHandler(this.FCCircularTrackbar_Resize);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FCCircularTrackbar_MouseDown);
            this.Resize += new System.EventHandler(this.FCCircularTrackbar_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FCCircularTrackbar_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
