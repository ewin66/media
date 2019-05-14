namespace FutureConcepts.Media.SVD.Controls
{
    partial class EncryptionKeyInput
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
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.gb = new System.Windows.Forms.GroupBox();
            this.clear = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.cancel = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.ok = new FutureConcepts.Media.CommonControls.RedFlatButton();
            this.pKeyValid = new System.Windows.Forms.Panel();
            this.key = new System.Windows.Forms.TextBox();
            this.keyLengthStatus = new System.Windows.Forms.Label();
            this.keyLength = new System.Windows.Forms.ComboBox();
            this.type = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            this.gb.SuspendLayout();
            this.pKeyValid.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 62);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(29, 13);
            label3.TabIndex = 5;
            label3.Text = "Key:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(201, 33);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(28, 13);
            label2.TabIndex = 3;
            label2.Text = "Bits:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 33);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 13);
            label1.TabIndex = 2;
            label1.Text = "Type:";
            // 
            // gb
            // 
            this.gb.Controls.Add(this.clear);
            this.gb.Controls.Add(this.cancel);
            this.gb.Controls.Add(this.ok);
            this.gb.Controls.Add(this.pKeyValid);
            this.gb.Controls.Add(this.keyLengthStatus);
            this.gb.Controls.Add(label3);
            this.gb.Controls.Add(label2);
            this.gb.Controls.Add(label1);
            this.gb.Controls.Add(this.keyLength);
            this.gb.Controls.Add(this.type);
            this.gb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb.ForeColor = System.Drawing.Color.White;
            this.gb.Location = new System.Drawing.Point(0, 0);
            this.gb.MinimumSize = new System.Drawing.Size(279, 166);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(279, 166);
            this.gb.TabIndex = 0;
            this.gb.TabStop = false;
            this.gb.Text = "Input Encryption Key:";
            // 
            // clear
            // 
            this.clear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clear.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clear.ForeColor = System.Drawing.Color.White;
            this.clear.Location = new System.Drawing.Point(6, 137);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(75, 23);
            this.clear.TabIndex = 11;
            this.clear.Text = "Clear";
            this.clear.UseVisualStyleBackColor = false;
            this.clear.Click += new System.EventHandler(this.clear_Click);
            // 
            // cancel
            // 
            this.cancel.BackColor = System.Drawing.Color.Red;
            this.cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.ForeColor = System.Drawing.Color.White;
            this.cancel.Location = new System.Drawing.Point(198, 137);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 10;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = false;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(96)))), ((int)(((byte)(160)))));
            this.ok.Enabled = false;
            this.ok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ok.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ok.ForeColor = System.Drawing.Color.White;
            this.ok.Location = new System.Drawing.Point(117, 137);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 9;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = false;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // pKeyValid
            // 
            this.pKeyValid.BackColor = System.Drawing.Color.Red;
            this.pKeyValid.Controls.Add(this.key);
            this.pKeyValid.Location = new System.Drawing.Point(6, 78);
            this.pKeyValid.Name = "pKeyValid";
            this.pKeyValid.Size = new System.Drawing.Size(269, 24);
            this.pKeyValid.TabIndex = 8;
            // 
            // key
            // 
            this.key.BackColor = System.Drawing.Color.Black;
            this.key.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.key.ForeColor = System.Drawing.Color.White;
            this.key.Location = new System.Drawing.Point(1, 1);
            this.key.Margin = new System.Windows.Forms.Padding(0);
            this.key.MaxLength = 32;
            this.key.Name = "key";
            this.key.Size = new System.Drawing.Size(267, 22);
            this.key.TabIndex = 6;
            this.key.TextChanged += new System.EventHandler(this.key_TextChanged);
            this.key.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.key_KeyPress);
            // 
            // keyLengthStatus
            // 
            this.keyLengthStatus.AutoSize = true;
            this.keyLengthStatus.Location = new System.Drawing.Point(41, 62);
            this.keyLengthStatus.Name = "keyLengthStatus";
            this.keyLengthStatus.Size = new System.Drawing.Size(35, 13);
            this.keyLengthStatus.TabIndex = 7;
            this.keyLengthStatus.Text = "0 / 32";
            // 
            // keyLength
            // 
            this.keyLength.BackColor = System.Drawing.Color.Black;
            this.keyLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.keyLength.ForeColor = System.Drawing.Color.White;
            this.keyLength.FormattingEnabled = true;
            this.keyLength.Location = new System.Drawing.Point(230, 30);
            this.keyLength.Name = "keyLength";
            this.keyLength.Size = new System.Drawing.Size(43, 21);
            this.keyLength.TabIndex = 1;
            this.keyLength.SelectedIndexChanged += new System.EventHandler(this.keyLength_SelectedIndexChanged);
            // 
            // type
            // 
            this.type.BackColor = System.Drawing.Color.Black;
            this.type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type.ForeColor = System.Drawing.Color.White;
            this.type.FormattingEnabled = true;
            this.type.Location = new System.Drawing.Point(40, 30);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(157, 21);
            this.type.TabIndex = 0;
            this.type.SelectedIndexChanged += new System.EventHandler(this.type_SelectedIndexChanged);
            this.type.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.type_Format);
            // 
            // EncryptionKeyInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.gb);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "EncryptionKeyInput";
            this.Size = new System.Drawing.Size(279, 166);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.pKeyValid.ResumeLayout(false);
            this.pKeyValid.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb;
        private System.Windows.Forms.ComboBox keyLength;
        private System.Windows.Forms.ComboBox type;
        private System.Windows.Forms.Label keyLengthStatus;
        private System.Windows.Forms.Panel pKeyValid;
        private System.Windows.Forms.TextBox key;
        private FutureConcepts.Media.CommonControls.RedFlatButton cancel;
        private FutureConcepts.Media.CommonControls.RedFlatButton ok;
        private FutureConcepts.Media.CommonControls.RedFlatButton clear;
    }
}
