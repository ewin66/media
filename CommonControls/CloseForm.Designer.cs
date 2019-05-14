using System.Windows.Forms;
using System.ComponentModel;
namespace FutureConcepts.Media.CommonControls
{
    partial class CloseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.B_OK = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel9.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel8);
            this.panel6.Controls.Add(this.panel7);
            this.panel6.Size = new System.Drawing.Size(316, 290);
            // 
            // BasePanel
            // 
            this.BasePanel.Size = new System.Drawing.Size(316, 332);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel9);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 252);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(316, 38);
            this.panel7.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.panel9.Controls.Add(this.B_OK);
            this.panel9.Location = new System.Drawing.Point(119, 7);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(78, 24);
            this.panel9.TabIndex = 1;
            // 
            // B_OK
            // 
            this.B_OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(140)))), ((int)(((byte)(79)))));
            this.B_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.B_OK.Dock = System.Windows.Forms.DockStyle.Left;
            this.B_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.B_OK.Location = new System.Drawing.Point(0, 0);
            this.B_OK.Name = "B_OK";
            this.B_OK.Size = new System.Drawing.Size(75, 24);
            this.B_OK.TabIndex = 0;
            this.B_OK.Text = "Close";
            this.B_OK.UseVisualStyleBackColor = false;
            this.B_OK.Click += new System.EventHandler(this.B_OK_Click);
            // 
            // panel8
            // 
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(316, 252);
            this.panel8.TabIndex = 1;
            // 
            // CloseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 336);
            this.Name = "CloseForm";
            this.Text = "CloseForm";
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel7;
        protected Panel panel8;
        private Panel panel9;
        private Button B_OK;
    }
}