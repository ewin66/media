namespace FutureConcepts.Media.TV.Scanner
{
    partial class ChannelKeypad
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
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum9;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelKeypad));
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum8;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum7;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum6;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum5;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum4;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum3;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum2;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum1;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnNum0;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnDot;
            FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton btnEnter;
            this.ilDigit = new System.Windows.Forms.ImageList(this.components);
            this.ilEnter = new System.Windows.Forms.ImageList(this.components);
            this.inputTimeoutTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            btnNum9 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum8 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum7 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum6 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum5 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum4 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum3 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum2 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum1 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnNum0 = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnDot = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            btnEnter = new FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons.FCClickButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNum9
            // 
            btnNum9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum9.ForeColor = System.Drawing.Color.White;
            btnNum9.ImageIndex = 0;
            btnNum9.ImageList = this.ilDigit;
            btnNum9.Location = new System.Drawing.Point(117, 35);
            btnNum9.Margin = new System.Windows.Forms.Padding(2);
            btnNum9.Name = "btnNum9";
            btnNum9.Size = new System.Drawing.Size(24, 24);
            btnNum9.TabIndex = 9;
            btnNum9.Text = "9";
            btnNum9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum9.Toggle = false;
            btnNum9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // ilDigit
            // 
            this.ilDigit.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilDigit.ImageStream")));
            this.ilDigit.TransparentColor = System.Drawing.Color.Transparent;
            this.ilDigit.Images.SetKeyName(0, "blank-up.png");
            this.ilDigit.Images.SetKeyName(1, "blank-down.png");
            // 
            // btnNum8
            // 
            btnNum8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum8.ForeColor = System.Drawing.Color.White;
            btnNum8.ImageIndex = 0;
            btnNum8.ImageList = this.ilDigit;
            btnNum8.Location = new System.Drawing.Point(89, 35);
            btnNum8.Margin = new System.Windows.Forms.Padding(2);
            btnNum8.Name = "btnNum8";
            btnNum8.Size = new System.Drawing.Size(24, 24);
            btnNum8.TabIndex = 8;
            btnNum8.Text = "8";
            btnNum8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum8.Toggle = false;
            btnNum8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum7
            // 
            btnNum7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum7.ForeColor = System.Drawing.Color.White;
            btnNum7.ImageIndex = 0;
            btnNum7.ImageList = this.ilDigit;
            btnNum7.Location = new System.Drawing.Point(61, 35);
            btnNum7.Margin = new System.Windows.Forms.Padding(2);
            btnNum7.Name = "btnNum7";
            btnNum7.Size = new System.Drawing.Size(24, 24);
            btnNum7.TabIndex = 7;
            btnNum7.Text = "7";
            btnNum7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum7.Toggle = false;
            btnNum7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum6
            // 
            btnNum6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum6.ForeColor = System.Drawing.Color.White;
            btnNum6.ImageIndex = 0;
            btnNum6.ImageList = this.ilDigit;
            btnNum6.Location = new System.Drawing.Point(33, 35);
            btnNum6.Margin = new System.Windows.Forms.Padding(2);
            btnNum6.Name = "btnNum6";
            btnNum6.Size = new System.Drawing.Size(24, 24);
            btnNum6.TabIndex = 6;
            btnNum6.Text = "6";
            btnNum6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum6.Toggle = false;
            btnNum6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum5
            // 
            btnNum5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum5.ForeColor = System.Drawing.Color.White;
            btnNum5.ImageIndex = 0;
            btnNum5.ImageList = this.ilDigit;
            btnNum5.Location = new System.Drawing.Point(5, 35);
            btnNum5.Margin = new System.Windows.Forms.Padding(2);
            btnNum5.Name = "btnNum5";
            btnNum5.Size = new System.Drawing.Size(24, 24);
            btnNum5.TabIndex = 5;
            btnNum5.Text = "5";
            btnNum5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum5.Toggle = false;
            btnNum5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum4
            // 
            btnNum4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum4.ForeColor = System.Drawing.Color.White;
            btnNum4.ImageIndex = 0;
            btnNum4.ImageList = this.ilDigit;
            btnNum4.Location = new System.Drawing.Point(117, 7);
            btnNum4.Margin = new System.Windows.Forms.Padding(2);
            btnNum4.Name = "btnNum4";
            btnNum4.Size = new System.Drawing.Size(24, 24);
            btnNum4.TabIndex = 4;
            btnNum4.Text = "4";
            btnNum4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum4.Toggle = false;
            btnNum4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum3
            // 
            btnNum3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum3.ForeColor = System.Drawing.Color.White;
            btnNum3.ImageIndex = 0;
            btnNum3.ImageList = this.ilDigit;
            btnNum3.Location = new System.Drawing.Point(89, 7);
            btnNum3.Margin = new System.Windows.Forms.Padding(2);
            btnNum3.Name = "btnNum3";
            btnNum3.Size = new System.Drawing.Size(24, 24);
            btnNum3.TabIndex = 3;
            btnNum3.Text = "3";
            btnNum3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum3.Toggle = false;
            btnNum3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum2
            // 
            btnNum2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum2.ForeColor = System.Drawing.Color.White;
            btnNum2.ImageIndex = 0;
            btnNum2.ImageList = this.ilDigit;
            btnNum2.Location = new System.Drawing.Point(61, 7);
            btnNum2.Margin = new System.Windows.Forms.Padding(2);
            btnNum2.Name = "btnNum2";
            btnNum2.Size = new System.Drawing.Size(24, 24);
            btnNum2.TabIndex = 2;
            btnNum2.Text = "2";
            btnNum2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum2.Toggle = false;
            btnNum2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum1
            // 
            btnNum1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum1.ForeColor = System.Drawing.Color.White;
            btnNum1.ImageIndex = 0;
            btnNum1.ImageList = this.ilDigit;
            btnNum1.Location = new System.Drawing.Point(33, 7);
            btnNum1.Margin = new System.Windows.Forms.Padding(2);
            btnNum1.Name = "btnNum1";
            btnNum1.Size = new System.Drawing.Size(24, 24);
            btnNum1.TabIndex = 1;
            btnNum1.Text = "1";
            btnNum1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum1.Toggle = false;
            btnNum1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnNum0
            // 
            btnNum0.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnNum0.ForeColor = System.Drawing.Color.White;
            btnNum0.ImageIndex = 0;
            btnNum0.ImageList = this.ilDigit;
            btnNum0.Location = new System.Drawing.Point(5, 7);
            btnNum0.Margin = new System.Windows.Forms.Padding(2);
            btnNum0.Name = "btnNum0";
            btnNum0.Size = new System.Drawing.Size(24, 24);
            btnNum0.TabIndex = 0;
            btnNum0.Text = "0";
            btnNum0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnNum0.Toggle = false;
            btnNum0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnDot
            // 
            btnDot.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnDot.ForeColor = System.Drawing.Color.White;
            btnDot.ImageIndex = 0;
            btnDot.ImageList = this.ilDigit;
            btnDot.Location = new System.Drawing.Point(145, 35);
            btnDot.Margin = new System.Windows.Forms.Padding(2);
            btnDot.Name = "btnDot";
            btnDot.Size = new System.Drawing.Size(24, 24);
            btnDot.TabIndex = 11;
            btnDot.Text = ".";
            btnDot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnDot.Toggle = false;
            btnDot.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDigit_MouseDown);
            // 
            // btnEnter
            // 
            btnEnter.BackColor = System.Drawing.Color.Black;
            btnEnter.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnEnter.ForeColor = System.Drawing.Color.White;
            btnEnter.ImageIndex = 0;
            btnEnter.ImageList = this.ilEnter;
            btnEnter.Location = new System.Drawing.Point(145, 7);
            btnEnter.Margin = new System.Windows.Forms.Padding(2);
            btnEnter.Name = "btnEnter";
            btnEnter.Size = new System.Drawing.Size(50, 24);
            btnEnter.TabIndex = 10;
            btnEnter.Text = " Enter";
            btnEnter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btnEnter.Toggle = false;
            btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // ilEnter
            // 
            this.ilEnter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilEnter.ImageStream")));
            this.ilEnter.TransparentColor = System.Drawing.Color.Transparent;
            this.ilEnter.Images.SetKeyName(0, "enter-up.gif");
            this.ilEnter.Images.SetKeyName(1, "enter-down.gif");
            // 
            // inputTimeoutTimer
            // 
            this.inputTimeoutTimer.Interval = 2000;
            this.inputTimeoutTimer.Tick += new System.EventHandler(this.inputTimeoutTimer_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(btnDot);
            this.panel1.Controls.Add(btnEnter);
            this.panel1.Controls.Add(btnNum9);
            this.panel1.Controls.Add(btnNum8);
            this.panel1.Controls.Add(btnNum0);
            this.panel1.Controls.Add(btnNum7);
            this.panel1.Controls.Add(btnNum1);
            this.panel1.Controls.Add(btnNum6);
            this.panel1.Controls.Add(btnNum2);
            this.panel1.Controls.Add(btnNum5);
            this.panel1.Controls.Add(btnNum3);
            this.panel1.Controls.Add(btnNum4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 64);
            this.panel1.TabIndex = 10;
            // 
            // ChannelKeypad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Name = "ChannelKeypad";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(201, 68);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ilDigit;
        private System.Windows.Forms.Timer inputTimeoutTimer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList ilEnter;
    }
}
