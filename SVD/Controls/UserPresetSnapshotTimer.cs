using System;
using FutureConcepts.Media.Client.StreamViewer;

namespace FutureConcepts.Media.SVD.Controls
{

    /// <summary>
    /// This class handles taking a snapshot at a delayed point in time from a stream viewer control
    /// kdixon
    /// </summary>
    internal class UserPresetSnapshotTimer
    {
        private System.Windows.Forms.Timer timer;

        private UserPresetItemView Target { get; set; }
        private StreamViewerControl StreamViewer { get; set; }

        public UserPresetSnapshotTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 5000;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if ((StreamViewer.State == StreamState.Playing) ||
                   (StreamViewer.State == StreamState.Recording))
                {
                    Target.Image = StreamViewer.GetSnapshot();
                }
            }
            catch
            {
            }
            finally
            {
                timer.Stop();
            }
        }

        public void TakeSnapshot(UserPresetItemView item, StreamViewerControl imageSource)
        {
            timer.Stop();
            Target = item;
            StreamViewer = imageSource;
            timer.Start();
        }

        public void Cancel()
        {
            timer.Stop();
        }
    }
}
