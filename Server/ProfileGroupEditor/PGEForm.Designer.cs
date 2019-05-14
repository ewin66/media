namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    partial class PGEForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PGEForm));
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.mFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mEditDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.pBrowser = new System.Windows.Forms.Panel();
            this.pBrowserButtons = new System.Windows.Forms.Panel();
            this.btnMoveDown = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnMoveUp = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnAddProfile = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.btnRemoveProfile = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.tvGroup = new System.Windows.Forms.TreeView();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.redFlatButton1 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.redFlatButton2 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.mTVContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mTVContextAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mTVContextDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mTVContextSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mTVContextMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.mTVContextMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.mTVContextSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mTVContextCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mTVContextPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mTVContextDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.groupEditor = new FutureConcepts.Media.Server.ProfileGroupEditor.ProfileGroupPropertiesEditor();
            this.profileEditor = new FutureConcepts.Media.Server.ProfileGroupEditor.CustomProfileEditor();
            this.menuBar.SuspendLayout();
            this.pBrowser.SuspendLayout();
            this.pBrowserButtons.SuspendLayout();
            this.mTVContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBar
            // 
            this.menuBar.BackColor = System.Drawing.SystemColors.Control;
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileMenu,
            this.mEditMenu});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuBar.Size = new System.Drawing.Size(649, 24);
            this.menuBar.TabIndex = 0;
            // 
            // mFileMenu
            // 
            this.mFileMenu.BackColor = System.Drawing.SystemColors.Control;
            this.mFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFileNew,
            this.toolStripMenuItem1,
            this.mFileOpen,
            this.mFileSave,
            this.mFileSaveAs,
            this.toolStripMenuItem2,
            this.mFileExit});
            this.mFileMenu.ForeColor = System.Drawing.Color.Black;
            this.mFileMenu.Name = "mFileMenu";
            this.mFileMenu.Size = new System.Drawing.Size(35, 20);
            this.mFileMenu.Text = "&File";
            // 
            // mFileNew
            // 
            this.mFileNew.BackColor = System.Drawing.SystemColors.Control;
            this.mFileNew.Name = "mFileNew";
            this.mFileNew.ShortcutKeyDisplayString = "";
            this.mFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mFileNew.Size = new System.Drawing.Size(204, 22);
            this.mFileNew.Text = "&New...";
            this.mFileNew.Click += new System.EventHandler(this.mFileNew_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            // 
            // mFileOpen
            // 
            this.mFileOpen.Name = "mFileOpen";
            this.mFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mFileOpen.Size = new System.Drawing.Size(204, 22);
            this.mFileOpen.Text = "&Open...";
            this.mFileOpen.Click += new System.EventHandler(this.mFileOpen_Click);
            // 
            // mFileSave
            // 
            this.mFileSave.Name = "mFileSave";
            this.mFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mFileSave.Size = new System.Drawing.Size(204, 22);
            this.mFileSave.Text = "&Save...";
            this.mFileSave.Click += new System.EventHandler(this.mFileSave_Click);
            // 
            // mFileSaveAs
            // 
            this.mFileSaveAs.Name = "mFileSaveAs";
            this.mFileSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.mFileSaveAs.Size = new System.Drawing.Size(204, 22);
            this.mFileSaveAs.Text = "Save &As...";
            this.mFileSaveAs.Click += new System.EventHandler(this.mFileSaveAs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(201, 6);
            // 
            // mFileExit
            // 
            this.mFileExit.Name = "mFileExit";
            this.mFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.mFileExit.Size = new System.Drawing.Size(204, 22);
            this.mFileExit.Text = "E&xit";
            this.mFileExit.Click += new System.EventHandler(this.mFileExit_Click);
            // 
            // mEditMenu
            // 
            this.mEditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mEditCopy,
            this.mEditPaste,
            this.toolStripMenuItem3,
            this.mEditDuplicate});
            this.mEditMenu.ForeColor = System.Drawing.Color.Black;
            this.mEditMenu.Name = "mEditMenu";
            this.mEditMenu.Size = new System.Drawing.Size(37, 20);
            this.mEditMenu.Text = "&Edit";
            // 
            // mEditCopy
            // 
            this.mEditCopy.Name = "mEditCopy";
            this.mEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mEditCopy.Size = new System.Drawing.Size(168, 22);
            this.mEditCopy.Text = "&Copy";
            this.mEditCopy.Click += new System.EventHandler(this.mEditCopy_Click);
            // 
            // mEditPaste
            // 
            this.mEditPaste.Name = "mEditPaste";
            this.mEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mEditPaste.Size = new System.Drawing.Size(168, 22);
            this.mEditPaste.Text = "&Paste";
            this.mEditPaste.Click += new System.EventHandler(this.mEditPaste_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(165, 6);
            // 
            // mEditDuplicate
            // 
            this.mEditDuplicate.Name = "mEditDuplicate";
            this.mEditDuplicate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.mEditDuplicate.Size = new System.Drawing.Size(168, 22);
            this.mEditDuplicate.Text = "&Duplicate";
            this.mEditDuplicate.Click += new System.EventHandler(this.mEditDuplicate_Click);
            // 
            // pBrowser
            // 
            this.pBrowser.Controls.Add(this.pBrowserButtons);
            this.pBrowser.Controls.Add(this.tvGroup);
            this.pBrowser.Dock = System.Windows.Forms.DockStyle.Left;
            this.pBrowser.Location = new System.Drawing.Point(0, 24);
            this.pBrowser.Name = "pBrowser";
            this.pBrowser.Size = new System.Drawing.Size(205, 313);
            this.pBrowser.TabIndex = 1;
            // 
            // pBrowserButtons
            // 
            this.pBrowserButtons.Controls.Add(this.btnMoveDown);
            this.pBrowserButtons.Controls.Add(this.btnMoveUp);
            this.pBrowserButtons.Controls.Add(this.btnAddProfile);
            this.pBrowserButtons.Controls.Add(this.btnRemoveProfile);
            this.pBrowserButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pBrowserButtons.Location = new System.Drawing.Point(0, 289);
            this.pBrowserButtons.Name = "pBrowserButtons";
            this.pBrowserButtons.Size = new System.Drawing.Size(205, 24);
            this.pBrowserButtons.TabIndex = 1;
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDown.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveDown.ForeColor = System.Drawing.Color.White;
            this.btnMoveDown.Location = new System.Drawing.Point(155, 0);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(50, 23);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.Text = "Down";
            this.toolTip.SetToolTip(this.btnMoveDown, "Moves the currently selected Profile down.");
            this.btnMoveDown.UseVisualStyleBackColor = false;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUp.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveUp.ForeColor = System.Drawing.Color.White;
            this.btnMoveUp.Location = new System.Drawing.Point(105, 0);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(50, 23);
            this.btnMoveUp.TabIndex = 2;
            this.btnMoveUp.Text = "Up";
            this.toolTip.SetToolTip(this.btnMoveUp, "Moves the currently selected Profile up.");
            this.btnMoveUp.UseVisualStyleBackColor = false;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnAddProfile
            // 
            this.btnAddProfile.BackColor = System.Drawing.Color.Green;
            this.btnAddProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddProfile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddProfile.ForeColor = System.Drawing.Color.White;
            this.btnAddProfile.Location = new System.Drawing.Point(0, 0);
            this.btnAddProfile.Name = "btnAddProfile";
            this.btnAddProfile.Size = new System.Drawing.Size(50, 23);
            this.btnAddProfile.TabIndex = 1;
            this.btnAddProfile.Text = "Add";
            this.toolTip.SetToolTip(this.btnAddProfile, "Add a new Profile to the Profile Group.");
            this.btnAddProfile.UseVisualStyleBackColor = false;
            this.btnAddProfile.Click += new System.EventHandler(this.btnAddProfile_Click);
            // 
            // btnRemoveProfile
            // 
            this.btnRemoveProfile.BackColor = System.Drawing.Color.Red;
            this.btnRemoveProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveProfile.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveProfile.ForeColor = System.Drawing.Color.White;
            this.btnRemoveProfile.Location = new System.Drawing.Point(50, 0);
            this.btnRemoveProfile.Name = "btnRemoveProfile";
            this.btnRemoveProfile.Size = new System.Drawing.Size(50, 23);
            this.btnRemoveProfile.TabIndex = 0;
            this.btnRemoveProfile.Text = "Delete";
            this.toolTip.SetToolTip(this.btnRemoveProfile, "Deletes the currently selected Profile.");
            this.btnRemoveProfile.UseVisualStyleBackColor = true;
            this.btnRemoveProfile.Click += new System.EventHandler(this.btnRemoveProfile_Click);
            // 
            // tvGroup
            // 
            this.tvGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvGroup.BackColor = System.Drawing.Color.Black;
            this.tvGroup.ForeColor = System.Drawing.Color.White;
            this.tvGroup.HideSelection = false;
            this.tvGroup.Location = new System.Drawing.Point(0, 0);
            this.tvGroup.Name = "tvGroup";
            this.tvGroup.Size = new System.Drawing.Size(205, 287);
            this.tvGroup.TabIndex = 0;
            this.toolTip.SetToolTip(this.tvGroup, "Select a Profile or Group to edit.");
            this.tvGroup.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvGroup_AfterSelect);
            this.tvGroup.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvGroup_NodeMouseClick);
            this.tvGroup.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvGroup_BeforeSelect);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Profile Groups (*.xml)|*.xml|All Files|*.*";
            this.openFileDialog.Title = "Open Profile Group...";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xml";
            this.saveFileDialog.Filter = "Profile Groups (*.xml)|*.xml";
            this.saveFileDialog.Title = "Save Profile Group As...";
            // 
            // redFlatButton1
            // 
            this.redFlatButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.redFlatButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.redFlatButton1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redFlatButton1.ForeColor = System.Drawing.Color.White;
            this.redFlatButton1.Location = new System.Drawing.Point(105, 0);
            this.redFlatButton1.Name = "redFlatButton1";
            this.redFlatButton1.Size = new System.Drawing.Size(50, 23);
            this.redFlatButton1.TabIndex = 2;
            this.redFlatButton1.Text = "Up";
            this.redFlatButton1.UseVisualStyleBackColor = false;
            // 
            // redFlatButton2
            // 
            this.redFlatButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(90)))), ((int)(((byte)(160)))));
            this.redFlatButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.redFlatButton2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.redFlatButton2.ForeColor = System.Drawing.Color.White;
            this.redFlatButton2.Location = new System.Drawing.Point(155, 0);
            this.redFlatButton2.Name = "redFlatButton2";
            this.redFlatButton2.Size = new System.Drawing.Size(50, 23);
            this.redFlatButton2.TabIndex = 3;
            this.redFlatButton2.Text = "Down";
            this.redFlatButton2.UseVisualStyleBackColor = false;
            // 
            // mTVContext
            // 
            this.mTVContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mTVContextAdd,
            this.mTVContextDelete,
            this.mTVContextSep1,
            this.mTVContextMoveUp,
            this.mTVContextMoveDown,
            this.mTVContextSep2,
            this.mTVContextCopy,
            this.mTVContextPaste,
            this.toolStripMenuItem4,
            this.mTVContextDuplicate});
            this.mTVContext.Name = "mTVContextMenu";
            this.mTVContext.Size = new System.Drawing.Size(142, 176);
            // 
            // mTVContextAdd
            // 
            this.mTVContextAdd.Name = "mTVContextAdd";
            this.mTVContextAdd.Size = new System.Drawing.Size(141, 22);
            this.mTVContextAdd.Text = "&Add";
            this.mTVContextAdd.Click += new System.EventHandler(this.btnAddProfile_Click);
            // 
            // mTVContextDelete
            // 
            this.mTVContextDelete.Name = "mTVContextDelete";
            this.mTVContextDelete.Size = new System.Drawing.Size(141, 22);
            this.mTVContextDelete.Text = "&Delete";
            this.mTVContextDelete.Click += new System.EventHandler(this.mTVContextDelete_Click);
            // 
            // mTVContextSep1
            // 
            this.mTVContextSep1.Name = "mTVContextSep1";
            this.mTVContextSep1.Size = new System.Drawing.Size(138, 6);
            // 
            // mTVContextMoveUp
            // 
            this.mTVContextMoveUp.Name = "mTVContextMoveUp";
            this.mTVContextMoveUp.Size = new System.Drawing.Size(141, 22);
            this.mTVContextMoveUp.Text = "Move &Up";
            this.mTVContextMoveUp.Click += new System.EventHandler(this.mTVContextMoveUp_Click);
            // 
            // mTVContextMoveDown
            // 
            this.mTVContextMoveDown.Name = "mTVContextMoveDown";
            this.mTVContextMoveDown.Size = new System.Drawing.Size(141, 22);
            this.mTVContextMoveDown.Text = "&Move Down";
            this.mTVContextMoveDown.Click += new System.EventHandler(this.mTVContextMoveDown_Click);
            // 
            // mTVContextSep2
            // 
            this.mTVContextSep2.Name = "mTVContextSep2";
            this.mTVContextSep2.Size = new System.Drawing.Size(138, 6);
            // 
            // mTVContextCopy
            // 
            this.mTVContextCopy.Name = "mTVContextCopy";
            this.mTVContextCopy.Size = new System.Drawing.Size(141, 22);
            this.mTVContextCopy.Text = "&Copy";
            this.mTVContextCopy.Click += new System.EventHandler(this.mTVContextCopy_Click);
            // 
            // mTVContextPaste
            // 
            this.mTVContextPaste.Name = "mTVContextPaste";
            this.mTVContextPaste.Size = new System.Drawing.Size(141, 22);
            this.mTVContextPaste.Text = "&Paste";
            this.mTVContextPaste.Click += new System.EventHandler(this.mTVContextPaste_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(138, 6);
            // 
            // mTVContextDuplicate
            // 
            this.mTVContextDuplicate.Name = "mTVContextDuplicate";
            this.mTVContextDuplicate.Size = new System.Drawing.Size(141, 22);
            this.mTVContextDuplicate.Text = "Duplica&te";
            this.mTVContextDuplicate.Click += new System.EventHandler(this.mTVContextDuplicate_Click);
            // 
            // groupEditor
            // 
            this.groupEditor.BackColor = System.Drawing.Color.Black;
            this.groupEditor.ForeColor = System.Drawing.Color.White;
            this.groupEditor.Location = new System.Drawing.Point(211, 45);
            this.groupEditor.Name = "groupEditor";
            this.groupEditor.ProfileGroup = null;
            this.groupEditor.Size = new System.Drawing.Size(130, 122);
            this.groupEditor.TabIndex = 2;
            this.groupEditor.ProfileGroupNameChanged += new System.EventHandler(this.groupEditor_ProfileGroupNameChanged);
            // 
            // profileEditor
            // 
            this.profileEditor.BackColor = System.Drawing.Color.White;
            this.profileEditor.Location = new System.Drawing.Point(364, 23);
            this.profileEditor.Margin = new System.Windows.Forms.Padding(0);
            this.profileEditor.Name = "profileEditor";
            this.profileEditor.Size = new System.Drawing.Size(444, 283);
            this.profileEditor.TabIndex = 2;
            this.profileEditor.ProfileNameChanged += new System.EventHandler(this.profileEditor_ProfileNameChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(649, 337);
            this.Controls.Add(this.groupEditor);
            this.Controls.Add(this.profileEditor);
            this.Controls.Add(this.pBrowser);
            this.Controls.Add(this.menuBar);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuBar;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.pBrowser.ResumeLayout(false);
            this.pBrowserButtons.ResumeLayout(false);
            this.mTVContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem mFileMenu;
        private System.Windows.Forms.ToolStripMenuItem mFileNew;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mFileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mFileExit;
        private System.Windows.Forms.Panel pBrowser;
        private System.Windows.Forms.Panel pBrowserButtons;
        private System.Windows.Forms.TreeView tvGroup;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnRemoveProfile;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnAddProfile;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnMoveDown;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnMoveUp;
        private FutureConcepts.Media.Server.ProfileGroupEditor.CustomProfileEditor profileEditor;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton redFlatButton1;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton redFlatButton2;
        private System.Windows.Forms.ToolStripMenuItem mFileSaveAs;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private FutureConcepts.Media.Server.ProfileGroupEditor.ProfileGroupPropertiesEditor groupEditor;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem mEditMenu;
        private System.Windows.Forms.ToolStripMenuItem mEditCopy;
        private System.Windows.Forms.ToolStripMenuItem mEditPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mEditDuplicate;
        private System.Windows.Forms.ContextMenuStrip mTVContext;
        private System.Windows.Forms.ToolStripMenuItem mTVContextAdd;
        private System.Windows.Forms.ToolStripMenuItem mTVContextDelete;
        private System.Windows.Forms.ToolStripSeparator mTVContextSep1;
        private System.Windows.Forms.ToolStripMenuItem mTVContextMoveUp;
        private System.Windows.Forms.ToolStripMenuItem mTVContextMoveDown;
        private System.Windows.Forms.ToolStripSeparator mTVContextSep2;
        private System.Windows.Forms.ToolStripMenuItem mTVContextCopy;
        private System.Windows.Forms.ToolStripMenuItem mTVContextPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem mTVContextDuplicate;
    }
}

