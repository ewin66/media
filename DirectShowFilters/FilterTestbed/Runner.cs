using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace FutureConcepts.Media.DirectShowFilters.FilterTestbed
{
    public partial class Runner : Form
    {
        public Runner()
        {
            InitializeComponent();
            Graph = null;
        }

        private void Runner_Load(object sender, EventArgs e)
        {
            cbGraphTypes.Items.Clear();
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(BaseGraph)))
                {
                    cbGraphTypes.Items.Add(t);
                }
            }

            if (cbGraphTypes.Items.Count > 0)
            {
                cbGraphTypes.SelectedIndex = 0;
            }
        }

        private BaseGraph graph;

        public BaseGraph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                UpdateEnabled();
            }
        }

        private void UpdateEnabled()
        {
            if (Graph == null)
            {
                run.Enabled = false;
                stop.Enabled = false;
                pause.Enabled = false;
                return;
            }

            switch (Graph.State)
            {
                case BaseGraph.GraphState.Stopped:
                    run.Enabled = true;
                    pause.Enabled = true;
                    stop.Enabled = false;
                    break;
                case BaseGraph.GraphState.Paused:
                    run.Enabled = true;
                    stop.Enabled = true;
                    pause.Enabled = false;
                    break;
                case BaseGraph.GraphState.Running:
                    stop.Enabled = true;
                    pause.Enabled = true;
                    run.Enabled = false;
                    break;
                default:
                    break;
            }

        }

        private void spawn_Click(object sender, EventArgs e)
        {
            try
            {
                Type t = (Type)cbGraphTypes.SelectedItem;

                KillGraph();
                Graph = (BaseGraph)Activator.CreateInstance(t);

                MessageBox.Show(this, "Spawn Successful");
   
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show(this, ex.InnerException.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void KillGraph()
        {
            if (Graph != null)
            {
                Graph.Dispose();
                Graph = null;
            }
        }

        private void kill_Click(object sender, EventArgs e)
        {
            try
            {
                KillGraph();
                MessageBox.Show(this, "Kill Successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void run_Click(object sender, EventArgs e)
        {
            try
            {
                if (Graph != null)
                {
                    Graph.Run();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
            finally
            {
                UpdateEnabled();
            }
        }

        private void pause_Click(object sender, EventArgs e)
        {
            try
            {
                if (Graph != null)
                {
                    Graph.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
            finally
            {
                UpdateEnabled();
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (Graph != null)
                {
                    Graph.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
            finally
            {
                UpdateEnabled();
            }
        }
    }
}
