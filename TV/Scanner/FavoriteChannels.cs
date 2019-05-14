using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FutureConcepts.Controls.AntaresX.AntaresXForms;
using System.Threading;
using FutureConcepts.Tools;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using FutureConcepts.Media.TV.Scanner.Config;

namespace FutureConcepts.Media.TV.Scanner
{
    public partial class FavoriteChannels : UserControl
    {
        public FavoriteChannels()
        {
            InitializeComponent();
            cbInterval.SelectedIndex = 2;
        }

        private Dictionary<TVSource, ChannelCollection> _allFavorites = new Dictionary<TVSource, ChannelCollection>();
        /// <summary>
        /// Contains all cached favorites for all sources
        /// </summary>
        private Dictionary<TVSource, ChannelCollection> AllFavorites
        {
            get
            {
                return _allFavorites;
            }
            set
            {
                _allFavorites = value;
            }
        }

        /// <summary>
        /// The list of current favorites, based on the CurrentTVSource property
        /// </summary>
        private ChannelCollection CurrentFavorites
        {
            get
            {
                try
                {
                    ChannelCollection cur = AllFavorites[CurrentTVSource];
                    if (cur == null)
                    {
                        cur = new ChannelCollection();
                    }
                    return cur;
                }
                catch (Exception)
                {
                    return new ChannelCollection();
                }
            }
        }
/*
        private bool _enabled;
        public new bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                foreach (FavoriteChannelThumbnail t in panelThumbnails.Controls)
                {
                    t.Enabled = value;
                }
                panelControls.Enabled = value;
            }
        }
        */
        #region [Public API]

        #region [API Events]

        [Category("Action"), Browsable(true), Description("Fired when the user clicks the Add Favorite Channel button. Application should respond by calling AddFavorite")]
        public event EventHandler AddCurrentAsFavorite;

        public delegate void TuneFavoriteChannelHandler(object sender, Channel channel);

        [Category("Action"), Browsable(true), Description("Fired when a favorite should be restored.")]
        public event TuneFavoriteChannelHandler TuneFavoriteChannel;
        /// <summary>
        /// Raises the TuneFavoriteChannel event
        /// </summary>
        /// <param name="channel">channel to tune</param>
        private void FireTuneFavoriteChannel(Channel channel)
        {
            if (TuneFavoriteChannel != null)
            {
                TuneFavoriteChannel(this, channel);
            }
        }

        [Category("Action"), Browsable(true), Description("Fired when the Scanning property has changed")]
        public event EventHandler ScanningStateChanged;
        /// <summary>
        /// Raises the ScanningStateChanged event.
        /// </summary>
        private void FireScanningStateChanged()
        {
            if (ScanningStateChanged != null)
            {
                ScanningStateChanged(this, new EventArgs());
            }
        }

        #endregion

        private TVSource _currentTvSource;
        /// <summary>
        /// The current TV Source.
        /// </summary>
        public TVSource CurrentTVSource
        {
            private get
            {
                return _currentTvSource;
            }
            set
            {
                _currentTvSource = value;

                if (!AllFavorites.ContainsKey(value))
                {
                    AllFavorites.Add(value, new ChannelCollection());
                }

                RecreateThumbnails();
                UpdateUIControls();
            }
        }

        private Channel _currentChannel;
        /// <summary>
        /// The currently tuned channel
        /// </summary>
        public Channel Channel
        {
            get
            {
                return _currentChannel;
            }
            set
            {
                _currentChannel = value;
                ClearHighlights();

                if (value != null)
                {
                    int i = CurrentFavorites.IndexOf(value);
                    if (i > -1)
                    {
                        FavoriteChannelThumbnail thumb = GetThumbnailAt(i);
                        if (thumb != null)
                        {
                            thumb.Highlighted = true;
                            panelThumbnails.ScrollControlIntoView(thumb);
                        }
                    }

                    btnAddFavorite.Enabled = (i < 0);

                }
            }
        }

        /// <summary>
        /// Makes the UI controls reflect the current state of the control
        /// </summary>
        private void UpdateUIControls()
        {
            btnAddFavorite.Enabled = !CurrentFavorites.Contains(this.Channel);
            btnClear.Enabled = CurrentFavorites.Count > 0;
            btnScan.Enabled = btnClear.Enabled;
            cbInterval.Enabled = btnClear.Enabled;
        }

