using System;
using System.Reflection;
using System.Windows.Forms;

namespace FutureConcepts.Media.Client.Tester
{
    public partial class MethodDude : Form
    {
        Type clientType = null;
        object clientObject = null;

        public MethodDude(object clientObject)
        {
            this.clientObject = clientObject;
            this.clientType = clientObject.GetType();
            InitializeComponent();

            BuildControls();
        }

        public MethodDude(Type t)
        {
            this.clientType = t;
            InitializeComponent();

            BuildStaticControl();
        }

        private void BuildStaticControl()
        {
            gbMethods.Text = clientType.Name + " (statics)";

            AddControls(this.clientType.GetMethods(BindingFlags.Static | BindingFlags.Public));
        }

        private void BuildControls()
        {
            gbMethods.Text = clientType.Name;

            AddControls(clientType.GetMethods());
        }

        private void AddControls(MethodInfo[] methods)
        {
            pMethodButtons.Controls.Clear();

            foreach (MethodInfo m in methods)
            {
                Button b = new Button();
                b.Text = m.ReturnType.Name + " " + m.Name;
                b.AutoSize = true;
                b.Tag = m;
                b.Click += new EventHandler(b_Click);
                pMethodButtons.Controls.Add(b);
                tt.SetToolTip(b, GetParamString(m));
            }
        }

        private string GetParamString(MethodInfo m)
        {
            string paramz = string.Empty;
            ParameterInfo[] pz = m.GetParameters();
            for (int i = 0; i < pz.Length; i++)
            {
                paramz += pz[i].ParameterType.Name + " " + pz[i].Name;
                if (i < pz.Length - 1)
                {
                    paramz += ", ";
                }
            }
            return paramz;
        }

        /// <summary>
        /// Invokes the method associated with the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void b_Click(object sender, EventArgs e)
        {
            try
            {
                Button b = (Button)sender;
                MethodInfo info = (MethodInfo)b.Tag;
                ParameterInfo[] parameters = info.GetParameters();

                object[] arguments = GetParameters(info.DeclaringType.Name + "." + info.Name, parameters);
                
                object returned = info.Invoke(clientObject, arguments);
                if (returned != null)
                {
                    ObjectDisplay d = new ObjectDisplay("evaluated: " + b.Text, returned);
                    d.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += Environment.NewLine + Environment.NewLine + ex.InnerException.Message;
                }
                MessageBox.Show(this, msg, "Cannot invoke method");
            }
        }

        private object[] GetParameters(string methodName, ParameterInfo[] parameters)
        {
            if (parameters.Length > 0)
            {
                ParameterGetter g = new ParameterGetter(methodName, parameters);
                if (DialogResult.Cancel != g.ShowDialog())
                {
                    return g.Arguments;
                }
            }

            return null;
        }

        private void MethodDude_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientObject is IDisposable)
            {
                ((IDisposable)clientObject).Dispose();
            }
        }
    }
}
