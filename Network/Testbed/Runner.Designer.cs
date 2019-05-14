namespace FutureConcepts.Media.Network.Test
{
    partial class Runner
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
            System.Windows.Forms.Button spawn;
            System.Windows.Forms.Button kill;
            this.run = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.pause = new System.Windows.Forms.Button();
            this.cbGraphTypes = new System.Windows.Forms.ComboBox();
            spawn = new System.Windows.Forms.Button();
            kill = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // spawn
            // 
            spawn.Location = new System.Drawing.Point(12, 39);
            spawn.Name = "spawn";
            spawn.Size = new System.Drawing.Size(75, 23);
            spawn.TabIndex = 0;
            spawn.Text = "Spawn";
            spawn.UseVisualStyleBackColor = true;
            spawn.Click += new System.EventHandler(this.spawn_Click);
            // 
            // kill
            // 
            kill.Location = new System.Drawing.Point(93, 39);
            kill.Name = "kill";
            kill.Size = new System.Drawing.Size(75, 23);
            kill.TabIndex = 1;
            kill.Text = "Kill";
            kill.UseVisualStyleBackColor = true;
            kill.Click += new System.EventHandler(this.kill_Click);
            // 
            // run
            // 
            this.run.Location = new System.Drawing.Point(237, 39);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(75, 23);
            this.run.TabIndex = 2;
            this.run.Text = "Run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(399, 39);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(75, 23);
            this.stop.TabIndex = 3;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // pause
            // 
            this.pause.Location = new System.Drawing.Point(318, 39);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(75, 23);
            this.pause.TabIndex = 4;
            this.pause.Text = "Pause";
            this.pause.UseVisualStyleBackColor = true;
            this.pause.Click += new System.EventHandler(this.pause_Click);
            // 
            // cbGraphTypes
            // 
            this.cbGraphTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGraphTypes.DropDownWidth = 400;
            this.cbGraphTypes.Location = new System.Drawing.Point(12, 12);
            this.cbGraphTypes.Name = "cbGraphTypes";
            this.cbGraphTypes.Size = new System.Drawing.Size(462, 21);
            this.cbGraphTypes.TabIndex = 5;
            this.cbGraphTypes.SelectedIndexChanged += new System.EventHandler(this.cbGraphTypes_SelectedIndexChanged);
            // 
            // Runner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 71);
            this.Controls.Add(this.cbGraphTypes);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.run);
            this.Controls.Add(kill);
            this.Controls.Add(spawn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Runner";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter Testbed - Runner";
            this.Load += new System.EventHandler(this.Runner_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbGraphTypes;
        private System.Windows.Forms.Button run;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Button pause;
    }
}