        /// <summary>
        /// Adds the given channel as a favorite.
        /// </summary>
        /// <param name="channel">channel to add</param>
        /// <param name="snapshot">Snapshot to display. Set to null to load from disk (if available)</param>
        public void Add(Channel channel, Snapshot snapshot)
        {
            try
            {
                int i = CurrentFavorites.Add(channel);
                if (i == -1)
                {
                    UpdateSnapshot(channel, snapshot);
                    return;
                }

                InsertThumbnail(i, CreateThumbnail(channel, snapshot));

                UpdateUIControls();
                //if the channel being added is logically equal to the one we're being tuned to
                //auto-highlight it.
                if (channel.CompareTo(Channel) == 0)
                {
                    this.Channel = channel; //invoke the setter
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }

        }

        /// <summary>
        /// Updates the snapshot for a given channel.
        /// Does nothing if the channel does not exist.
        /// Does nothing if the snapshot is null.
        /// </summary>
        /// <param name="channel">channel to update</param>
        /// <param name="snapshot">new snapshot to display</param>
        public void UpdateSnapshot(Channel channel, Snapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            FavoriteChannelThumbnail t = GetThumbnailAt(CurrentFavorites.IndexOf(channel));
            if (t != null)
            {
                t.Image = snapshot.DIBBitmap;
            }
        }

        /// <summary>
        /// Removes a channel from the CurrentFavorites collection
        /// </summary>
        /// <param name="channel">channel to remove</param>
        public void Remove(Channel channel)
        {
            RemoveThumbmnnail(GetThumbnailAt(CurrentFavorites.IndexOf(channel)));

            CurrentFavorites.Remove(channel);

            UpdateUIControls();
        }

        /// <summary>
        /// Clears all of the currently displayed favorites
        /// </summary>
        public void Clear()
        {
            this.Scanning = false;

            foreach (FavoriteChannelThumbnail ct in panelThumbnails.Controls)
            {
                ReleaseThumbnail(ct);
            }
            panelThumbnails.Controls.Clear();
            panelThumbnails.RowCount = 0;
            CurrentFavorites.Clear();

            UpdateUIControls();
        }


        /// <summary>
        /// Returns true if the given channel is a favorite.
        /// </summary>
        /// <param name="channel">channel to check</param>
        /// <returns>Returns true if the logical channel is a favorite, false if not.</returns>
        public bool IsFavorite(Channel channel)
        {
            return CurrentFavorites.Contains(channel);
        }

        #endregion

        #region [Channel Scanning]

        private bool _scanning;
        /// <summary>
        /// Sets or Gets the favorites scanning status. Set to true to commence scanning, set to false to stop scanning
        /// </summary>
        public bool Scanning
        {
            get
            {
                return _scanning;
            }
            set
            {
                if (_scanning != value)
                {
                    _scanning = value;
                    if (value)
                    {
                        StartScanning();
                    }
                    else
                    {
                        StopScanning();
                    }

                    FireScanningStateChanged();
                }
            }
        }

        /// <summary>
        /// Starts the channel scannning cycle
        /// </summary>
        private void StartScanning()
        {
            curScanIndex = -1;
            timerScanCycle_Tick(this, new EventArgs());
            timerScanCycle.Interval = Convert.ToInt32(cbInterval.SelectedItem) * 1000;
            timerScanCycle.Enabled = true;
            btnScan.Tag = "Stop";
            tt.SetToolTip(btnScan, "Click to stop scanning favorite channels.");
            btnScan.ImageIndex = 1;
        }

        /// <summary>
        /// Stops the channel scanning cycle
        /// </summary>
        private void StopScanning()
        {
            timerScanCycle.Enabled = false;
            btnScan.Tag = "Scan";
            tt.SetToolTip(btnScan, "Click to begin scanning favorite channels.");
            btnScan.ImageIndex = 0;
        }

        private int curScanIndex;

        private void timerScanCycle_Tick(object sender, EventArgs e)
        {
            try
            {
                if (CurrentFavorites.Count < 1)
                {
                    this.Scanning = false;
                    return;
                }

                timerScanCycle.Enabled = false;

                curScanIndex++;
                //loop back around
                if (curScanIndex > CurrentFavorites.Count - 1)
                {
                    curScanIndex = 0;
                }

                FireTuneFavoriteChannel(CurrentFavorites.Items[curScanIndex]);
                timerScanCycle.Enabled = true;
            }
            catch (Exception exc)
            {
                ErrorLogger.DumpToDebug(exc);
            }
        }

        #endregion

        #region [Thumbnail Control Helpers]

        /// <summary>
        /// Creates a thumbnail
        /// </summary>
        /// <param name="channel">channel to bind to</param>
        /// <param name="snapshot">snapshot to display. Specify null to try to load from memory/disk</param>
        private FavoriteChannelThumbnail CreateThumbnail(Channel channel, Snapshot snapshot)
        {
            FavoriteChannelThumbnail t = new FavoriteChannelThumbnail();
            t.Channel = channel;
            if (snapshot != null)
            {
                t.Image = snapshot.DIBBitmap;
            }
            else
            {
                t.LoadThumbnailImage(FavoritesStoreRoot + "\\" + CurrentTVSource.ToString());
            }
            t.SnapshotClick += new EventHandler(Item_SnapshotClick);
            t.DeleteClick += new EventHandler(Item_DeleteClick);
            return t;
        }

        /// <summary>
        /// releases a thumbnail so it can be removed from the Controls collection and actually get disposed
        /// </summary>
        /// <param name="thumb">thumbnail to release</param>
        private void ReleaseThumbnail(FavoriteChannelThumbnail thumb)
        {
            thumb.SnapshotClick -= new EventHandler(Item_SnapshotClick);
            thumb.DeleteClick -= new EventHandler(Item_DeleteClick);
            thumb.Dispose();
        }

        /// <summary>
        /// Inserts a thumbnail control into the list at a given positon
        /// </summary>
        /// <param name="i">index to insert at</param>
        /// <param name="thumb">thumbnail to insert</param>
        private void InsertThumbnail(int i, FavoriteChannelThumbnail thumb)
        {
            //hide control to avoid repainting
            panelThumbnails.Visible = false; 

            //Insert new RowStyles corresponding to new row
            RowStyle newRow = new RowStyle();
            newRow.SizeType = SizeType.AutoSize;
            panelThumbnails.RowStyles.Insert(i, newRow);

            panelThumbnails.RowCount++;

            //Shift existing controls down
            foreach (Control existing in panelThumbnails.Controls)
            {
                if (panelThumbnails.GetRow(existing) >= i)
                {
                    panelThumbnails.SetRow(existing, panelThumbnails.GetRow(existing) + 1);
                }
            }

            //insert thumbnail in the correct position
            panelThumbnails.Controls.Add(thumb, 0, i);

            //repaint
            panelThumbnails.Visible = true;
            panelThumbnails.Invalidate();
        }

        /// <summary>
        /// Removes the thumbnail specified
        /// </summary>
        /// <param name="thumb">index of the channel being removed</param>
        private void RemoveThumbmnnail(FavoriteChannelThumbnail thumb)
        {
            if(thumb == null)
            {
                return;
            }

            panelThumbnails.Visible = false;

            int removeAt = panelThumbnails.GetRow(thumb);
            panelThumbnails.Controls.Remove(thumb);

            //Shift existing controls down
            foreach (Control existing in panelThumbnails.Controls)
            {
                if (panelThumbnails.GetRow(existing) >= removeAt)
                {
                    panelThumbnails.SetRow(existing, panelThumbnails.GetRow(existing) - 1);
                }
            }

            panelThumbnails.RowCount--;

            //repaint
            panelThumbnails.Visible = true;
            panelThumbnails.Invalidate();
        }

        /// <summary>
        /// Returns the desired thumbnail for the given channel index
        /// </summary>
        /// <param name="i">index to get</param>
        /// <returns>the thumbnail at that channel index. Returns null if not found</returns>
        private FavoriteChannelThumbnail GetThumbnailAt(int i)
        {
            foreach (Control c in panelThumbnails.Controls)
            {
                if (panelThumbnails.GetRow(c) == i)
                {
                    return c as FavoriteChannelThumbnail;
                }
            }
            return null;
        }

        /// <summary>
        /// Recreates all thumbnails for the CurrentFavorites
        /// </summary>
        /// <remarks>
        /// When the thumbnails are created this way, it causes their images to be loaded from disk
        /// </remarks>
        private void RecreateThumbnails()
        {
            foreach (FavoriteChannelThumbnail t in panelThumbnails.Controls)
            {
                ReleaseThumbnail(t);
            }
            panelThumbnails.Controls.Clear();

            for (int i = 0; i < CurrentFavorites.Count; i++)
            {
                InsertThumbnail(i, CreateThumbnail(CurrentFavorites.Items[i], null));
            }

            //update UI
            this.Channel = this.Channel;
            UpdateUIControls();
        }

        /// <summary>
        /// Clears all of the highlights from all of the controls
        /// </summary>
        private void ClearHighlights()
        {
            foreach (FavoriteChannelThumbnail ctl in panelThumbnails.Controls)
            {
                ctl.Highlighted = false;
            }
        }

        #endregion

        #region [UI Event Handlers]

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (FCYesNoMsgBox.ShowDialog("Clear favorites", "Are you sure you want to clear your list of favorite channels?", new Size(380, 125), this) == DialogResult.Yes)
            {
                Clear();
            }
        }

