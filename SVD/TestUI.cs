using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.SVD
{
    public partial class TestUI : Form
    {
        public TestUI()
        {
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            gradientProgressBar1.Value = trackBar1.Value;
        }
    }
}
