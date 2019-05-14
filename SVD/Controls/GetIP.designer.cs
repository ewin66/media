namespace FutureConcepts.Media.SVD.Controls
{
    partial class GetIP
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
            this.TB_IP = new System.Windows.Forms.TextBox();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.TB_IP);
            this.panel8.Controls.Add(this.label2);
            this.panel8.Size = new System.Drawing.Size(337, 44);
            // 
            // panel6
            // 
            this.panel6.Size = new System.Drawing.Size(337, 82);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(337, 124);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP or Hostname:";
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(115, 8);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(214, 23);
            this.TB_IP.TabIndex = 1;
            // 
            // GetIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 128);
            this.Name = "GetIP";
            this.ShowInTaskbar = false;
            this.Text = "Enter an IP or Hostname:";
            this.Load += new System.EventHandler(this.GetIP_Load);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_IP;
    }
}