        private void btnAddFavorite_Click(object sender, EventArgs e)
        {
            if (AddCurrentAsFavorite != null)
            {
                AddCurrentAsFavorite(this, new EventArgs());
            }
        }

        private void cbInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            timerScanCycle.Interval = Convert.ToInt32(cbInterval.SelectedItem) * 1000;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            this.Scanning = btnScan.Tag.ToString() != "Stop";
        }

        /// <summary>
        /// Called when the user clicks on the snapshot. This causes the favorite to be tuned.
        /// </summary>
        private void Item_SnapshotClick(object sender, EventArgs e)
        {
            FavoriteChannelThumbnail t = sender as FavoriteChannelThumbnail;
            if (t == null)
            {
                return;
            }

            this.Scanning = false;
            FireTuneFavoriteChannel(t.Channel);
        }

        /// <summary>
        /// Called when the user clicks on the "X" button on the favorite. This causes the favorite to be deleted.
        /// </summary>
        private void Item_DeleteClick(object sender, EventArgs e)
        {
            FavoriteChannelThumbnail t = sender as FavoriteChannelThumbnail;
            if (t == null)
            {
                return;
            }

            this.Remove(t.Channel);
        }

        #endregion

        #region [File I/O]

        private void FavoriteChannels_Load(object sender, EventArgs e)
        {
            LoadFavoritesFromFile();
        }

