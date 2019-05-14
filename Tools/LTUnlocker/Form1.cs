using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FutureConcepts.Media.License;
using System.Threading;

namespace LTUnlocker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LeadTools.SerialChanged += new EventHandler<LeadTools.LeadToolsLockEventArgs>(LeadTools_SerialChanged);
            bwUnlocker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwUnlocker_RunWorkerCompleted);
        }

        void bwUnlocker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTextBox1.AppendText("UNLOCKED Zenith." + Environment.NewLine);
        }

        private void LeadTools_SerialChanged(object sender, LeadTools.LeadToolsLockEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<LeadTools.LeadToolsLockEventArgs>(LeadTools_SerialChanged), sender, e);
            }
            else
            {
                richTextBox1.AppendText("Unlocked " + e.Serial + Environment.NewLine);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            if (!bwUnlocker.IsBusy)
            {
                richTextBox1.AppendText("Beginning Unlock..." + Environment.NewLine);
                bwUnlocker.RunWorkerAsync();
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            LeadTools.Lock();
            richTextBox1.AppendText("LOCKED Zenith" + Environment.NewLine);
        }

        private void bwUnlocker_DoWork(object sender, DoWorkEventArgs e)
        {
            LeadTools.Unlock();
        }
    }
}
