namespace FutureConcepts.Media.Client.Tester
{
    partial class ObjectDisplay
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
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.lblValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // properties
            // 
            this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.properties.HelpVisible = false;
            this.properties.Location = new System.Drawing.Point(0, 13);
            this.properties.Name = "properties";
            this.properties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.properties.Size = new System.Drawing.Size(382, 294);
            this.properties.TabIndex = 0;
            this.properties.ToolbarVisible = false;
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblValue.Location = new System.Drawing.Point(0, 0);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(35, 13);
            this.lblValue.TabIndex = 1;
            this.lblValue.Text = "label1";
            // 
            // ObjectDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 307);
            this.Controls.Add(this.properties);
            this.Controls.Add(this.lblValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ObjectDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ObjectDisplay";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid properties;
        private System.Windows.Forms.Label lblValue;
    }
}