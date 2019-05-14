using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class FCMessageBox : CloseForm
    {
        public FCMessageBox()
        {
            InitializeComponent();
        }

        public FCMessageBox(string message)
        {
            InitializeComponent();
            Graphics g = fcTextBox1.CreateGraphics();
            SizeF size = g.MeasureString(message, fcTextBox1.Font, 115);
            int height = (int)Math.Ceiling(size.Height);
            fcTextBox1.Height = height;
            if (fcTextBox1.Height > 112)
            {
                fcTextBox1.Height = 112;
                fcTextBox1.ScrollBars = ScrollBars.Vertical;
            }
           
            this.Height = fcTextBox1.Height + 90;
            fcTextBox1.Text = message;
        }

        public static DialogResult Show(string title, string message)
        {
            FCMessageBox toShow = new FCMessageBox(message);
            toShow.Text = title;
            toShow.Height = toShow.fcTextBox1.Height + 110;
            return toShow.ShowDialog();
         }

        public static DialogResult Show(string title, string message, IWin32Window parent)
        {
            FCMessageBox toShow = new FCMessageBox(message);
            toShow.Text = title;
            toShow.Height = toShow.fcTextBox1.Height + 110;
            return toShow.ShowDialog(parent);
        }

        //private void close(object sender, EventArgs e)
        //{
        //    this.Dispose();
        //}

        protected override void CloseButtonClicked()
        {
            this.Close();
            //this.Dispose();
        }       
    }
}