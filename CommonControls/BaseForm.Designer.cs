using System.ComponentModel;
using System.Windows.Forms;
namespace FutureConcepts.Media.CommonControls
{
    partial class BaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this.PRed = new System.Windows.Forms.Panel();
            this.BasePanel = new System.Windows.Forms.Panel();
            this.PRed.SuspendLayout();
            this.SuspendLayout();
            // 
            // PRed
            // 
            this.PRed.BackColor = System.Drawing.Color.Transparent;
            this.PRed.Controls.Add(this.BasePanel);
            this.PRed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PRed.Location = new System.Drawing.Point(1, 1);
            this.PRed.Name = "PRed";
            this.PRed.Padding = new System.Windows.Forms.Padding(1);
            this.PRed.Size = new System.Drawing.Size(339, 334);
            this.PRed.TabIndex = 0;
            // 
            // BasePanel
            // 
            this.BasePanel.BackColor = System.Drawing.Color.Black;
            this.BasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BasePanel.Location = new System.Drawing.Point(1, 1);
            this.BasePanel.Name = "BasePanel";
            this.BasePanel.Size = new System.Drawing.Size(337, 332);
            this.BasePanel.TabIndex = 0;
            // 
            // BaseForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(341, 336);
            this.Controls.Add(this.PRed);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "BaseForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "OKCancelForm";
            this.PRed.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel PRed;
        protected Panel BasePanel;
    }
}