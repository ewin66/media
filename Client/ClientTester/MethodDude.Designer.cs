namespace FutureConcepts.Media.Client.Tester
{
    partial class MethodDude
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
            this.components = new System.ComponentModel.Container();
            this.gbMethods = new System.Windows.Forms.GroupBox();
            this.pMethodButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.gbMethods.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMethods
            // 
            this.gbMethods.Controls.Add(this.pMethodButtons);
            this.gbMethods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbMethods.Location = new System.Drawing.Point(3, 3);
            this.gbMethods.Name = "gbMethods";
            this.gbMethods.Size = new System.Drawing.Size(566, 260);
            this.gbMethods.TabIndex = 0;
            this.gbMethods.TabStop = false;
            this.gbMethods.Text = "<TypeName>";
            // 
            // pMethodButtons
            // 
            this.pMethodButtons.AutoScroll = true;
            this.pMethodButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMethodButtons.Location = new System.Drawing.Point(3, 16);
            this.pMethodButtons.Name = "pMethodButtons";
            this.pMethodButtons.Size = new System.Drawing.Size(560, 241);
            this.pMethodButtons.TabIndex = 0;
            // 
            // tt
            // 
            this.tt.AutomaticDelay = 10;
            this.tt.AutoPopDelay = 5000;
            this.tt.InitialDelay = 10;
            this.tt.IsBalloon = true;
            this.tt.ReshowDelay = 2;
            // 
            // MethodDude
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(572, 266);
            this.Controls.Add(this.gbMethods);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MethodDude";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "MethodDude";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MethodDude_FormClosing);
            this.gbMethods.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMethods;
        private System.Windows.Forms.FlowLayoutPanel pMethodButtons;
        private System.Windows.Forms.ToolTip tt;
    }
}