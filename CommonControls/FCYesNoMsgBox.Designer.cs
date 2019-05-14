namespace FutureConcepts.Media.CommonControls
{
    partial class FCYesNoMsgBox
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.panel8.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.lblInfo);
            this.panel8.Size = new System.Drawing.Size(470, 138);
            // 
            // panel6
            // 
            this.panel6.Size = new System.Drawing.Size(470, 176);
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(432, 0);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(470, 218);
            // 
            // lblInfo
            // 
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Location = new System.Drawing.Point(5, 5);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(460, 128);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "label2";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FCYesNoMsgBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButtonDialogResult = System.Windows.Forms.DialogResult.No;
            this.CancelButtonText = "No";
            this.ClientSize = new System.Drawing.Size(474, 222);
            this.Name = "FCYesNoMsgBox";
            this.OKButtonDialogResult = System.Windows.Forms.DialogResult.Yes;
            this.OKButtonText = "Yes";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Message Box";
            this.panel8.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;

    }
}