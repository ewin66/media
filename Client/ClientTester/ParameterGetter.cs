using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.Tester
{
    public partial class ParameterGetter : Form
    {
        private ParameterInfo[] parameters;
        private object[] arguments;

        public ParameterGetter(string methodName, ParameterInfo[] parameters)
        {
            InitializeComponent();
            gbParams.Text = methodName;
            this.parameters = parameters;
            BuildInputs(this.parameters);
            arguments = new object[this.parameters.Length];
        }

        private void BuildInputs(ParameterInfo[] parameters)
        {
            pParams.Controls.Clear();
            int y = 0;
            for(int i = 0; i < parameters.Length; i++)
            {
                Control c = Build(i, parameters[i]);
                c.Location = new Point(0, y);
                pParams.Controls.Add(c);
                y += c.Height;
            }
        }

        private Control Build(int index, ParameterInfo i)
        {
            Panel p = new Panel();
            p.AutoSize = true;
            p.Tag = index;

            Label l = new Label();
            l.Text = i.ParameterType.Name + " " + i.Name;
            l.Dock = DockStyle.Left;
            

            if(i.ParameterType.Equals(typeof(string)))
            {
                TextBox b = new TextBox();
                b.Tag = i;
                b.Dock = DockStyle.Fill;
                p.Controls.Add(b);
            }

            p.Controls.Add(l);

            return p;

        }

        public object[] Arguments
        {
            get
            {
                return arguments;
            }
        }

        private void btnAuto_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.Equals(typeof(Contract.ClientConnectRequest)))
                {
                    arguments[i] = new Contract.ClientConnectRequest("hdtv");
                }
                else if (parameters[i].ParameterType.Equals(typeof(string)))
                {
                    arguments[i] = string.Empty;
                }
                else
                {
                    arguments[i] = Activator.CreateInstance(parameters[i].ParameterType);
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Control r = GetControl(parameters[i], gbParams.Controls);
                if (r != null)
                {
                    if (r is TextBox)
                    {
                        arguments[i] = ((TextBox)r).Text;
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private Control GetControl(ParameterInfo info, Control.ControlCollection collection)
        {
            foreach (Control c in collection)
            {
                if (c.Tag == info)
                {
                    return c;
                }
                if (c.Controls != null)
                {
                    Control temp = GetControl(info, c.Controls);
                    if (temp != null)
                    {
                        return temp;
                    }
                }
            }
            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
