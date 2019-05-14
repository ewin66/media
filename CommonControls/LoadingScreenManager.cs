using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace FutureConcepts.Media.CommonControls
{
    public class LoadingScreenManager
    {
        LoadScreen loadingScreen;
        public bool DestroyCalled=false;
        private Thread t;

        public LoadingScreenManager(Point location)
        {
            loadingScreen = new LoadScreen();
            loadingScreen.StartPosition = FormStartPosition.Manual;
            Rectangle r = System.Windows.Forms.Screen.GetWorkingArea(location);
            loadingScreen.Location = new Point(r.X + (r.Width - loadingScreen.Width) / 2, r.Y + (r.Height - loadingScreen.Height) / 2);

            t = new Thread(UpdateScreen) { IsBackground = true, Name = "LoadingScreen" };
            t.Start();
        }

        public LoadingScreenManager(Size size, Point location)
        {
            loadingScreen = new LoadScreen();
            loadingScreen.MaximumSize = size;
            loadingScreen.Size = size;
            loadingScreen.StartPosition = FormStartPosition.Manual;
            Rectangle r = System.Windows.Forms.Screen.GetWorkingArea(location);
            loadingScreen.Location = new Point(r.X + (r.Width - loadingScreen.Width) / 2, r.Y + (r.Height - loadingScreen.Height) / 2);

            t = new Thread(UpdateScreen) { IsBackground = true, Name = "LoadingScreen" };
            t.Start();
        }

        private void UpdateScreen()
        {
            if (DestroyCalled) return;

            if (!loadingScreen.Disposing && !loadingScreen.IsDisposed)
            {

                Application.Run(loadingScreen);
                //loadingScreen.Show();
                //while (!loadingScreen.Disposing && !loadingScreen.IsDisposed)
                //{
                //    Application.DoEvents();
                //    System.Threading.Thread.Sleep(150);
                //}               
            }
        }

        private string  text;
        public string Text
        {
            get { return text; }
            set 
            {
                text = value;
                loadingScreen.StatusText = text;
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set 
            {
                title = value;
                loadingScreen.Title = title;
            }
        }
	

        private int percent;
        public int Percent
        {
            get { return percent; }
            set 
            { 
                percent = value;
                loadingScreen.StatusPercent = percent;
            }
        }

        public void DestroyLoadingScreen()
        {
            DestroyCalled = true;
            
            try
            {
                if (!loadingScreen.IsDisposed)
                {
                    loadingScreen.DestroyNeeded();
                }
            }
            catch (Exception err)
            {
                FutureConcepts.Media.CommonControls.FCMessageBox.Show("Loading Screen Error", err.Message.ToString());
            }
        }

        private delegate void DoTopMost(bool value);
        public void SetLoadingScreenTopMost(bool value)
        {
            if(loadingScreen.InvokeRequired)
                loadingScreen.Invoke(new DoTopMost(SetLoadingScreenTopMost), value);
            else
                this.loadingScreen.SetTopMost(value);
        }
    }
}
