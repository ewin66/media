using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class EncryptionKeyInput : UserControl
    {
        public EncryptionKeyInput()
        {
            InitializeComponent();
        }

        private EncryptionCapabilities _capabilities;
        /// <summary>
        /// Controls the capabilities that are listed for the user
        /// </summary>
        public EncryptionCapabilities Capabilities
        {
            get
            {
                return _capabilities;
            }
            set
            {
                _capabilities = value;
                if (_capabilities != null)
                {
                    SetupControl();
                }
            }
        }

        /// <summary>
        /// applies the Capabilities and inits the control
        /// </summary>
        private void SetupControl()
        {
            type.Items.Clear();
            foreach (EncryptionType t in Capabilities.SupportedTypes)
            {
                type.Items.Add(t);
            }
            type.SelectedIndex = -1;

            keyLength.Items.Clear();
            keyLength.Enabled = false;

            key.Text = string.Empty;
            key.Enabled = false;

            UpdateKeyLengthStatus();
        }

        //private EncryptionInfo _disabledEncryptionInfo = new EncryptionInfo() { Type = EncryptionType.None };
        ///// <summary>
        ///// Can be used to test if encryption is disabled
        ///// </summary>
        //public EncryptionInfo DisabledEncryptionInfo
        //{
        //    get
        //    {
        //        return _disabledEncryptionInfo;
        //    }
        //}

        private bool _clearEncryption = true;
        /// <summary>
        /// Gets or sets the current selected encryption the control is displaying
        /// </summary>
        public EncryptionInfo SelectedEncryption
        {
            get
            {
                if (_clearEncryption)
                {
                    return null;
                }
                else
                {
                    EncryptionInfo info = new EncryptionInfo();
                    if (type.SelectedItem != null)
                    {
                        info.Type = (EncryptionType)type.SelectedItem;
                    }
                    else
                    {
                        info.Type = EncryptionType.Unknown;
                    }

                    if (keyLength.SelectedItem != null)
                    {
                        info.KeyLength = (int)keyLength.SelectedItem;
                    }
                    else
                    {
                        info.KeyLength = 0;
                    }

                    if (!string.IsNullOrEmpty(key.Text))
                    {
                        info.DecryptionKey = EncryptionInfo.ParseHex(key.Text);
                    }

                    return info;
                }
            }
            set
            {
                if (value == null)
                {
                    type.SelectedItem = EncryptionType.None;
                    keyLength.SelectedIndex = -1;
                    key.Text = string.Empty;
                    _clearEncryption = true;
                    return;
                }

                _clearEncryption = false;

                for (int i = 0; i < type.Items.Count; i++)
                {
                    if (value.Type == (EncryptionType)type.Items[i])
                    {
                        type.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < keyLength.Items.Count; i++)
                {
                    if (value.KeyLength == (int)keyLength.Items[i])
                    {
                        keyLength.SelectedIndex = i;
                        break;
                    }
                }

                if (value.DecryptionKey != null)
                {
                    key.Text = EncryptionInfo.ToHex(value.DecryptionKey);
                }
                else
                {
                    key.Text = "";
                }
            }
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e)
        {
            _clearEncryption = false;
            keyLength.Items.Clear();

            if (type.SelectedIndex == -1)
            {
                keyLength.Enabled = false;
                key.Enabled = false;
            }
            else
            {
                EncryptionType selectedType = (EncryptionType)type.SelectedItem;

                key.Enabled = (selectedType != EncryptionType.None);

                List<int> supportedKeyLengthBits = Capabilities.SupportedTypesAndKeyLengths[selectedType];
                if (supportedKeyLengthBits != null)
                {
                    foreach (int i in supportedKeyLengthBits)
                    {
                        keyLength.Items.Add(i);
                    }
                }

                if (keyLength.Items.Count > 0)
                {
                    keyLength.SelectedIndex = 0;
                }

                keyLength.Enabled = (keyLength.Items.Count > 1);
            }

            UpdateKeyLengthStatus();
        } 

        private void keyLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            _clearEncryption = false;
            if (keyLength.SelectedIndex != -1)
            {
                key.MaxLength = ((int)keyLength.SelectedItem) / 4;
            }
            UpdateKeyLengthStatus();
        }

        private void key_TextChanged(object sender, EventArgs e)
        {
            _clearEncryption = false;
            UpdateKeyLengthStatus();
        }

        private void UpdateKeyLengthStatus()
        {
            string szLen = "?";
            int len = -1;
            if (keyLength.SelectedIndex != -1)
            {
                len = ((int)keyLength.SelectedItem) / 4;
                szLen = len.ToString();
            }

            keyLengthStatus.Text = key.Text.Length + " / " + szLen;

            keyLengthStatus.Visible = (len != -1);  //hide the label if its in appropriate

            pKeyValid.BackColor = Color.Black;

            if (key.Text.Length != len)
            {
                keyLengthStatus.ForeColor = Color.Red;
                if (len != -1)
                {
                    pKeyValid.BackColor = keyLengthStatus.ForeColor;
                }
                ok.Enabled = false;
            }
            else
            {
                keyLengthStatus.ForeColor = Color.FromArgb(24, 140, 79);
                ok.Enabled = (EncryptionType)type.SelectedItem != EncryptionType.None;
            }
        }

        private void key_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = e.KeyChar.ToString().ToUpperInvariant()[0];

            if (e.KeyChar == 0x08)
            {
                e.Handled = false;
            }
            else if (!Regex.IsMatch(e.KeyChar.ToString(), "^[0-9a-fA-F]+$"))
            {
                e.Handled = true;
            }
            //TODO handle Enter here to click OK?
        }

        private void type_Format(object sender, ListControlConvertEventArgs e)
        {
            if ((e.ListItem is EncryptionType) && (e.DesiredType == typeof(string)))
            {
                e.Value = ((EncryptionType)e.ListItem).ToDisplayString();
            }
        }

        #region Dialog Close

        public event CancelEventHandler DialogClosed;

        private void cancel_Click(object sender, EventArgs e)
        {
            RaiseDialogClosed(true);
        }

        private void RaiseDialogClosed(bool cancel)
        {
            if (DialogClosed != null)
            {
                DialogClosed(this, new CancelEventArgs(cancel));
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            RaiseDialogClosed(false);
        }

        private void clear_Click(object sender, EventArgs e)
        {
            SelectedEncryption = null;
            RaiseDialogClosed(false);
        }

        #endregion

    }
}
