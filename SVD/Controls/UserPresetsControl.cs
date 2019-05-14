using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Settings;
using FutureConcepts.Tools;
using FutureConcepts.Media.Contract;
using System.Xml.Serialization;
using System.IO;
using FutureConcepts.Tools.Utilities;
using System.Drawing.Imaging;
using System.Diagnostics;

using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class UserPresetsControl : UserControl
    {
        /// <summary>
        /// Holds the storage path for the user presets images
        /// </summary>
        private static readonly string StoragePath = FutureConcepts.Settings.GenericSettings.AntaresXDataPath + @"Output\SVD\UserPresets\";

        public UserPresetsControl()
        {
            InitializeComponent();
        }

        #region Public API

        #region Properties

        /// <summary>
        /// Gets or sets the title of the control
        /// </summary>
        [Browsable(true), Category("Appearance"), Description("Title to show for control.")]
        public string Title
        {
            get
            {
                return gbContent.Text;
            }
            set
            {
                gbContent.Text = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when the host should add a preset.
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the host should add a preset.")]
        public event EventHandler AddPreset;

        /// <summary>
        /// This event is raised when a preset needs to be restored.
        /// </summary>
        [Browsable(true), Category("Action"), Description("This event is raised when a preset needs to be restored.")]
        public event EventHandler<UserPresetEventArgs> RestorePreset;

        /// <summary>
        /// Raised when the user has renamed a preset.
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the user has renamed a preset.")]
        public event EventHandler<UserPresetEventArgs> PresetRenamed;

        /// <summary>
        /// Raised when the user has deleted a preset.
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the user has deleted a preset.")]
        public event EventHandler<UserPresetEventArgs> PresetDeleted;

        /// <summary>
        /// Raised when the user has cleared all of the presets.
        /// </summary>
        [Browsable(true), Category("Action"), Description("Raised when the user has cleared all of the presets.")]
        public event EventHandler PresetsCleared;

        #endregion

        /// <summary>
        /// Removes any items that are not in the given UserPresetStore, and updates items that do exist, retaining any images
        /// </summary>
        /// <param name="userPresetStore">the presets to update from -- note: this object is altered!</param>
        public void UpdateItems(UserPresetStore userPresetStore)
        {
            Debug.WriteLine("UserPresetsControl.UpdateItems");

            if (userPresetStore == null)
            {
                return;
            }

            //Step 1: Remove items that no longer exist
            List<int> indexesToDelete = new List<int>();
            for(int i = 0; i < this.Items.Count; i++)
            {
                if (!userPresetStore.Contains(this.Items[i].Preset.ID))
                {
                    indexesToDelete.Add(i);
                }
            }
            //iterate these backwards so the list shrinks predictably, instead of accidently deleteing the wrong items
            for (int i = indexesToDelete.Count - 1; i >= 0; i--)
            {
                this.Delete(indexesToDelete[i]);
            }

            //Step 2: Update items that we still have
            foreach (UserPresetItemView i in this.Items)
            {
                if (userPresetStore.Contains(i.Preset.ID))
                {
                    i.Preset = userPresetStore[i.Preset.ID];
                    //remove items we already have, so we don't get duplicates
                    userPresetStore.Remove(i.Preset.ID);
                }
            }

            //Step 3: Add new items
            foreach (UserPresetItem i in userPresetStore)
            {
                UserPresetItemView view = new UserPresetItemView();
                view.Preset = i;
                this.Add(view);
            }
        }

        private List<UserPresetItemView> _favorites = null;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<UserPresetItemView> Items
        {
            get
            {
                if (_favorites == null)
                {
                    _favorites = new List<UserPresetItemView>();
                }
                return _favorites;
            }
            set
            {
                Clear(false);
                _favorites = value;
                if (value != null)
                {
                    lv.VirtualListSize = value.Count;
                    foreach (UserPresetItemView f in value)
                    {
                        f.PropertyChanged += new PropertyChangedEventHandler(favorite_PropertyChanged);
                    }
                }
                lv_SelectedIndexChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Adds a singular item to the presets control
        /// </summary>
        /// <param name="item">item to add</param>
        public void Add(UserPresetItemView item)
        {
            if (item.Image == null)
            {
                item.Image = LoadImageIfPresent(item.Preset.ID);
            }
            item.PropertyChanged += new PropertyChangedEventHandler(favorite_PropertyChanged);
            Items.Add(item);
            lv.VirtualListSize++;
            btnClear.Visible = true; 
        }

        private string PathForPresetImage(Guid presetID)
        {
            return StoragePath + presetID.ToString();
        }

        /// <summary>
        /// If an image for the specified preset ID exists, then it is loaded. Otherwise, returns null
        /// </summary>
        /// <param name="presetID">preset ID to load</param>
        /// <returns>The Image associated with this preset, or null if none found</returns>
        private Image LoadImageIfPresent(Guid presetID)
        {
            return Utilities.ImageFromDisk(PathForPresetImage(presetID));
        }

        /// <summary>
        /// Call this method to persist the thumbnail images to disk.
        /// </summary>
        public void SaveThumbnailImages()
        {
            try
            {
                Directory.CreateDirectory(StoragePath);

                foreach (UserPresetItemView i in Items)
                {
                    try
                    {
                        if ((i.Image != null) && (i.Preset != null))
                        {
                            File.Delete(PathForPresetImage(i.Preset.ID));
                            i.Image.Save(PathForPresetImage(i.Preset.ID));
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.DumpToDebug(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Clears all the favorites locally
        /// </summary>
        /// <param name="clearOnServer">if true, clears on the server as well</param>
        public void Clear(bool clearOnServer)
        {
            if (clearOnServer)
            {
                FirePresetsCleared();
            }

            if (Items != null)
            {
                foreach (UserPresetItemView f in Items)
                {
                    f.PropertyChanged -= new PropertyChangedEventHandler(favorite_PropertyChanged);
                }
                Items.Clear();
            }
            lv.VirtualListSize = 0;
            btnClear.Visible = false;
            lv_SelectedIndexChanged(this, new EventArgs());
        }

        #endregion

        #region Painting Code

        private void favorite_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Refresh();
        }

        private static readonly Size imageSize = new Size(50, 37);
        private static readonly int yLabelOffset = imageSize.Height + 2;

        private void lv_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            int x;

            //draw frame for selection
            if (e.Item.Selected)
            {
                Pen border = new Pen(Brushes.Yellow, 1);
                e.Graphics.DrawRectangle(border, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 3);
            }

            //draw thumbnail
            if (Items[e.ItemIndex].Image != null)
            {
                x = (int)((e.Bounds.Width / 2.0) - (imageSize.Width / 2.0)) + e.Bounds.X;

                e.Graphics.DrawImage(Items[e.ItemIndex].Image,
                                     x, e.Bounds.Y + 2,
                                     imageSize.Width,
                                     imageSize.Height);
            }
            else
            {
                x = (int)((e.Bounds.Width / 2.0) - (ilIconNoImageAvailable.ImageSize.Width / 2.0)) + e.Bounds.X;

                e.Graphics.DrawImage(ilIconNoImageAvailable.Images[0],
                                     x, e.Bounds.Y + 2,
                                     ilIconNoImageAvailable.ImageSize.Width,
                                     ilIconNoImageAvailable.ImageSize.Height);
            }

            //calculate placement and size of label
            int maxTextWidth = e.Bounds.Width;
            int maxTextHeight = e.Bounds.Height - yLabelOffset;
            string itemLabel = e.Item.Text;

            SizeF labelSize;
            do
            {
                labelSize = e.Graphics.MeasureString(itemLabel, lv.Font, maxTextWidth);

                x = (int)((e.Bounds.Width / 2) - (labelSize.Width / 2.0)) + e.Bounds.X + 1;
                if (x < e.Bounds.X)
                {
                    x = e.Bounds.X + 1;
                }

                if ((labelSize.Height + yLabelOffset) > e.Bounds.Height)
                {
                    int targetSize = (int)(itemLabel.Length * (maxTextHeight / labelSize.Height)) - 2;
                    itemLabel = itemLabel.Substring(0, (targetSize < 1) ? 1 : targetSize) + "...";
                }
            } while ((labelSize.Height + yLabelOffset) > e.Bounds.Height);

            e.Graphics.DrawString(itemLabel, lv.Font, new SolidBrush(lv.ForeColor),
                                  new Rectangle(x, yLabelOffset + e.Bounds.Y, maxTextWidth, e.Bounds.Height - yLabelOffset));

            e.Item.ToolTipText = "";

            e.DrawDefault = false;
        }

        private void lv_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                ListViewItem lvi = new ListViewItem(Items[e.ItemIndex].Preset.Name);
                lvi.ImageIndex = 0;
                lvi.Tag = e.ItemIndex;
                e.Item = lvi;
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
                e.Item = new ListViewItem("?");
            }
        }

        #region ToolTip Thumbnail

        private int ttItemIndex = -1;

        /// <summary>
        /// Spawns the image/thumbnail tooltip
        /// </summary>
        /// <param name="e">allows us to determine which item we need to draw</param>
        private void lv_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (e.Item.Tag is Int32)
            {
                ttItemIndex = (int)e.Item.Tag;
                ttImage.Show("show", lv,
                             gbContent.Location.X + gbContent.Size.Width,
                             lv.Parent.Location.Y, 2000);
            }
        }

        private static readonly Size largeImageSize = new Size(133, 100);
        private static readonly Size imageToolTipSize = new Size(135, 145);

        /// <summary>
        /// When the tooltip says its going to appear, tell it what size its going to be.
        /// </summary>
        private void ttImage_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = imageToolTipSize;
        }

        private void ttImage_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds);

            //select image to show
            Image previewImage;
            if (Items[ttItemIndex].Image != null)
            {
                previewImage = Items[ttItemIndex].Image;
            }
            else
            {
                previewImage = ilToolTipNoImageAvailable.Images[0];
            }
            //draw the image
            e.Graphics.DrawImage(previewImage,
                                 e.Bounds.X + 1, e.Bounds.Y + 1,
                                 largeImageSize.Width, largeImageSize.Height);


            Brush foreBrush = new SolidBrush(lv.ForeColor);

            e.Graphics.DrawString(Items[ttItemIndex].ToString(),
                                  lv.Font,
                                  foreBrush,
                                  new RectangleF(e.Bounds.X,
                                                 e.Bounds.Y + 1 + largeImageSize.Height,
                                                 e.Bounds.Width,
                                                 e.Bounds.Height - largeImageSize.Height));

            //draw the white border
            e.Graphics.DrawRectangle(new Pen(foreBrush, 1), e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
        }

        private void lv_MouseLeave(object sender, EventArgs e)
        {
            ttImage.Hide(lv);
        }

        #endregion

        #endregion

        #region UI Handlers

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (AddPreset != null)
            {
                AddPreset(this, new EventArgs());
            }
        }

        private void lv_DoubleClick(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                FireRestorePreset(Items[lv.SelectedIndices[0]]);
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                FireRestorePreset(Items[lv.SelectedIndices[0]]);
            }
        }

        private void FireRestorePreset(UserPresetItemView item)
        {            
            if (lv.SelectedIndices.Count > 0)
            {
                if (RestorePreset != null)
                {
                    RestorePreset(this, new UserPresetEventArgs(item));
                }
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                lv.Items[lv.SelectedIndices[0]].BeginEdit();
            }
        }

        private void lv_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if ((string.IsNullOrEmpty(e.Label)) || (e.CancelEdit))
            {
                e.CancelEdit = true;
            }
            else
            {
                Items[e.Item].Preset.Name = e.Label;
                FirePresetRenamed(Items[e.Item]);
            }
        }

        private void FirePresetRenamed(UserPresetItemView item)
        {
            if (PresetRenamed != null)
            {
                PresetRenamed(this, new UserPresetEventArgs(item));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                if (DialogResult.Yes == FCYesNoMsgBox.ShowDialog("Remove preset?", "Are you sure you want to delete the preset \"" + Items[lv.SelectedIndices[0]].ToString() + "\"?", this))
                {
                    FirePresetDeleted(Items[lv.SelectedIndices[0]]);

                    Delete(lv.SelectedIndices[0]);
                }
            }
        }

        /// <summary>
        /// Deletes an item locally
        /// </summary>
        /// <param name="index">index to delete</param>
        private void Delete(int index)
        {
            if ((index >= Items.Count) || (index < 0))
            {
                return;
            }

            Items[index].PropertyChanged -= new PropertyChangedEventHandler(favorite_PropertyChanged);
            Items[index].Image = null;

            try
            {
                File.Delete(PathForPresetImage(Items[index].Preset.ID));
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }

            Items.RemoveAt(index);

            lv.VirtualListSize--;
            lv_SelectedIndexChanged(this, new EventArgs());
        }

        private void FirePresetDeleted(UserPresetItemView item)
        {
            if (PresetDeleted != null)
            {
                PresetDeleted(this, new UserPresetEventArgs(item));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == FCYesNoMsgBox.ShowDialog("Clear all presets?", "Are you sure you want to permenently delete all current presets?", this))
            {
                Clear(true);
            }
        }

        private void FirePresetsCleared()
        {
            if (PresetsCleared != null)
            {
                PresetsCleared(this, new EventArgs());
            }
        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool itemSelected = false;
            if (lv != null)
            {
                itemSelected = (lv.SelectedIndices.Count > 0);
            }
            btnRestore.Visible = itemSelected;
            btnRename.Visible = itemSelected;
            btnDelete.Visible = itemSelected;

            bool anyItems = false;
            if (Items != null)
            {
                anyItems = (Items.Count > 0);
            }
            btnClear.Visible = anyItems;

            lv.Invalidate();
        }

        #endregion
    }

    /// <summary>
    /// This class is used to pass UserPresetItemView objects that may need to be updated
    /// by a client of the UserPresetsControl class
    /// kdixon
    /// </summary>
    public class UserPresetEventArgs : EventArgs
    {
        public UserPresetEventArgs(UserPresetItemView item)
        {
            Item = item;
        }

        public UserPresetItemView Item { get; set; }
    }
}
