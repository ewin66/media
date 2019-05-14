using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.Tester
{
    public partial class ObjectDisplay : Form
    {
        public ObjectDisplay(string title, object obj)
        {
            InitializeComponent();
            this.Text = title;
            properties.SelectedObject = obj;
            lblValue.Text = obj.ToString();
        }
    }
}
