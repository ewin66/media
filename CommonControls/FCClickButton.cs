using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class FCClickButton : Label
    {
        public FCClickButton()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.MouseDown += new MouseEventHandler(FCClickButtonToo_MouseDown);
            this.MouseUp += new MouseEventHandler(FCClickButtonToo_MouseUp);
            this.Click += new EventHandler(FCClickButtonToo_Click);
        }


        void FCClickButtonToo_Click(object sender, EventArgs e)
        {
            HandleButtonClick();
        }

        protected virtual void HandleButtonClick()
        {
        }

        public void ButtonClicked()
        {
            HandleButtonClick();
        }

        void FCClickButtonToo_MouseUp(object sender, MouseEventArgs e)
        {
            if (!toggle)
            {
                this.ImageIndex = 0;
            }
        }

        void FCClickButtonToo_MouseDown(object sender, MouseEventArgs e)
        {
            if (toggle)
            {
                lock (this)
                {
                    if (ImageIndex == 1) ImageIndex = 0;
                    else ImageIndex = 1;
                }
            }
            else
            {
                this.ImageIndex = 1;
            }
        }

        private bool toggle = false;
        public bool Toggle
        {
            get { return toggle; }
            set { toggle = value; }
        }

        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = false;
                // always false! i don't like autosize!
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
    }
}
