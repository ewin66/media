using System;
using System.Reflection;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.Tester
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Assembly clientCommon = Assembly.GetAssembly(typeof(GraphControl));
            Type[] types = clientCommon.GetExportedTypes();
            foreach (Type t in types)
            {
                if ((t.IsClass) && (!t.IsAbstract))
                {
                    cbClientType.Items.Add(t);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbClientType.SelectedIndex > -1)
                {
                    object client = Activator.CreateInstance((Type)cbClientType.SelectedItem, tbServer.Text);
                    MethodDude dude = new MethodDude(client);
                    dude.Show(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, @"Instantiate Fail");
            }
        }

        private void btnStatics_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbClientType.SelectedIndex > -1)
                {
                    MethodDude dude = new MethodDude((Type)cbClientType.SelectedItem);
                    dude.Show(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, @"Fail");
            }
        }
    }
}
