using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class FCYesNoMsgBox : OKCancelForm
    {
        public FCYesNoMsgBox()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public string Message
        {
            get { return lblInfo.Text; }
            set { lblInfo.Text = value; }
        }

        public DialogResult ShowDialog(string title, string message, Size size)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            base.Text = title;
            lblInfo.Text = message;
            this.Size = size;
            return this.ShowDialog();
        }

        public static DialogResult ShowDialog(string title, string message, Size size, IWin32Window owner)
        {
            FCYesNoMsgBox msg = new FCYesNoMsgBox();
            msg.StartPosition = FormStartPosition.CenterScreen;
            msg.Text = title;
            msg.Message = message;
            msg.Size = size;
            return msg.ShowDialog(owner);
        }

        public static DialogResult ShowDialog(string title, string message, IWin32Window owner)
        {
            FCYesNoMsgBox msg = new FCYesNoMsgBox();
            msg.StartPosition = FormStartPosition.CenterScreen;
            msg.Text = title;
            msg.Message = message;
            return msg.ShowDialog(owner);
        }

        public static DialogResult ShowDialog(string title, string message)
        {
            FCYesNoMsgBox msg = new FCYesNoMsgBox();
            msg.StartPosition = FormStartPosition.CenterScreen;
            msg.Text = title;
            msg.Message = message;
            return msg.ShowDialog();
        }
    }
}