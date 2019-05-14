namespace FutureConcepts.Media.TV.Scanner
{
    partial class FavoriteChannelThumbnail
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FavoriteChannelThumbnail));
            this.gbChannelThumbnail = new System.Windows.Forms.GroupBox();
            this.pbSnapshot = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.btnDelete = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.ilRedX = new System.Windows.Forms.ImageList(this.components);
            this.panel_black = new System.Windows.Forms.Panel();
            this.gbChannelThumbnail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSnapshot)).BeginInit();
            this.panel_black.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbChannelThumbnail
            // 
            this.gbChannelThumbnail.BackColor = System.Drawing.Color.Black;
            this.gbChannelThumbnail.Controls.Add(this.pbSnapshot);
            this.gbChannelThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbChannelThumbnail.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbChannelThumbnail.ForeColor = System.Drawing.Color.White;
            this.gbChannelThumbnail.Location = new System.Drawing.Point(3, 0);
            this.gbChannelThumbnail.Margin = new System.Windows.Forms.Padding(0);
            this.gbChannelThumbnail.Name = "gbChannelThumbnail";
            this.gbChannelThumbnail.Size = new System.Drawing.Size(158, 133);
            this.gbChannelThumbnail.TabIndex = 0;
            this.gbChannelThumbnail.TabStop = false;
            this.gbChannelThumbnail.Text = "groupBox1";
            // 
            // pbSnapshot
            // 
            this.pbSnapshot.Location = new System.Drawing.Point(6, 19);
            this.pbSnapshot.Name = "pbSnapshot";
            this.pbSnapshot.Size = new System.Drawing.Size(149, 111);
            this.pbSnapshot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSnapshot.TabIndex = 0;
            this.pbSnapshot.TabStop = false;
            this.tt.SetToolTip(this.pbSnapshot, "Switch to this channel");
            this.pbSnapshot.Click += new System.EventHandler(this.pbSnapshot_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "set-up.gif");
            this.imageList1.Images.SetKeyName(1, "set-down.gif");
            // 
            // tt
            // 
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.IsBalloon = true;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Black;
            this.btnDelete.ImageIndex = 0;
            this.btnDelete.ImageList = this.ilRedX;
            this.btnDelete.Location = new System.Drawing.Point(147, 1);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(20, 20);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Toggle = false;
            this.tt.SetToolTip(this.btnDelete, "Delete this favorite channel");
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // ilRedX
            // 
            this.ilRedX.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRedX.ImageStream")));
            this.ilRedX.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRedX.Images.SetKeyName(0, "red-x.gif");
            this.ilRedX.Images.SetKeyName(1, "red-x.gif");
            // 
            // panel_black
            // 
            this.panel_black.BackColor = System.Drawing.Color.Black;
            this.panel_black.Controls.Add(this.gbChannelThumbnail);
            this.panel_black.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_black.Location = new System.Drawing.Point(2, 2);
            this.panel_black.Margin = new System.Windows.Forms.Padding(0);
            this.panel_black.Name = "panel_black";
            this.panel_black.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panel_black.Size = new System.Drawing.Size(164, 136);
            this.panel_black.TabIndex = 1;
            // 
            // FavoriteChannelThumbnail
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Yellow;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.panel_black);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FavoriteChannelThumbnail";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(168, 140);
            this.gbChannelThumbnail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSnapshot)).EndInit();
            this.panel_black.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbChannelThumbnail;
        private System.Windows.Forms.PictureBox pbSnapshot;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip tt;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnDelete;
        private System.Windows.Forms.ImageList ilRedX;
        private System.Windows.Forms.Panel panel_black;
    }
}
