namespace FutureConcepts.Media.SVD.Controls
{
    partial class UserPresetsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserPresetsControl));
            this.ttImage = new System.Windows.Forms.ToolTip(this.components);
            this.gbContent = new System.Windows.Forms.GroupBox();
            this.btnRestore = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.btnDelete = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.btnRename = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.btnSave = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.btnClear = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.lv = new System.Windows.Forms.ListView();
            this.ilIconNoImageAvailable = new System.Windows.Forms.ImageList(this.components);
            this.ilToolTipNoImageAvailable = new System.Windows.Forms.ImageList(this.components);
            this.gbContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // ttImage
            // 
            this.ttImage.OwnerDraw = true;
            this.ttImage.Popup += new System.Windows.Forms.PopupEventHandler(this.ttImage_Popup);
            this.ttImage.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.ttImage_Draw);
            // 
            // gbContent
            // 
            this.gbContent.Controls.Add(this.btnRestore);
            this.gbContent.Controls.Add(this.btnDelete);
            this.gbContent.Controls.Add(this.btnRename);
            this.gbContent.Controls.Add(this.btnSave);
            this.gbContent.Controls.Add(this.btnClear);
            this.gbContent.Controls.Add(this.lv);
            this.gbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbContent.ForeColor = System.Drawing.Color.White;
            this.gbContent.Location = new System.Drawing.Point(0, 0);
            this.gbContent.Name = "gbContent";
            this.gbContent.Size = new System.Drawing.Size(331, 165);
            this.gbContent.TabIndex = 0;
            this.gbContent.TabStop = false;
            this.gbContent.Text = "User Presets Control";
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRestore.ForeColor = System.Drawing.Color.White;
            this.btnRestore.Location = new System.Drawing.Point(267, 43);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(61, 23);
            this.btnRestore.TabIndex = 6;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = false;
            this.btnRestore.Visible = false;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.Red;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(267, 110);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(61, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Remove";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Visible = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRename
            // 
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRename.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRename.ForeColor = System.Drawing.Color.White;
            this.btnRename.Location = new System.Drawing.Point(267, 72);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(61, 23);
            this.btnRename.TabIndex = 4;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = false;
            this.btnRename.Visible = false;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(267, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(61, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.Red;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(267, 139);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(61, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear All";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Visible = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lv
            // 
            this.lv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lv.BackColor = System.Drawing.Color.Black;
            this.lv.ForeColor = System.Drawing.Color.White;
            this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv.HideSelection = false;
            this.lv.LabelEdit = true;
            this.lv.LargeImageList = this.ilIconNoImageAvailable;
            this.lv.Location = new System.Drawing.Point(3, 14);
            this.lv.MultiSelect = false;
            this.lv.Name = "lv";
            this.lv.OwnerDraw = true;
            this.lv.ShowGroups = false;
            this.lv.Size = new System.Drawing.Size(261, 148);
            this.lv.TabIndex = 0;
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.VirtualMode = true;
            this.lv.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lv_DrawItem);
            this.lv.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lv_AfterLabelEdit);
            this.lv.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.lv_ItemMouseHover);
            this.lv.SelectedIndexChanged += new System.EventHandler(this.lv_SelectedIndexChanged);
            this.lv.DoubleClick += new System.EventHandler(this.lv_DoubleClick);
            this.lv.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lv_RetrieveVirtualItem);
            this.lv.MouseLeave += new System.EventHandler(this.lv_MouseLeave);
            // 
            // ilIconNoImageAvailable
            // 
            this.ilIconNoImageAvailable.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIconNoImageAvailable.ImageStream")));
            this.ilIconNoImageAvailable.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIconNoImageAvailable.Images.SetKeyName(0, "ptzFavorite_noimage-32x37-qmark-box.png");
            // 
            // ilToolTipNoImageAvailable
            // 
            this.ilToolTipNoImageAvailable.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilToolTipNoImageAvailable.ImageStream")));
            this.ilToolTipNoImageAvailable.TransparentColor = System.Drawing.Color.Transparent;
            this.ilToolTipNoImageAvailable.Images.SetKeyName(0, "ptzFavorite_noimage-133x100.png");
            // 
            // UserPresetsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.gbContent);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "UserPresetsControl";
            this.Size = new System.Drawing.Size(331, 165);
            this.gbContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ttImage;
        private System.Windows.Forms.GroupBox gbContent;
        private System.Windows.Forms.ListView lv;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnClear;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnSave;
        private System.Windows.Forms.ImageList ilIconNoImageAvailable;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnDelete;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnRename;
        private FutureConcepts.Media.CommonControls.RedFlatButton btnRestore;
        private System.Windows.Forms.ImageList ilToolTipNoImageAvailable;
    }
}
