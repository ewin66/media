namespace FutureConcepts.Media.CommonControls
{
    partial class BaseDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseDialog));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.ilMaximize = new System.Windows.Forms.ImageList(this.components);
            this.ilMinimize = new System.Windows.Forms.ImageList(this.components);
            this.CloseImageList = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.ilRestore = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.CloseButton = new FutureConcepts.Media.CommonControls.FCClickButton();
            this.BasePanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // BasePanel
            // 
            this.BasePanel.Controls.Add(this.panel3);
            this.BasePanel.Controls.Add(this.panel2);
            this.BasePanel.Size = new System.Drawing.Size(541, 332);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(541, 42);
            this.panel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(541, 42);
            this.panel5.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(541, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ilMaximize
            // 
            this.ilMaximize.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMaximize.ImageStream")));
            this.ilMaximize.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMaximize.Images.SetKeyName(0, "maximize-up.gif");
            this.ilMaximize.Images.SetKeyName(1, "maximize-down.gif");
            // 
            // ilMinimize
            // 
            this.ilMinimize.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMinimize.ImageStream")));
            this.ilMinimize.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMinimize.Images.SetKeyName(0, "minimize-up.gif");
            this.ilMinimize.Images.SetKeyName(1, "minimize-down.gif");
            // 
            // CloseImageList
            // 
            this.CloseImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CloseImageList.ImageStream")));
            this.CloseImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.CloseImageList.Images.SetKeyName(0, "close-up.gif");
            this.CloseImageList.Images.SetKeyName(1, "close-down.gif");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 42);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(541, 290);
            this.panel3.TabIndex = 1;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.CloseButton);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(541, 290);
            this.panel6.TabIndex = 0;
            // 
            // ilRestore
            // 
            this.ilRestore.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRestore.ImageStream")));
            this.ilRestore.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRestore.Images.SetKeyName(0, "restore-up.gif");
            this.ilRestore.Images.SetKeyName(1, "restore-down.gif");
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1, 1);
            this.panel4.TabIndex = 1;
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(0, -40);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(1, 1);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Toggle = false;
            this.CloseButton.Visible = false;
            // 
            // BaseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 336);
            this.Controls.Add(this.panel4);
            this.Name = "BaseDialog";
            this.Text = "MainForm";
            this.Controls.SetChildIndex(this.panel4, 0);
            this.BasePanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList CloseImageList;
        protected System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ImageList ilMinimize;
        private System.Windows.Forms.ImageList ilMaximize;
        private System.Windows.Forms.ImageList ilRestore;
        private System.Windows.Forms.ToolTip tt;
        public System.Windows.Forms.Panel panel4;
        public FutureConcepts.Media.CommonControls.FCClickButton CloseButton;
    }
}