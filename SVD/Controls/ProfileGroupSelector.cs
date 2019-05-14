using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using FutureConcepts.Media.CommonControls;
using FutureConcepts.Media.Client;
using FutureConcepts.Media.Client.StreamViewer;
using FutureConcepts.Tools;

using FutureConcepts.Settings;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class ProfileGroupSelector : UserControl
    {
        public static class ButtonColors
        {
            public static Color Normal = Color.FromArgb(0, 96, 160);
            public static Color Selected = Color.FromArgb(24, 140, 79);
        }

        protected ProfileNegotiationWorker theNegotiator = new ProfileNegotiationWorker();

        public ProfileGroupSelector()
        {
            MediaApplicationSettings settings = new MediaApplicationSettings();
            _username = settings.UserName;

            theNegotiator = new ProfileNegotiationWorker();
            theNegotiator.ChangeComplete += new EventHandler(ProfileNegotiation_ChangeCompleted);

            InitializeComponent();
        }

        private string _username;
        private ulong _customProfileSequenceNumber = 0;
        /// <summary>
        /// Getting this property creates a new custom profile sequence number
        /// </summary>
        private string CustomProfileSequenceNumber
        {
            get
            {
                return _username + (++_customProfileSequenceNumber).ToString();
            }
        }

        private StreamViewerControl _streamViewer;
        /// <summary>
        /// The StreamViewer that this profile group selector is working with
        /// </summary>
        public StreamViewerControl StreamViewer
        {
            get
            {
                return _streamViewer;
            }
            set
            {
                //reset this so that the PGS rebuilds correctly even if it was in a bad state.
                ChangingSelf = false;

                //remove property change listener
                if (_streamViewer != null)
                {
                    _streamViewer.PropertyChanged -= new PropertyChangedEventHandler(StreamViewer_PropertyChanged);
                }

                //set value
                _streamViewer = value;
                theNegotiator.StreamViewer = value;

                if (_streamViewer == null)
                {
                    return;
                }

                //add property change listener
                _streamViewer.PropertyChanged += new PropertyChangedEventHandler(StreamViewer_PropertyChanged);

                try
                {
                    if (StreamViewer.SessionDescription != null)
                    {
                        ProfileGroups = StreamViewer.SessionDescription.ProfileGroups;
                    }

                    if (StreamViewer.State == StreamState.Playing)
                    {
                        LoadProfileGroup(StreamViewer.CurrentProfile.Name);
                    }
                    else
                    {
                        cbProfileGroups.SelectedIndex = -1;
                    }
                    //get latest info about CustomProfile
                    if ((StreamViewer.CurrentProfile != null) && (StreamViewer.CustomProfile != null))
                    {
                        //check if the custom profile is in the same group as the current group
                        if (Profile.GetProfileNameParts(StreamViewer.CurrentProfile.Name)[0]
                             == Profile.GetProfileNameParts(StreamViewer.CustomProfile.Name)[0])
                        {
                            CustomProfile = StreamViewer.CustomProfile;
                        }
                        else
                        {
                            CustomProfile = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                }
                finally
                {
                    ChangingSelf = false;
                }
                
            }
        }

        void StreamViewer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new PropertyChangedEventHandler(StreamViewer_PropertyChanged), new object[] { sender, e });
                return;
            }

            if ((sender == StreamViewer) && (StreamViewer.State == StreamState.Playing))
            {
                switch (e.PropertyName)
                {
                    case "CurrentProfile":
                        if (SelectedProfileGroup == null)
                        {
                            LoadProfileGroup(StreamViewer.CurrentProfile.Name);
                        }
                        else
                        {
                            if (SelectedProfileGroup.Items.Contains(StreamViewer.CurrentProfile))
                            {
                                RefreshCustomProfile();
                                //set selected profile
                                this.SelectedProfile = StreamViewer.CurrentProfile;
                            }
                            else
                            {
                                LoadProfileGroup(StreamViewer.CurrentProfile.Name);

                                if (RefreshCustomProfile())
                                {
                                    this.SelectedProfile = this.CustomProfile;
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// If the StreamViewer's CurrentProfile is a custom profile, it is cached.
        /// </summary>
        /// <returns>Returns true if the current profile is a custom profile, false if not</returns>
        private bool RefreshCustomProfile()
        {
            if (StreamViewer.CurrentProfile.Name.ToLowerInvariant().Contains("custom"))
            {
                //copy custom profile
                this.CustomProfile = StreamViewer.CurrentProfile;
                //associate with current StreamViewer
                StreamViewer.CustomProfile = this.CustomProfile;

                return true;
            }
            return false;
        }

        private ProfileGroups _profileGroups = new ProfileGroups();
        /// <summary>
        /// The list of ProfileGroups currently available to choose from.
        /// </summary>
        protected ProfileGroups ProfileGroups
        {
            get { return _profileGroups; }
            set
            {
                Debug.WriteLine("PGS: ProfileGroups changing");
                _profileGroups = value;

                cbProfileGroups.Items.Clear();
                foreach (ProfileGroup g in ProfileGroups.Items)
                {
                    cbProfileGroups.Items.Add(g.Name ?? "<Unknown>");
                }
                cbProfileGroups.MaxDropDownItems = (cbProfileGroups.Items.Count > 30) ? 30 : cbProfileGroups.Items.Count;
            }
        }

        /// <summary>
        /// Adds a profile to the Profile Group list.
        /// </summary>
        /// <param name="profileGroup">the profile group to add.</param>
        public void AddProfileGroup(ProfileGroup group)
        {
            Debug.Assert(group != null);
            ProfileGroups.Items.Add(group);
            cbProfileGroups.Items.Add((group.Name == null) ? "<Unknown>" : group.Name);
        }

        #region Loading/Selecting of Groups and Profiles

        /// <summary>
        /// Called when the Profile Group combobox is changed
        /// </summary>
        /// <remarks>
        /// Loads the default profile from the newly selected profile group, and sends the change if Enabled.
        /// </remarks>
        private void cbProfileGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCustomProfile.Visible = false;

            if (cbProfileGroups.SelectedIndex == -1)
                return;

            LoadProfileGroup(ProfileGroups.Items[cbProfileGroups.SelectedIndex]);

            if (!ChangingSelf)
            {
                //notify stream viewer of profile group change.
                theNegotiator.ChangeProfileName(SelectedProfile.Name);
            }
        }

        /// <summary>
        /// Loads a profile group into current view, and selects the default profile for that group
        /// </summary>
        /// <param name="group">profile group to load</param>
        private void LoadProfileGroup(ProfileGroup group)
        {
            LoadProfileGroup(group, group.DefaultProfileName);
        }

        /// <summary>
        /// Loads a profile group into current view, and selects the specifed profile.
        /// </summary>
        /// <remarks>
        /// Clears any previously loaded profile group.
        /// </remarks>
        /// <param name="profileToSelect">
        /// The profile to select while loading the profile group. If fully qualified, will select group and profile.
        /// If relative, then the profile is assumed to be in the current group.
        /// </param>
        public void LoadProfileGroup(string profileToSelect)
        {
            bool oldEnabled = this.Enabled;
            this.Enabled = false;
            if (ChangingSelf)
            {
                return;
            }

            string[] parts = Profile.GetProfileNameParts(profileToSelect);
            int groupIndex = cbProfileGroups.SelectedIndex;
            string profileNameToSelect = (parts.Length == 2) ? parts[1] : profileToSelect;

            //if we need to change groups
            if (parts.Length == 2)
            {
                groupIndex = IndexOfProfileGroup(parts[0]);
                if (groupIndex == -1)
                {
                    throw new ArgumentException("The requested profile group is not in the session description", "profileToSelect");
                }

                if (cbProfileGroups.SelectedIndex != groupIndex)
                {
                    ChangingSelf = true;
                    btnCustomProfile.Visible = false;
                    cbProfileGroups.SelectedIndex = groupIndex;
                    ChangingSelf = false;
                }
            }

            LoadProfileGroup(ProfileGroups.Items[groupIndex], profileNameToSelect);

            this.Enabled = oldEnabled;
        }

        /// <summary>
        /// Loads all the buttons for a specified profile group.
        /// </summary>
        /// <param name="g">group to load</param>
        /// <param name="profileNameToSelect">a RELATIVE profile name</param>
        private void LoadProfileGroup(ProfileGroup group, string profileNameToSelect)
        {
            ClearProfileButtons();

            int lowestBitrate = Int32.MaxValue;
            int highestBitrate = 1024;

            string fqProfileName = group.Name + ":" + profileNameToSelect;

            //Since the buttons are Dock'ed Top, we need to add them in reverse order
            //so that they appear in the correct order.
            for (int i = group.Items.Count - 1; i >= 0; i--)
            {
                lowestBitrate = Math.Min(lowestBitrate, GetLowestVideoBitrate(group[i]));
                highestBitrate = Math.Max(highestBitrate, GetHighestVideoBitrate(group[i]));

                //set acceptable ranges for the custom controls, as currently known.
                //this makes sure that the sliders will always be set correctly
                tbBitrate.Maximum = BitrateToExponent(highestBitrate);
                tbBitrate.Minimum = BitrateToExponent(lowestBitrate);

                if (group[i].Name == fqProfileName)
                {
                    SelectedProfileButton = AddProfile(group[i]);
                }
                else
                {
                    AddProfile(group[i]);
                }
            }
//            if (group.CustomProfileEnabled)
//            {
//                pTrackbars.Visible = true;
//                //take care of custom profile
//                if (RefreshCustomProfile())
//                {
//                    this.SelectedProfile = StreamViewer.CustomProfile;
//                }
//            }
//            else
            {
                CustomProfile = null;
                StreamViewer.CustomProfile = null;
                pTrackbars.Visible = false;
            }
        }

        #endregion

        #region Profile Buttons

        /// <summary>
        /// Clears all the profile buttons and makes sure that all references to the buttons are null
        /// </summary>
        private void ClearProfileButtons()
        {
            foreach (Control c in pProfileButtons.Controls)
            {
                RedFlatButton button = c as RedFlatButton;
                if (button != null)
                {
                    button.Click -= new EventHandler(ProfileButton_Click);
                }
            }
            pProfileButtons.Controls.Clear();
            SelectedProfileButton = null;
            CustomProfile = null;
        }

        /// <summary>
        /// Adds a profile to the profile button list
        /// </summary>
        /// <param name="p">profile to add</param>
        /// <returns>
        /// returns the button used to select the profile specified.
        /// </returns>
        protected Button AddProfile(Profile p)
        {
            RedFlatButton b = new RedFlatButton();
            b.Text = Profile.GetProfileNameParts(p.Name)[1];
            b.BackColor = ButtonColors.Normal;
            b.Dock = DockStyle.Top;

            b.Click += new EventHandler(ProfileButton_Click);

            pProfileButtons.Controls.Add(b);

            return b;
        }

        /// <summary>
        /// When a profile button is clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProfileButton_Click(object sender, EventArgs e)
        {
            Button pb = sender as Button;
            if (pb == null)
            {
                return;
            }

            if (pb == SelectedProfileButton)
            {
                return;
            }

            SelectedProfileButton = pb;
            this.Enabled = false;
        }

        #endregion

        #region Accessors

        private Button _selectedProfileButton = null;
        /// <summary>
        /// This property allows you to set or get the selected profile button
        /// </summary>
        protected Button SelectedProfileButton
        {
            get { return _selectedProfileButton; }
            set
            {
                try
                {
                    if (value == btnCustomProfile)
                    {
                        if ((CustomProfile == null) && (SelectedProfile != null))
                        {
                            CustomProfile = SelectedProfile;
                        }
                        btnCustomProfile.Visible = true;
                    }

                    //de-select old button
                    if (SelectedProfileButton != null)
                    {
                        SelectedProfileButton.BackColor = ButtonColors.Normal;
                    }

                    //update value
                    _selectedProfileButton = value;
                    if (_selectedProfileButton == null)
                    {
                        return;
                    }

                    //select the button
                    _selectedProfileButton.BackColor = ButtonColors.Selected;

                    //propogate changes to the server if neccesary
                    if ((!ChangingSelf) && Enabled)
                    {
                        this.Enabled = false;
                        if (SelectedProfile != CustomProfile)
                        {
                            theNegotiator.ChangeProfileName(SelectedProfile.Name);
                        }
                        else
                        {
                            theNegotiator.ChangeCustomProfile(SelectedProfile);
                        }
                    }

                    //in some circumstances it is possible that the profile will become null,
                    //--this can happen if the profile groups are changing, or an update from
                    //  the server arrived at a weird time
                    if (this.SelectedProfile == null)
                    {
                        return;
                    }

                    //update the display values, if not changing self
                    if ((!ChangingSelf) && (SelectedProfile.Video != null))
                    {
                        bool lastEnabled = Enabled;
                        Enabled = false;

                        if (SelectedProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerSecond)
                        {
                            tbFramerate.CustomTicks = ValidFrameRates.FramesPerSecond;
                        }
                        else if (SelectedProfile.Video.FrameRateUnits == VideoFrameRateUnits.FramesPerMinute)
                        {
                            tbFramerate.CustomTicks = ValidFrameRates.FramesPerMinute;
                        }
                        tbFramerate.Maximum = tbFramerate.CustomTicks[tbFramerate.CustomTicks.Length - 1];

                        DisplayBitrate(SelectedProfile.Video.ConstantBitRate);
                        BitrateEnabled = !CodecBitrateAdjustmentDisabled(SelectedProfile.Video.CodecType);

                        DisplayFramerate(SelectedProfile.Video.FrameRate);
                        FramerateEnabled = true;

                        Enabled = lastEnabled;
                    }
                    else if (SelectedProfile.Video == null)  //TODO when Audio is implemented, account for that too
                    {
                        FramerateEnabled = false;
                        BitrateEnabled = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.DumpToDebug(ex);
                    //it is not clear what circumstances lead to this, but probably we are broken
                    this.Visible = false;
                }
            }
        }

        /// <summary>
        /// sets or gets the currently selected profile group
        /// </summary>
        public ProfileGroup SelectedProfileGroup
        {
            get
            {
                if (cbProfileGroups.SelectedIndex == -1)
                {
                    //throw new Exception("PGS: there is no SelectedProfileGroup!");
                    return null;
                }

                return ProfileGroups.Items[cbProfileGroups.SelectedIndex];
            }

            set
            {
                if (value == null)
                {
                    cbProfileGroups.SelectedIndex = -1;
                }
                else
                {
                    int index = ProfileGroups.Items.IndexOf(value);
                    if (index > -1)
                    {
                        cbProfileGroups.SelectedIndex = index;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Returns the Selected Profile.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if there is no selected profile.
        /// If you set the profile via this property, it will attempt to select a profile that is already in the list. -- O(n)
        /// </remarks>
        public Profile SelectedProfile
        {
            get
            {
                if (SelectedProfileButton != null)
                {
                    //handle the custom profile
                    if (SelectedProfileButton == btnCustomProfile)
                    {
                        return btnCustomProfile.Tag as Profile;
                    }
                    //else get the appropriate profile
                    else
                    {
                        try
                        {
                            return SelectedProfileGroup[SelectedProfileButton.Text];
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.DumpToDebug(ex);
                        }
                    }
                }

                return null;
            }
            set
            {
                //don't waste our time if the profiles are the same
                if (SelectedProfile == value)
                {
                    return;
                }

                if ((value == null) && (SelectedProfileButton != null))
                {
                    SelectedProfileButton.BackColor = ButtonColors.Normal;
                    SelectedProfileButton = null;
                }
                else if(value == CustomProfile)
                {
                    SelectedProfileButton = btnCustomProfile;
                }
                else if(value != null)
                {
                    string justProfileName = Profile.GetProfileNameParts(value.Name)[1];
                    foreach (Control c in pProfileButtons.Controls)
                    {
                        Button b = c as Button;
                        if (b.Text == justProfileName)
                        {
                            SelectedProfileButton = b;
                            return;
                        }
                    }
                    SelectedProfileButton = null;
                }
            }
        }

        /// <summary>
        /// gets or sets the CustomProfile
        /// When setting the CustomProfile, it is automatically cloned and has " Custom" appended to its name
        /// </summary>
        /// <remarks>
        /// Setting the CustomProfile to null causes the Custom button to become invisible, setting it to non-null causes it to be visible.
        /// 
        /// </remarks>
        public Profile CustomProfile
        {
            get
            {
                return btnCustomProfile.Tag as Profile;
            }
            set
            {
                if (value != null)
                {
                    btnCustomProfile.Tag = value.Clone();

                    if (CustomProfile.Name == null)
                    {
                        CustomProfile.Name = "Custom " + CustomProfileSequenceNumber;
                    }
                    else if (!CustomProfile.Name.ToLowerInvariant().Contains("custom"))
                    {
                        CustomProfile.Name += " Custom " + CustomProfileSequenceNumber;
                    }
                    else
                    {
                        int indexOfCustom = CustomProfile.Name.ToLowerInvariant().LastIndexOf("custom");
                        if(indexOfCustom > -1)
                        {
                            CustomProfile.Name = CustomProfile.Name.Substring(0, indexOfCustom + 7) + CustomProfileSequenceNumber;
                        }
                    }
                }
                else
                {
                    btnCustomProfile.Tag = null;
                }
                btnCustomProfile.Visible = (value != null);
            }
        }

        /// <summary>
        /// Determines the index of a given Profile Group based on its name
        /// </summary>
        /// <param name="groupName">name of group to find</param>
        /// <returns>The index of the profile group, or -1 if not found</returns>
        private int IndexOfProfileGroup(string groupName)
        {
            for (int i = 0; i < ProfileGroups.Items.Count; i++)
            {
                if (ProfileGroups.Items[i].Name == groupName)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// gets the lowest video bitrate found in this profile
        /// </summary>
        /// <param name="profile">profile to get lowest from</param>
        /// <returns>
        /// Returns Int32.MaxValue
        ///     -if profile doesn't contain a VideoSettings
        ///     -if VideoCodecType == MJ2K
        /// Otherwise returns the VBR Min, or the constant bitrate
        /// </returns>
        private int GetLowestVideoBitrate(Profile profile)
        {
            if (profile.Video == null)
            {
                return Int32.MaxValue;
            }

            if (CodecBitrateAdjustmentDisabled(profile.Video.CodecType))
            {
                return Int32.MaxValue;
            }

            if (profile.Video.VBR != null)
            {
                return profile.Video.VBR.MinBitRate;
            }
            else
            {
                return profile.Video.ConstantBitRate;
            }
        }

        /// <summary>
        /// gets the highest video bitrate found in this profile
        /// </summary>
        /// <param name="profile">profile to get highest from</param>
        /// <returns>
        /// Returns Int32.MinValue
        ///     -if profile doesn't contain a VideoSettings
        ///     -if VideoCodecType == MJ2K
        /// Otherwise returns the VBR Max, or the constant bitrate
        /// </returns>
        private int GetHighestVideoBitrate(Profile profile)
        {
            if (profile.Video == null)
            {
                return Int32.MinValue;
            }

            if (CodecBitrateAdjustmentDisabled(profile.Video.CodecType))
            {
                return Int32.MinValue;
            }

            if (profile.Video.VBR != null)
            {
                return profile.Video.VBR.MaxBitRate;
            }
            else
            {
                return profile.Video.ConstantBitRate;
            }
        }

        private delegate void SetEnabledDelegate(bool enabled);

        /// <summary>
        /// this function is used for cross-thread calls to the Enabled property.
        /// </summary>
        /// <param name="enabled"></param>
        private void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        private bool _bitrateEnabled;
        private bool BitrateEnabled
        {
            get
            {
                return _bitrateEnabled;
            }
            set
            {
                _bitrateEnabled = value;
                if (!value)
                {
                    lblBitrateKBPS.Text = "n/a";
                }
            }
        }

        private bool _framerateEnabled;
        private bool FramerateEnabled
        {
            get
            {
                return _framerateEnabled;
            }
            set
            {
                _framerateEnabled = value;
                if (!value)
                {
                    lblFramerateFPS.Text = "n/a";
                }
            }
        }

        /// <summary>
        /// Enables or disables all of the user controls used to select profiles.
        /// </summary>
        /// <param name="enable"></param>
        public new bool Enabled
        {
            get
            {
                return cbProfileGroups.Enabled;
            }
            set
            {
                cbProfileGroups.Enabled = value;

                foreach (Control c in pProfileButtons.Controls)
                {
                    c.Enabled = value;
                }

                btnCustomProfile.Enabled = value;

                tbBitrate.Enabled = value && BitrateEnabled;
                tbFramerate.Enabled = value && FramerateEnabled;
            }
        }

        #endregion

        #region Profile Negotiation

        /// <summary>
        /// invoked when a profile negitiation change is compeleted.
        /// </summary>
        protected void ProfileNegotiation_ChangeCompleted(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetEnabledDelegate(SetEnabled), new object[1] { true });
            }
            else
            {
                this.Enabled = true;
            }
            //handle error
            if (theNegotiator.LastCompletedChangeArgs.Result is Exception)
            {
                ErrorLogger.DumpToDebug((Exception)theNegotiator.LastCompletedChangeArgs.Result);
                //do something to re-get the server's profile settings.
            }
        }

        /// <summary>
        /// This class handles server requests for a given StreamViewer.StreamViewer in the background.
        /// </summary>
        protected class ProfileNegotiationWorker
        {
            private BackgroundWorker profileChangeWorker;

            public ProfileNegotiationWorker()
            {
                profileChangeWorker = new BackgroundWorker();
                profileChangeWorker.DoWork += new DoWorkEventHandler(profileChangeWorker_DoWork);
                profileChangeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
                profileChangeWorker.WorkerReportsProgress = false;
            }

            private RunWorkerCompletedEventArgs _lastCompeletedEventArgs = null;
            /// <summary>
            /// The RunWorkerCompletedEventArgs for the last completed change
            /// </summary>
            public RunWorkerCompletedEventArgs LastCompletedChangeArgs
            {
                get
                {
                    return _lastCompeletedEventArgs;
                }
                private set
                {
                    _lastCompeletedEventArgs = value;
                }
            }

            /// <summary>
            /// Called whenever a sub-worker is completed.
            /// </summary>
            private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                Debug.WriteLine("PGS.PNW: RunWorkerCompleted: ");
                if (e.Error != null)
                {
                    ErrorLogger.DumpToDebug(e.Error);
                }

                LastCompletedChangeArgs = e;
                if (ChangeComplete != null)
                {
                    ChangeComplete(this, new System.EventArgs());
                }
            }

            /// <summary>
            /// Does the work for changing the profile name
            /// </summary>
            private void profileChangeWorker_DoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    Profile newProfile = null;
                    if (e.Argument is string)
                    {
                        newProfile = new Profile();
                        newProfile.Name = e.Argument as string;
                    }
                    else if (e.Argument is Profile)
                    {
                        newProfile = e.Argument as Profile;
                    }
                    else
                    {
                        throw new ArgumentException(@"Unrecognized type. Don't know what to do.", "e.Argument");
                    }

                    StreamViewer.SetProfileOnServer(newProfile);
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                }
            }

            private StreamViewerControl _sv;
            /// <summary>
            /// The StreamViewer that we are negiotiating through/for
            /// </summary>
            public StreamViewerControl StreamViewer
            {
                get { return _sv; }
                set { _sv = value; }
            }

            /// <summary>
            /// Changes the Profile Name on the currently selected <c>StreamViewer</c>
            /// </summary>
            /// <param name="fullyQualifiedProfileName">profile name</param>
            public void ChangeProfileName(string fullyQualifiedProfileName)
            {
                if (!profileChangeWorker.IsBusy)
                {
                    Debug.WriteLine("PGS.PNW: ChangeProfileName: " + fullyQualifiedProfileName);
                    profileChangeWorker.RunWorkerAsync(fullyQualifiedProfileName);
                }
            }

            /// <summary>
            /// Changes to the specified custom profile on the currently selected <c>StreamViewer</c>
            /// </summary>
            /// <param name="custom">custom profile to select</param>
            public void ChangeCustomProfile(Profile custom)
            {
                if (!profileChangeWorker.IsBusy)
                {
                    Debug.WriteLine("PGS.PNW: ChangeCustomProfile: " + custom.Name);
                    profileChangeWorker.RunWorkerAsync(custom);
                }
            }
            
            /// <summary>
            /// This event is fired whenever a change is compeleted.
            /// </summary>
            public event EventHandler ChangeComplete;
        }

        #endregion

        #region Custom Controls

        private bool _customControlsChanging;
        /// <summary>
        /// This property is used to determine when the custom button should update the controls to show it's values,
        /// or when it's controls are changing.
        /// </summary>
        public bool ChangingSelf
        {
            get { return _customControlsChanging; }
            set { _customControlsChanging = value; }
        }

        /// <summary>
        /// Called when the framerate slider's value has changed
        /// </summary>
        private void tbFramerate_ValueChanged(object sender, EventArgs e)
        {
            VideoFrameRateUnits units = VideoFrameRateUnits.FramesPerSecond;
            if (SelectedProfile != null)
            {
                if (SelectedProfile.Video != null)
                {
                    units = SelectedProfile.Video.FrameRateUnits;
                }
            }
            UpdateFramerateLabel((int)tbFramerate.Value, units);

            if ((SelectedProfileButton == btnCustomProfile) && (Enabled))
            {
                CustomProfile.Video.FrameRate = (int)tbFramerate.Value;
                NegotiateCustomProfile();
            }
        }

        /// <summary>
        /// Called when the bitrate slider's value has changed.
        /// </summary>
        private void tbBitrate_ValueChanged(object sender, EventArgs e)
        {
            UpdateBitrateLabel(ExponentToBitrate(tbBitrate.Value));

            if ((SelectedProfileButton == btnCustomProfile) && (Enabled))
            {
                CustomProfile.Video.ConstantBitRate = ExponentToBitrate(tbBitrate.Value);
                NegotiateCustomProfile();
            }
        }

        /// <summary>
        /// Called while the bitrate slider is being dragged.
        /// </summary>
        /// <remarks>
        /// Activates the custom profile
        /// </remarks>
        private void tbBitrate_Scroll(object sender, EventArgs e)
        {
            UpdateBitrateLabel(ExponentToBitrate(tbBitrate.CurrentValue));

            if (SelectedProfileButton != btnCustomProfile)
            {
                CreateCustomProfile();
            }
        }

        /// <summary>
        /// Called while the framerate slider is being dragged.
        /// </summary>
        /// <remarks>
        /// Activates the custom profile.
        /// </remarks>
        private void tbFramerate_Scroll(object sender, EventArgs e)
        {
            VideoFrameRateUnits units = VideoFrameRateUnits.FramesPerSecond;
            if (SelectedProfile != null)
            {
                if (SelectedProfile.Video != null)
                {
                    units = SelectedProfile.Video.FrameRateUnits;
                }
            }
            UpdateFramerateLabel((int)tbFramerate.CurrentValue, units);

            if (SelectedProfileButton != btnCustomProfile)
            {
                CreateCustomProfile();
            }
        }

        /// <summary>
        /// Sets CustomProfile to a profile that represents the settings of a given StreamViewerControl
        /// </summary>
        /// <param name="sv">StreamViewerControl to get settings from</param>
  /*      public void BuildCustomProfile(StreamViewerControl sv)
        {
            if (sv == null)
            {
                throw new ArgumentNullException();
            }

            //create a new profile for the CurrentVideoSettings
            Profile current = new Profile();
            current.Name = sv.CurrentProfile.Name;
            current.Video = sv.CurrentProfile.Video;

            CustomProfile = current;
        }*/

        /// <summary>
        /// Starts the creation of a new Custom Profile, based on the SelectedProfile.
        /// Call this method when a custom-parameter is changing
        /// </summary>
        private void CreateCustomProfile()
        {
            ChangingSelf = true;
            CustomProfile = SelectedProfile;
            SelectedProfileButton = btnCustomProfile;
        }

        /// <summary>
        /// Sends the custom profile information to the server
        /// </summary>
        private void NegotiateCustomProfile()
        {
            if (!Enabled)
            {
                return;
            }
            ChangingSelf = false;
            this.Enabled = false;
            theNegotiator.ChangeCustomProfile(CustomProfile);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Determines if the specified codec supports adjusting the bitrate
        /// </summary>
        /// <param name="codec">codec to check</param>
        /// <returns>true if the codec cannot have its bitrate adjusted, false if bitrate adjustment is allowed.</returns>
        private bool CodecBitrateAdjustmentDisabled(VideoCodecType codec)
        {
            return ((codec == VideoCodecType.MJPEG) ||
                    (codec == VideoCodecType.MJPEG2000));
        }

        /// <summary>
        /// Makes the bitrate slider display the specified bitrate
        /// </summary>
        /// <param name="kbps">the bitrate to display</param>
        protected void DisplayBitrate(int kbps)
        {
            tbBitrate.Value = BitrateToExponent(kbps);
        }

        /// <summary>
        /// Converts a bitrate into an exponent of 2 such that 2^x = kbps
        /// </summary>
        /// <param name="kbps">the answer to 2^x = kbps</param>
        /// <returns>the value of x such that 2^x = kbps</returns>
        protected double BitrateToExponent(int kbps)
        {
            if (kbps < 1)
            {
                return 0;
            }

            return Math.Log((double)kbps) / Math.Log(2);
        }

        /// <summary>
        /// Converts an exponent into a bitrate, using the formula 2^x = kbps
        /// </summary>
        /// <param name="exponent">exponent of 2</param>
        /// <returns>2^<paramref>exponent</paramref></returns>
        protected int ExponentToBitrate(double exponent)
        {
            return (int)Math.Pow(2, exponent);
        }

        /// <summary>
        /// Makes the framerate slider display the specified framerate
        /// </summary>
        /// <param name="frames">framerate to display</param>
        protected void DisplayFramerate(int frames)
        {
            tbFramerate.Value = (double)frames;
        }

        /// <summary>
        /// Updates the framerate fps read-out
        /// </summary>
        /// <param name="frames">value to show in text</param>
        private void UpdateFramerateLabel(int frames, VideoFrameRateUnits units)
        {
            lblFramerateFPS.Text = frames.ToString("0 " + ((units == VideoFrameRateUnits.FramesPerMinute) ? "fpm" : "fps"));
        }

        /// <summary>
        /// Updates the bitrate kbps read-out
        /// </summary>
        /// <param name="kbps">value to show in text</param>
        private void UpdateBitrateLabel(int kbps)
        {
            lblBitrateKBPS.Text = kbps.ToString("0 kbps");
        }

        #endregion
    }
}
