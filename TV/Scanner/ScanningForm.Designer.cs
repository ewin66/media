namespace FutureConcepts.Media.TV.Scanner
{
    partial class ScanningForm
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.pbMarquee = new System.Windows.Forms.ProgressBar();
            this.btnStop = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.lblPleaseWait = new System.Windows.Forms.Label();
            this.BasePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BasePanel
            // 
            this.BasePanel.Controls.Add(this.lblPleaseWait);
            this.BasePanel.Controls.Add(this.btnStop);
            this.BasePanel.Controls.Add(this.pbMarquee);
            this.BasePanel.Controls.Add(this.lblMessage);
            this.BasePanel.Margin = new System.Windows.Forms.Padding(6);
            this.BasePanel.MaximumSize = new System.Drawing.Size(348, 145);
            this.BasePanel.Padding = new System.Windows.Forms.Padding(6);
            this.BasePanel.Size = new System.Drawing.Size(348, 135);
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.Location = new System.Drawing.Point(9, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(330, 39);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "The server is currently scanning for available channels. You will be connected wh" +
                "en it has completed.";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbMarquee
            // 
            this.pbMarquee.ForeColor = System.Drawing.Color.Red;
            this.pbMarquee.Location = new System.Drawing.Point(12, 70);
            this.pbMarquee.Name = "pbMarquee";
            this.pbMarquee.Size = new System.Drawing.Size(327, 23);
            this.pbMarquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbMarquee.TabIndex = 1;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Red;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(127, 99);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Cancel";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblPleaseWait
            // 
            this.lblPleaseWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPleaseWait.Location = new System.Drawing.Point(9, 49);
            this.lblPleaseWait.Name = "lblPleaseWait";
            this.lblPleaseWait.Size = new System.Drawing.Size(330, 18);
            this.lblPleaseWait.TabIndex = 3;
            this.lblPleaseWait.Text = "Please Wait...";
            this.lblPleaseWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ScanningForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(352, 139);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScanningForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ScanningForm";
            this.BasePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ProgressBar pbMarquee;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnStop;
        private System.Windows.Forms.Label lblPleaseWait;
    }
}