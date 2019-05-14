namespace FutureConcepts.Media.SVD
{
    partial class InitialSettingsDialog
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
            this.label2 = new System.Windows.Forms.Label();
            this.TB_UserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label3);
            this.panel8.Controls.Add(this.TB_UserName);
            this.panel8.Controls.Add(this.label2);
            this.panel8.Size = new System.Drawing.Size(525, 109);
            // 
            // panel6
            // 
            this.panel6.Size = new System.Drawing.Size(525, 147);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(525, 189);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "User Name:";
            // 
            // TB_UserName
            // 
            this.TB_UserName.Location = new System.Drawing.Point(92, 56);
            this.TB_UserName.Name = "TB_UserName";
            this.TB_UserName.Size = new System.Drawing.Size(425, 23);
            this.TB_UserName.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(5, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(515, 34);
            this.label3.TabIndex = 1;
            this.label3.Text = "Please enter a username to identify your workstation. This name is shown to other" +
                " vehicles when you are watching remote video.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // InitialSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 193);
            this.Name = "InitialSettingsDialog";
            this.Text = "AntaresX Streaming Video Desktop";
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TB_UserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}