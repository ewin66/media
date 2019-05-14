namespace FutureConcepts.Media.SVD.Controls
{
    partial class MicrowavePalette
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
            this.keyInput = new FutureConcepts.Media.SVD.Controls.EncryptionKeyInput();
            this.busy = new FutureConcepts.Media.SVD.Controls.PaletteBusy();
            this.presets = new FutureConcepts.Media.SVD.Controls.UserPresetsControl();
            this.sweep = new FutureConcepts.Media.SVD.Controls.MicrowaveFreqSweep();
            this.tuner = new FutureConcepts.Media.SVD.Controls.MicrowaveTuner();
            this.SuspendLayout();
            // 
            // keyInput
            // 
            this.keyInput.BackColor = System.Drawing.Color.Black;
            this.keyInput.Capabilities = null;
            this.keyInput.Dock = System.Windows.Forms.DockStyle.Left;
            this.keyInput.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keyInput.ForeColor = System.Drawing.Color.White;
            this.keyInput.Location = new System.Drawing.Point(674, 0);
            this.keyInput.Name = "keyInput";
            this.keyInput.SelectedEncryption = null;
            this.keyInput.Size = new System.Drawing.Size(279, 166);
            this.keyInput.TabIndex = 4;
            this.keyInput.Visible = false;
            this.keyInput.DialogClosed += new System.ComponentModel.CancelEventHandler(this.keyInput_DialogClosed);
            // 
            // busy
            // 
            this.busy.BackColor = System.Drawing.Color.Black;
            this.busy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.busy.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.busy.ForeColor = System.Drawing.Color.White;
            this.busy.InfoText = "Waiting for server...";
            this.busy.Location = new System.Drawing.Point(674, 0);
            this.busy.Name = "busy";
            this.busy.Size = new System.Drawing.Size(0, 166);
            this.busy.TabIndex = 3;
            // 
            // presets
            // 
            this.presets.BackColor = System.Drawing.Color.Black;
            this.presets.Dock = System.Windows.Forms.DockStyle.Left;
            this.presets.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.presets.ForeColor = System.Drawing.Color.White;
            this.presets.Location = new System.Drawing.Point(343, 0);
            this.presets.Name = "presets";
            this.presets.Size = new System.Drawing.Size(331, 166);
            this.presets.TabIndex = 2;
            this.presets.Title = "Frequency Presets";
            this.presets.Visible = false;
            this.presets.AddPreset += new System.EventHandler(this.presets_AddPreset);
            this.presets.RestorePreset += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.presets_RestorePreset);
            this.presets.PresetRenamed += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.presets_PresetRenamed);
            this.presets.PresetDeleted += new System.EventHandler<FutureConcepts.Media.SVD.Controls.UserPresetEventArgs>(this.presets_PresetDeleted);
            this.presets.PresetsCleared += new System.EventHandler(this.presets_PresetsCleared);
            // 
            // sweep
            // 
            this.sweep.BackColor = System.Drawing.Color.Black;
            this.sweep.Dock = System.Windows.Forms.DockStyle.Left;
            this.sweep.EndFrequency = 2700;
            this.sweep.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sweep.ForeColor = System.Drawing.Color.White;
            this.sweep.Location = new System.Drawing.Point(184, 0);
            this.sweep.Name = "sweep";
            this.sweep.Progress = 0;
            this.sweep.Size = new System.Drawing.Size(159, 166);
            this.sweep.StartFrequency = 1700;
            this.sweep.TabIndex = 1;
            this.sweep.Threshold = 20;
            this.sweep.Visible = false;
            this.sweep.StartSweep += new System.EventHandler(this.sweep_StartSweep);
            this.sweep.CancelSweep += new System.EventHandler(this.sweep_CancelSweep);
            // 
            // tuner
            // 
            this.tuner.BackColor = System.Drawing.Color.Black;
            this.tuner.Dock = System.Windows.Forms.DockStyle.Left;
            this.tuner.EncryptionKeyActive = false;
            this.tuner.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tuner.ForeColor = System.Drawing.Color.White;
            this.tuner.Frequency = ((ulong)(9999000000ul));
            this.tuner.InputEnabled = true;
            this.tuner.Location = new System.Drawing.Point(0, 0);
            this.tuner.MinimumSize = new System.Drawing.Size(184, 166);
            this.tuner.Name = "tuner";
            this.tuner.ReceivedCarrierLevel = 1D;
            this.tuner.SignalToNoiseRatio = 40D;
            this.tuner.Size = new System.Drawing.Size(184, 166);
            this.tuner.TabIndex = 0;
            this.tuner.TentativeFrequency = ((ulong)(0ul));
            this.tuner.Visible = false;
            this.tuner.FrequencyChanged += new System.EventHandler(this.tuner_FrequencyChanged);
            this.tuner.EnterEncryptionKey += new System.EventHandler(this.tuner_EnterEncryptionKey);
            // 
            // MicrowavePalette
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.keyInput);
            this.Controls.Add(this.busy);
            this.Controls.Add(this.presets);
            this.Controls.Add(this.sweep);
            this.Controls.Add(this.tuner);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "MicrowavePalette";
            this.Size = new System.Drawing.Size(520, 166);
            this.ResumeLayout(false);

        }

        #endregion

        private MicrowaveTuner tuner;
        private MicrowaveFreqSweep sweep;
        private UserPresetsControl presets;
        private PaletteBusy busy;
        private EncryptionKeyInput keyInput;

    }
}
