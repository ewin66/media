using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Media.CommonControls;
using System.Text.RegularExpressions;
using System.Net;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class GetIP : OKCancelForm
    {
        public GetIP()
        {
            InitializeComponent();
        }

        public string IP
        {
            get
            {
                IPHostEntry entry = Dns.GetHostEntry(TB_IP.Text);
                return entry.AddressList[0].ToString();
            }
        }

        private void GetIP_Load(object sender, EventArgs e)
        {
            TB_IP.Focus();
        }
    }
}