        private static readonly string FavoritesStoreRoot = Config.AppUser.TVScannerSettingsRoot + @"favchan\";
        private static readonly string FavoritesStoreFile = "index.xml";

        private void LoadFavoritesFromFile()
        {
            try
            {
                SourceChannelsStore stored = SourceChannelsStore.LoadFromFile(FavoritesStoreRoot + FavoritesStoreFile);
                AllFavorites = new Dictionary<TVSource, ChannelCollection>();
                foreach (SourceChannelsStoreItem item in stored.Source)
                {
                    AllFavorites.Add(item.Type, item.Channels);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        public void SaveFavoritesToFile()
        {
            try
            {
                Directory.CreateDirectory(FavoritesStoreRoot);

                //save screenshots
                string screenShotsDir = FavoritesStoreRoot + this.CurrentTVSource.ToString();
                Directory.CreateDirectory(screenShotsDir);
                foreach(FavoriteChannelThumbnail t in panelThumbnails.Controls)
                {
                    t.SaveThumbnailImage(screenShotsDir);
                }

                SourceChannelsStore saveAs = new SourceChannelsStore();
                foreach (KeyValuePair<TVSource, ChannelCollection> item in AllFavorites)
                {
                    saveAs.Add(item.Key, item.Value);
                }
                saveAs.SaveToFile(FavoritesStoreRoot + FavoritesStoreFile);
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        #endregion

        /// <summary>
        /// For the channels in the specified channel collection, if any of them are CurrentFavorites, we will assume that
        /// the argument has more current information, and we'll copy that info.
        /// </summary>
        /// <param name="knownChannels">the most up to date channel information</param>
        public void UpdateFavorites(ChannelCollection knownChannels)
        {
            for (int i = 0; i < CurrentFavorites.Items.Count; i++)
            {
                int index = knownChannels.IndexOf(CurrentFavorites.Items[i]);
                if (index > -1)
                {
                    CurrentFavorites.Items[i] = knownChannels.Items[index];
                }
            }

            RecreateThumbnails();
        }
    }
}
