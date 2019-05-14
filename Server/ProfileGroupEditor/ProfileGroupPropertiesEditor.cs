using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.Server.ProfileGroupEditor
{
    public partial class ProfileGroupPropertiesEditor : UserControl
    {
        public ProfileGroupPropertiesEditor()
        {
            InitializeComponent();
        }

        private ProfileGroup _myGroup;

        public ProfileGroup ProfileGroup
        {
            get { return _myGroup; }
            set
            {
                _myGroup = value;

                if (_myGroup == null)
                {
                    return;
                }

                txtName.Text = _myGroup.Name;

                txtComment.Text = _myGroup.Comment;

                int lowVidBits = int.MaxValue;
                int hiVidBits = int.MinValue;
                int lowAudioBits = int.MaxValue;
                int hiAudioBits = int.MinValue;

                cbDefaultProfile.Items.Clear();
                foreach (Profile p in _myGroup.Items)
                {
                    cbDefaultProfile.Items.Add(p.Name);

                    if (p.Video != null)
                    {
                        if (p.Video.VBR != null)
                        {
                            if (p.Video.VBR.MinBitRate < lowVidBits)
                                lowVidBits = p.Video.VBR.MinBitRate;
                            if (p.Video.VBR.MaxBitRate > hiVidBits)
                                hiVidBits = p.Video.VBR.MaxBitRate;
                        }
                        else
                        {
                            if (p.Video.ConstantBitRate < lowVidBits)
                                lowVidBits = p.Video.ConstantBitRate;
                            if (p.Video.ConstantBitRate > hiVidBits)
                                hiVidBits = p.Video.ConstantBitRate;
                        }
                    }
                    if (p.Audio != null)
                    {
                        if (p.Audio.VBR != null)
                        {
                            if (p.Audio.VBR.MinBitRate < lowAudioBits)
                                lowAudioBits = p.Audio.VBR.MinBitRate;
                            if (p.Audio.VBR.MaxBitRate > hiAudioBits)
                                hiAudioBits = p.Audio.VBR.MaxBitRate;
                        }
                        else
                        {
                            if (p.Audio.ConstantBitRate < lowAudioBits)
                                lowAudioBits = p.Audio.ConstantBitRate;
                            if (p.Audio.ConstantBitRate > hiAudioBits)
                                hiAudioBits = p.Audio.ConstantBitRate;
                        }
                    }
                }

                cbDefaultProfile.Enabled = (cbDefaultProfile.Items.Count != 0);

                if (_myGroup.DefaultProfileName != null)
                {
                    cbDefaultProfile.SelectedItem = _myGroup.DefaultProfileName;
                    if (cbDefaultProfile.SelectedIndex == -1)
                    {
                        _myGroup.DefaultProfileName = null;
                    }
                }

                general_txtLowVid.Text = (lowVidBits != int.MaxValue) ? lowVidBits.ToString("0 kbps") : "-";
                general_txtHiVid.Text = (hiVidBits != int.MinValue) ? hiVidBits.ToString("0 kbps") : "-";

                general_txtLowAud.Text = (lowAudioBits != int.MaxValue) ? lowAudioBits.ToString("0 kbps") : "-";
                general_txtHiAud.Text = (hiAudioBits != int.MinValue) ? hiAudioBits.ToString("0 kbps") : "-";
            }
        }

        /// <summary>
        /// fired when the ProfileGroup name changes.
        /// </summary>
        [Category("Action"), Browsable(true)]
        public event EventHandler ProfileGroupNameChanged;

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            ProfileGroup.Name = txtName.Text;

            if (ProfileGroupNameChanged != null)
            {
                ProfileGroupNameChanged(this, new EventArgs());
            }
        }

        private void cbDefaultProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProfileGroup.DefaultProfileName = cbDefaultProfile.SelectedItem.ToString();
        }

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            ProfileGroup.Comment = txtComment.Text;

            lblCharCount.Text = (txtComment.MaxLength - txtComment.Text.Length).ToString("(0 chars left)");
        }

    }
}
