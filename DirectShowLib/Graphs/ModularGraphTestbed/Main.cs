using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using FutureConcepts.Media.DirectShowLib.Framework;

namespace FutureConcepts.Media.DirectShowLib.Graphs.ModularGraphTestbed
{
    public partial class Main : Form
    {
        private ModularGraph graph = new ModularGraph();
        private static ISourceSubGraph source = null;

        private BackgroundWorker connector;

        public Main()
        {
            InitializeComponent();
            timer1.Start();

            Assembly graphs = Assembly.GetAssembly(typeof(ISinkSubGraph));
            Type[] types = graphs.GetExportedTypes();
            foreach (Type t in types)
            {
                if ((t.GetInterface(typeof(ISinkSubGraph).FullName) != null) &&
                    (t.GetInterface(typeof(IIntermediateSubGraph).FullName) == null) &&
                    (t.IsClass))
                {
                    cbSinks.Items.Add(t);
                }
            }

            connector = new BackgroundWorker();
            connector.DoWork += new DoWorkEventHandler(connector_DoWork);
            connector.RunWorkerCompleted += new RunWorkerCompletedEventHandler(connector_RunWorkerCompleted);
        }

        void connector_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ((INetworkSource)source).Connect();
            }
            catch { }
        }

        void connector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
        }

        private void btnAddSource_Click(object sender, EventArgs e)
        {
            try
            {
                source = new LTNetworkSource(tbLtsfUrl.Text);
                ((INetworkSource)source).PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);

                connector.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Delegate d = new PropertyChangedEventHandler(source_PropertyChanged);
                this.Invoke(d, new object[] { sender, e });
                return;
            }

            if (e.PropertyName.Equals("Status"))
            {
                lblStatus.Text = ((INetworkSource)source).Status.ToString();

                switch (((INetworkSource)source).Status)
                {
                    case NetworkSourceStatus.Inactive:
                        break;
                    case NetworkSourceStatus.Connecting:
                        break;
                    case NetworkSourceStatus.Buffering:
                        break;
                    case NetworkSourceStatus.Connected:
                        graph.Source = (ISourceSubGraph)source;
                        break;
                    case NetworkSourceStatus.Disconnecting:
                        break;
                    case NetworkSourceStatus.Faulted:
                        if (((INetworkSource)source).StatusError != null)
                        {
                            MessageBox.Show(((INetworkSource)source).StatusError.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (graph.Sink == null)
                {
                    graph.Sink = Activator.CreateInstance((Type)cbSinks.SelectedItem) as ISinkSubGraph;
                }
                graph.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Cannot Run");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                graph.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Cannot Stop");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            spinner.Text = DateTime.Now.Second.ToString() + " . " + DateTime.Now.Millisecond.ToString();
        }

        private void btnTestPattern_Click(object sender, EventArgs e)
        {
            source = new TestPattern(true);
            graph.Source = source;
        }
    }
}
