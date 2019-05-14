namespace FutureConcepts.Media.CommonControls
{
    partial class FCMessageBox
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
            this.fcTextBox1 = new FutureConcepts.Media.CommonControls.FCTextBox();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.fcTextBox1);
            this.panel8.Size = new System.Drawing.Size(298, 234);
            // 
            // panel6
            // 
            this.panel8.Size = new System.Drawing.Size(298, 234);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(298, 314);
            // 
            // fcTextBox1
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.fcTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fcTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.fcTextBox1.Editable = false;
            this.fcTextBox1.ForeColor = System.Drawing.Color.White;
            this.fcTextBox1.Location = new System.Drawing.Point(0, 0);
            this.fcTextBox1.Multiline = true;
            this.fcTextBox1.Name = "fcTextBox1";
            this.fcTextBox1.ReadOnly = true;
            this.fcTextBox1.Size = new System.Drawing.Size(360, 28);
            this.fcTextBox1.TabIndex = 0;
            // 
            // FCMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 336);
            this.Name = "FCMessageBox";
            this.OKButtonText = "OK";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "";
            this.TopMost = true;
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private FutureConcepts.Media.CommonControls.FCTextBox fcTextBox1;



    }
}