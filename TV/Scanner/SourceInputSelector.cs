using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FutureConcepts.Controls.AntaresX.AntaresXControls.Buttons;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class SourceInputSelector : UserControl
    {
        public SourceInputSelector()
        {
            InitializeComponent();
        }

        #region Selected Source

        [Browsable(true), Category("Action"), Description("Fired when a source button is selected by the user.")]
        public event EventHandler SelectedSourceChanged;

        /// <summary>
        /// Fires the SelectedSourceChanged if any listeners are attached.
        /// </summary>
        private void FireSelectedSourceChanged()
        {
            if (SelectedSourceChanged != null)
            {
                SelectedSourceChanged(this, new EventArgs());
            }
        }

        private TVSource _selectedSource;
        /// <summary>
        /// Sets or Gets the currently selected source
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("The currently selected source")]
        public TVSource SelectedSource
        {
            get
            {
                return _selectedSource;
            }
            set
            {
                bool actuallyChanging = false;
                if (_selectedSource != value)
                {
                    actuallyChanging = true;
                    _selectedSource = value;
                }

                ResetSourceImageIndexes();

                GetSourceButton(value).ImageIndex = 1;

                if (actuallyChanging && Enabled)
                {
                    FireSelectedSourceChanged();
                }
            }
        }

        /// <summary>
        /// Returns a reference to the currently selected source button
        /// </summary>
        private FCButton GetSourceButton(TVSource source)
        {
            switch (source)
            {
                case TVSource.LocalAnalog:
                    return sourceBtn_LocalAnalog;
                case TVSource.LocalDigital:
                    return sourceBtn_LocalDigital;
                case TVSource.Network:
                    return sourceBtn_Network;
            }
            return null;
        }

        /// <summary>
        /// Resets all ImageIndex's on all FCButtons in sources to 0
        /// </summary>
        private void ResetSourceImageIndexes()
        {
            foreach (Control c in tbPanelSources.Controls)
            {
                if (c is FCButton)
                {
                    ((FCButton)c).ImageIndex = 0;
                }
            }
        }

        /// <summary>
        /// Resets all ImageIndex's on all FCButtons in inputs to 0
        /// </summary>
        private void ResetInputImageIndexes()
        {
            foreach (Control c in tbPanelInputs.Controls)
            {
                if (c is FCButton)
                {
                    ((FCButton)c).ImageIndex = 0;
                }
            }
        }

        private void sourceBtn_LocalDigital_Click(object sender, EventArgs e)
        {
            if (Enabled)
            {
                SelectedSource = TVSource.LocalDigital;
            }
        }

        private void sourceBtn_LocalAnalog_Click(object sender, EventArgs e)
        {
            if (Enabled)
            {
                SelectedSource = TVSource.LocalAnalog;
            }
        }

        private void sourceBtn_Network_Click(object sender, EventArgs e)
        {
            if (Enabled)
            {
                SelectedSource = TVSource.Network;
            }
        }

        private void sourceBtn_DoubleClick(object sender, EventArgs e)
        {
            if (Enabled)
            {
                FireSelectedSourceChanged();
            }
        }

        /// <summary>
        /// Shows or hides the associated button.
        /// </summary>
        /// <param name="source">Type of TVSource</param>
        /// <param name="available">true if available (show), false if not available (hide)</param>
        public void SetSourceAvailable(TVSource source, bool available)
        {
            GetSourceButton(source).Visible = available;
        }

        #endregion

        #region Selected Input

        [Browsable(true), Category("Action"), Description("Fired when an input button is selected by the user.")]
        public event EventHandler SelectedInputChanged;

        /// <summary>
        /// Fires the SelectedInputChanged if any listeners are attached.
        /// </summary>
        private void FireSelectedInputChanged()
        {
            if (SelectedInputChanged != null)
            {
                SelectedInputChanged(this, new EventArgs());
            }
        }

        private TVMode _selectedInput;
        /// <summary>
        /// Sets or Gets the currently selected input
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("The currently selected input")]
        public TVMode SelectedInput
        {
            get
            {
                return _selectedInput;
            }
            set
            {
                bool actuallyChanging = false;
                if (_selectedInput != value)
                {
                    actuallyChanging = true;
                    _selectedInput = value;
                }

              //  if (!Enabled)
              //  {
                    ResetInputImageIndexes();
               // }
                GetInputButton(value).ImageIndex = 1;

                if (actuallyChanging && Enabled)
                {
                    FireSelectedInputChanged();
                }
            }
        }

        /// <summary>
        /// Returns the button associated with the currently SelectedInput
        /// </summary>
        private FCButton GetInputButton(TVMode input)
        {
            switch (input)
            {
                case TVMode.Broadcast:
                    return sourceInputBtn_Antenna;
                case TVMode.Satellite:
                    return sourceInputBtn_Composite;
            }
            return null;
        }

        private void sourceInputBtn_Antenna_Click(object sender, EventArgs e)
        {
            if (Enabled)
            {
                SelectedInput = TVMode.Broadcast;
            }
        }

        private void sourceInputBtn_Composite_Click(object sender, EventArgs e)
        {
            if (Enabled)
            {
                SelectedInput = TVMode.Satellite;
            }
        }

        #endregion

        private bool _enabled = true;
        [Browsable(true), Category("Behavior"), Description("Gets or sets the Enabled state of the whole control")]
        public new bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                foreach (Control c in tbPanelInputs.Controls)
                {
                    UpdateButtonEnabled(c as FCButton);
                }
                foreach (Control c in tbPanelSources.Controls)
                {
                    UpdateButtonEnabled(c as FCButton);
                }
            }
        }

        /// <summary>
        /// Updates the Enabled state of a button to match this.Enabled.
        /// </summary>
        /// <remarks>
        /// The actual "Enabled" property of the button is not guarunteed to be set "correctly" so you must trap
        /// clicks by checking this.Enabled property.
        /// If the button has an ImageList with the appropriate images, an image is selected to represent its state.
        /// If no image is available, the actual Enabled state of the button will be set to draw it as "grayed out"
        /// </remarks>
        /// <param name="button">the button to set enabled/disabled</param>
        private void UpdateButtonEnabled(FCButton button)
        {
            if (button == null)
            {
                return;
            }

            if (button.ImageList != null)
            {
                if ((button.ImageIndex == 2) && (Enabled))
                    button.ImageIndex = 1;
                else if ((button.ImageIndex == 1) && (!Enabled) && (button.ImageList.Images.Count >= 3))
                    button.ImageIndex = 2;
                else
                    button.Enabled = Enabled;
            }
            else
            {
                button.Enabled = Enabled;
            }
        }
    }
}
