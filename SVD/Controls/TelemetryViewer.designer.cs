namespace FutureConcepts.Media.SVD.Controls
{
    partial class TelemetryViewer
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
            System.Windows.Forms.Label lblUpdated;
            System.Windows.Forms.Label lblState;
            this.lblUsers = new System.Windows.Forms.Label();
            this.P_Telemetry = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblBitrate = new System.Windows.Forms.Label();
            this.lblUpdatedValue = new System.Windows.Forms.Label();
            this.lblUsersValue = new System.Windows.Forms.Label();
            this.lblBitrateValue = new System.Windows.Forms.Label();
            this.lblStateValue = new System.Windows.Forms.Label();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            lblUpdated = new System.Windows.Forms.Label();
            lblState = new System.Windows.Forms.Label();
            this.P_Telemetry.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUsers
            // 
            this.lblUsers.AutoSize = true;
            this.lblUsers.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblUsers.Location = new System.Drawing.Point(14, 38);
            this.lblUsers.Margin = new System.Windows.Forms.Padding(0);
            this.lblUsers.Name = "lblUsers";
            this.lblUsers.Size = new System.Drawing.Size(37, 19);
            this.lblUsers.TabIndex = 4;
            this.lblUsers.Text = "Users:";
            // 
            // lblUpdated
            // 
            lblUpdated.AutoSize = true;
            lblUpdated.Dock = System.Windows.Forms.DockStyle.Right;
            lblUpdated.Location = new System.Drawing.Point(0, 0);
            lblUpdated.Margin = new System.Windows.Forms.Padding(0);
            lblUpdated.Name = "lblUpdated";
            lblUpdated.Size = new System.Drawing.Size(51, 19);
            lblUpdated.TabIndex = 9;
            lblUpdated.Text = "Updated:";
            // 
            // lblState
            // 
            lblState.AutoSize = true;
            lblState.Dock = System.Windows.Forms.DockStyle.Right;
            lblState.Location = new System.Drawing.Point(16, 76);
            lblState.Margin = new System.Windows.Forms.Padding(0);
            lblState.Name = "lblState";
            lblState.Size = new System.Drawing.Size(35, 23);
            lblState.TabIndex = 7;
            lblState.Text = "State:";
            // 
            // P_Telemetry
            // 
            this.P_Telemetry.AutoSize = true;
            this.P_Telemetry.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.P_Telemetry.BackColor = System.Drawing.Color.Black;
            this.P_Telemetry.Controls.Add(this.tableLayoutPanel1);
            this.P_Telemetry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.P_Telemetry.ForeColor = System.Drawing.Color.White;
            this.P_Telemetry.Location = new System.Drawing.Point(1, 1);
            this.P_Telemetry.Name = "P_Telemetry";
            this.P_Telemetry.Size = new System.Drawing.Size(168, 99);
            this.P_Telemetry.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblUsers, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBitrate, 0, 3);
            this.tableLayoutPanel1.Controls.Add(lblUpdated, 0, 0);
            this.tableLayoutPanel1.Controls.Add(lblState, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblUpdatedValue, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblUsersValue, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblBitrateValue, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblStateValue, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(168, 99);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // lblBitrate
            // 
            this.lblBitrate.AutoSize = true;
            this.lblBitrate.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblBitrate.Location = new System.Drawing.Point(11, 57);
            this.lblBitrate.Margin = new System.Windows.Forms.Padding(0);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(40, 19);
            this.lblBitrate.TabIndex = 0;
            this.lblBitrate.Text = "Bitrate:";
            this.lblBitrate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUpdatedValue
            // 
            this.lblUpdatedValue.AutoSize = true;
            this.lblUpdatedValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUpdatedValue.Location = new System.Drawing.Point(51, 0);
            this.lblUpdatedValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblUpdatedValue.Name = "lblUpdatedValue";
            this.lblUpdatedValue.Size = new System.Drawing.Size(117, 19);
            this.lblUpdatedValue.TabIndex = 10;
            this.lblUpdatedValue.Text = "-";
            // 
            // lblUsersValue
            // 
            this.lblUsersValue.AutoSize = true;
            this.lblUsersValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsersValue.Location = new System.Drawing.Point(51, 38);
            this.lblUsersValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblUsersValue.Name = "lblUsersValue";
            this.lblUsersValue.Size = new System.Drawing.Size(117, 19);
            this.lblUsersValue.TabIndex = 12;
            this.lblUsersValue.Text = "-";
            // 
            // lblBitrateValue
            // 
            this.lblBitrateValue.AutoSize = true;
            this.lblBitrateValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBitrateValue.Location = new System.Drawing.Point(51, 57);
            this.lblBitrateValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblBitrateValue.Name = "lblBitrateValue";
            this.lblBitrateValue.Size = new System.Drawing.Size(117, 19);
            this.lblBitrateValue.TabIndex = 13;
            this.lblBitrateValue.Text = "-";
            // 
            // lblStateValue
            // 
            this.lblStateValue.AutoSize = true;
            this.lblStateValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStateValue.Location = new System.Drawing.Point(51, 76);
            this.lblStateValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblStateValue.Name = "lblStateValue";
            this.lblStateValue.Size = new System.Drawing.Size(117, 23);
            this.lblStateValue.TabIndex = 14;
            this.lblStateValue.Text = "-";
            // 
            // tt
            // 
            this.tt.AutomaticDelay = 525;
            this.tt.AutoPopDelay = 3000;
            this.tt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tt.ForeColor = System.Drawing.Color.White;
            this.tt.InitialDelay = 10;
            this.tt.IsBalloon = true;
            this.tt.ReshowDelay = 10;
            // 
            // TelemetryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.P_Telemetry);
            this.Name = "TelemetryViewer";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(170, 101);
            this.P_Telemetry.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel P_Telemetry;
        private System.Windows.Forms.Label lblBitrate;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblUpdatedValue;
        private System.Windows.Forms.Label lblUsersValue;
        private System.Windows.Forms.Label lblBitrateValue;
        private System.Windows.Forms.Label lblStateValue;
        private System.Windows.Forms.Label lblUsers;
    }
}
