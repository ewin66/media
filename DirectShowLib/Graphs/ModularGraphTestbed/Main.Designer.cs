namespace FutureConcepts.Media.DirectShowLib.Graphs.ModularGraphTestbed
{
    partial class Main
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
            this.btnAddSource = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tbLtsfUrl = new System.Windows.Forms.TextBox();
            this.spinner = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbSinks = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnTestPattern = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSource
            // 
            this.btnAddSource.Location = new System.Drawing.Point(175, 17);
            this.btnAddSource.Name = "btnAddSource";
            this.btnAddSource.Size = new System.Drawing.Size(75, 23);
            this.btnAddSource.TabIndex = 0;
            this.btnAddSource.Text = "Connect";
            this.btnAddSource.UseVisualStyleBackColor = true;
            this.btnAddSource.Click += new System.EventHandler(this.btnAddSource_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(175, 19);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(256, 19);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tbLtsfUrl
            // 
            this.tbLtsfUrl.Location = new System.Drawing.Point(6, 19);
            this.tbLtsfUrl.Name = "tbLtsfUrl";
            this.tbLtsfUrl.Size = new System.Drawing.Size(163, 20);
            this.tbLtsfUrl.TabIndex = 5;
            this.tbLtsfUrl.Text = "ltsf:dgram://127.0.0.1:27015";
            // 
            // spinner
            // 
            this.spinner.AutoSize = true;
            this.spinner.Location = new System.Drawing.Point(5, 24);
            this.spinner.Name = "spinner";
            this.spinner.Size = new System.Drawing.Size(25, 13);
            this.spinner.TabIndex = 6;
            this.spinner.Text = "100";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(256, 22);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(45, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Inactive";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbLtsfUrl);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.btnAddSource);
            this.groupBox1.Location = new System.Drawing.Point(12, 124);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(383, 47);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "2. LTNetworkSource";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.spinner);
            this.groupBox2.Controls.Add(this.btnRun);
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Location = new System.Drawing.Point(12, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(383, 49);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "3. Control";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbSinks);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(383, 51);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "1. Renderer";
            // 
            // cbSinks
            // 
            this.cbSinks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSinks.FormattingEnabled = true;
            this.cbSinks.Location = new System.Drawing.Point(6, 19);
            this.cbSinks.Name = "cbSinks";
            this.cbSinks.Size = new System.Drawing.Size(371, 21);
            this.cbSinks.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnTestPattern);
            this.groupBox4.Location = new System.Drawing.Point(12, 71);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(383, 47);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "2. Test Pattern";
            // 
            // btnTestPattern
            // 
            this.btnTestPattern.Location = new System.Drawing.Point(175, 18);
            this.btnTestPattern.Name = "btnTestPattern";
            this.btnTestPattern.Size = new System.Drawing.Size(75, 23);
            this.btnTestPattern.TabIndex = 0;
            this.btnTestPattern.Text = "Instantiate";
            this.btnTestPattern.UseVisualStyleBackColor = true;
            this.btnTestPattern.Click += new System.EventHandler(this.btnTestPattern_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 243);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ModularGraph Testbed";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddSource;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox tbLtsfUrl;
        private System.Windows.Forms.Label spinner;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbSinks;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnTestPattern;
    }
}