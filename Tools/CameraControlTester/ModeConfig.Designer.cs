namespace FutureConcepts.Media.Tools.CameraControlTester
{
    partial class ModeConfig
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
            this.gb_mode = new System.Windows.Forms.GroupBox();
            this.rb_client = new System.Windows.Forms.RadioButton();
            this.rb_serial = new System.Windows.Forms.RadioButton();
            this.btnOK = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.gb_address = new System.Windows.Forms.GroupBox();
            this.cbPTZType = new System.Windows.Forms.ComboBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSourceName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton();
            this.gb_mode.SuspendLayout();
            this.gb_address.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_mode
            // 
            this.gb_mode.Controls.Add(this.rb_client);
            this.gb_mode.Controls.Add(this.rb_serial);
            this.gb_mode.ForeColor = System.Drawing.Color.White;
            this.gb_mode.Location = new System.Drawing.Point(12, 12);
            this.gb_mode.Name = "gb_mode";
            this.gb_mode.Size = new System.Drawing.Size(265, 48);
            this.gb_mode.TabIndex = 3;
            this.gb_mode.TabStop = false;
            this.gb_mode.Text = "Mode";
            // 
            // rb_client
            // 
            this.rb_client.AutoSize = true;
            this.rb_client.Location = new System.Drawing.Point(135, 19);
            this.rb_client.Name = "rb_client";
            this.rb_client.Size = new System.Drawing.Size(93, 17);
            this.rb_client.TabIndex = 1;
            this.rb_client.TabStop = true;
            this.rb_client.Text = "Client / Server";
            this.rb_client.UseVisualStyleBackColor = true;
            this.rb_client.CheckedChanged += new System.EventHandler(this.rb_mode_CheckedChanged);
            // 
            // rb_serial
            // 
            this.rb_serial.AutoSize = true;
            this.rb_serial.Location = new System.Drawing.Point(6, 19);
            this.rb_serial.Name = "rb_serial";
            this.rb_serial.Size = new System.Drawing.Size(53, 17);
            this.rb_serial.TabIndex = 0;
            this.rb_serial.TabStop = true;
            this.rb_serial.Text = "Direct";
            this.rb_serial.UseVisualStyleBackColor = true;
            this.rb_serial.CheckedChanged += new System.EventHandler(this.rb_mode_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(124, 198);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gb_address
            // 
            this.gb_address.Controls.Add(this.cbPTZType);
            this.gb_address.Controls.Add(this.txtAddress);
            this.gb_address.Controls.Add(this.label4);
            this.gb_address.Controls.Add(this.label3);
            this.gb_address.Controls.Add(this.txtSourceName);
            this.gb_address.Controls.Add(this.label2);
            this.gb_address.Controls.Add(this.txtServerAddress);
            this.gb_address.Controls.Add(this.label1);
            this.gb_address.ForeColor = System.Drawing.Color.White;
            this.gb_address.Location = new System.Drawing.Point(12, 66);
            this.gb_address.Name = "gb_address";
            this.gb_address.Size = new System.Drawing.Size(265, 121);
            this.gb_address.TabIndex = 5;
            this.gb_address.TabStop = false;
            this.gb_address.Text = "Address";
            // 
            // cbPTZType
            // 
            this.cbPTZType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPTZType.FormattingEnabled = true;
            this.cbPTZType.Location = new System.Drawing.Point(91, 67);
            this.cbPTZType.Name = "cbPTZType";
            this.cbPTZType.Size = new System.Drawing.Size(168, 21);
            this.cbPTZType.TabIndex = 7;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(91, 90);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(168, 20);
            this.txtAddress.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "PTZType:";
            // 
            // txtSourceName
            // 
            this.txtSourceName.Location = new System.Drawing.Point(91, 35);
            this.txtSourceName.Name = "txtSourceName";
            this.txtSourceName.Size = new System.Drawing.Size(168, 20);
            this.txtSourceName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "SourceName:";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(91, 13);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(168, 20);
            this.txtServerAddress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ServerAddress:";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Red;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(205, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ModeConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 233);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gb_address);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gb_mode);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ModeConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera Control Tester Configuration";
            this.Load += new System.EventHandler(this.ModeConfig_Load);
            this.gb_mode.ResumeLayout(false);
            this.gb_mode.PerformLayout();
            this.gb_address.ResumeLayout(false);
            this.gb_address.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_mode;
        private System.Windows.Forms.RadioButton rb_serial;
        private System.Windows.Forms.RadioButton rb_client;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnOK;
        private System.Windows.Forms.GroupBox gb_address;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbPTZType;
        private FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.RedFlatButton btnCancel;
    }
}