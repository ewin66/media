namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    partial class ProfileGroupPropertiesEditor
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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDefaultProfile = new System.Windows.Forms.Label();
            this.cbDefaultProfile = new System.Windows.Forms.ComboBox();
            this.general_lblLowVid = new System.Windows.Forms.Label();
            this.general_lblHiVid = new System.Windows.Forms.Label();
            this.general_txtLowVid = new System.Windows.Forms.Label();
            this.general_txtHiVid = new System.Windows.Forms.Label();
            this.general_txtHiAud = new System.Windows.Forms.Label();
            this.general_txtLowAud = new System.Windows.Forms.Label();
            this.general_lblHiAud = new System.Windows.Forms.Label();
            this.general_lblLoAud = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.lblComment = new System.Windows.Forms.Label();
            this.lblCharCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(5, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(102, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Profile Group Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.Black;
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(113, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(243, 20);
            this.txtName.TabIndex = 1;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblDefaultProfile
            // 
            this.lblDefaultProfile.AutoSize = true;
            this.lblDefaultProfile.Location = new System.Drawing.Point(31, 56);
            this.lblDefaultProfile.Name = "lblDefaultProfile";
            this.lblDefaultProfile.Size = new System.Drawing.Size(76, 13);
            this.lblDefaultProfile.TabIndex = 2;
            this.lblDefaultProfile.Text = "Default Profile:";
            // 
            // cbDefaultProfile
            // 
            this.cbDefaultProfile.BackColor = System.Drawing.Color.Black;
            this.cbDefaultProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDefaultProfile.ForeColor = System.Drawing.Color.White;
            this.cbDefaultProfile.FormattingEnabled = true;
            this.cbDefaultProfile.Location = new System.Drawing.Point(113, 53);
            this.cbDefaultProfile.Name = "cbDefaultProfile";
            this.cbDefaultProfile.Size = new System.Drawing.Size(243, 21);
            this.cbDefaultProfile.TabIndex = 3;
            this.cbDefaultProfile.SelectedIndexChanged += new System.EventHandler(this.cbDefaultProfile_SelectedIndexChanged);
            // 
            // general_lblLowVid
            // 
            this.general_lblLowVid.AutoSize = true;
            this.general_lblLowVid.Location = new System.Drawing.Point(5, 123);
            this.general_lblLowVid.Name = "general_lblLowVid";
            this.general_lblLowVid.Size = new System.Drawing.Size(107, 13);
            this.general_lblLowVid.TabIndex = 4;
            this.general_lblLowVid.Text = "Lowest Video Bitrate:";
            // 
            // general_lblHiVid
            // 
            this.general_lblHiVid.AutoSize = true;
            this.general_lblHiVid.Location = new System.Drawing.Point(3, 146);
            this.general_lblHiVid.Name = "general_lblHiVid";
            this.general_lblHiVid.Size = new System.Drawing.Size(109, 13);
            this.general_lblHiVid.TabIndex = 5;
            this.general_lblHiVid.Text = "Highest Video Bitrate:";
            // 
            // general_txtLowVid
            // 
            this.general_txtLowVid.AutoSize = true;
            this.general_txtLowVid.Location = new System.Drawing.Point(115, 123);
            this.general_txtLowVid.Name = "general_txtLowVid";
            this.general_txtLowVid.Size = new System.Drawing.Size(45, 13);
            this.general_txtLowVid.TabIndex = 6;
            this.general_txtLowVid.Text = "00 kbps";
            // 
            // general_txtHiVid
            // 
            this.general_txtHiVid.AutoSize = true;
            this.general_txtHiVid.Location = new System.Drawing.Point(115, 146);
            this.general_txtHiVid.Name = "general_txtHiVid";
            this.general_txtHiVid.Size = new System.Drawing.Size(57, 13);
            this.general_txtHiVid.TabIndex = 7;
            this.general_txtHiVid.Text = "0000 kbps";
            // 
            // general_txtHiAud
            // 
            this.general_txtHiAud.AutoSize = true;
            this.general_txtHiAud.Location = new System.Drawing.Point(304, 146);
            this.general_txtHiAud.Name = "general_txtHiAud";
            this.general_txtHiAud.Size = new System.Drawing.Size(57, 13);
            this.general_txtHiAud.TabIndex = 11;
            this.general_txtHiAud.Text = "0000 kbps";
            // 
            // general_txtLowAud
            // 
            this.general_txtLowAud.AutoSize = true;
            this.general_txtLowAud.Location = new System.Drawing.Point(304, 123);
            this.general_txtLowAud.Name = "general_txtLowAud";
            this.general_txtLowAud.Size = new System.Drawing.Size(45, 13);
            this.general_txtLowAud.TabIndex = 10;
            this.general_txtLowAud.Text = "00 kbps";
            // 
            // general_lblHiAud
            // 
            this.general_lblHiAud.AutoSize = true;
            this.general_lblHiAud.Location = new System.Drawing.Point(192, 146);
            this.general_lblHiAud.Name = "general_lblHiAud";
            this.general_lblHiAud.Size = new System.Drawing.Size(109, 13);
            this.general_lblHiAud.TabIndex = 9;
            this.general_lblHiAud.Text = "Highest Audio Bitrate:";
            // 
            // general_lblLoAud
            // 
            this.general_lblLoAud.AutoSize = true;
            this.general_lblLoAud.Location = new System.Drawing.Point(194, 123);
            this.general_lblLoAud.Name = "general_lblLoAud";
            this.general_lblLoAud.Size = new System.Drawing.Size(107, 13);
            this.general_lblLoAud.TabIndex = 8;
            this.general_lblLoAud.Text = "Lowest Audio Bitrate:";
            // 
            // txtComment
            // 
            this.txtComment.BackColor = System.Drawing.Color.Black;
            this.txtComment.ForeColor = System.Drawing.Color.White;
            this.txtComment.Location = new System.Drawing.Point(113, 80);
            this.txtComment.MaxLength = 40;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(243, 20);
            this.txtComment.TabIndex = 13;
            this.txtComment.TextChanged += new System.EventHandler(this.txtComment_TextChanged);
            // 
            // lblComment
            // 
            this.lblComment.AutoSize = true;
            this.lblComment.Location = new System.Drawing.Point(53, 83);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(54, 13);
            this.lblComment.TabIndex = 12;
            this.lblComment.Text = "Comment:";
            // 
            // lblCharCount
            // 
            this.lblCharCount.AutoSize = true;
            this.lblCharCount.Location = new System.Drawing.Point(362, 83);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new System.Drawing.Size(71, 13);
            this.lblCharCount.TabIndex = 14;
            this.lblCharCount.Text = "(40 chars left)";
            // 
            // ProfileGroupPropertiesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.lblCharCount);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.general_txtHiAud);
            this.Controls.Add(this.general_txtLowAud);
            this.Controls.Add(this.general_lblHiAud);
            this.Controls.Add(this.general_lblLoAud);
            this.Controls.Add(this.general_txtHiVid);
            this.Controls.Add(this.general_txtLowVid);
            this.Controls.Add(this.general_lblHiVid);
            this.Controls.Add(this.general_lblLowVid);
            this.Controls.Add(this.cbDefaultProfile);
            this.Controls.Add(this.lblDefaultProfile);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ProfileGroupPropertiesEditor";
            this.Size = new System.Drawing.Size(453, 313);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblDefaultProfile;
        private System.Windows.Forms.ComboBox cbDefaultProfile;
        private System.Windows.Forms.Label general_lblLowVid;
        private System.Windows.Forms.Label general_lblHiVid;
        private System.Windows.Forms.Label general_txtLowVid;
        private System.Windows.Forms.Label general_txtHiVid;
        private System.Windows.Forms.Label general_txtHiAud;
        private System.Windows.Forms.Label general_txtLowAud;
        private System.Windows.Forms.Label general_lblHiAud;
        private System.Windows.Forms.Label general_lblLoAud;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.Label lblCharCount;
    }
}
