using System;
using System.Drawing;

namespace FutureConcepts.Media.CommonControls
{
	/// <summary>
	/// Summary description for MoveWindow.
	/// </summary>
	public class MoveWindow
	{
        bool mouseDown = false;
        int xOffset = 0;
        int yOffset = 0;
        System.Windows.Forms.Form form;
        System.Windows.Forms.Label caption;
        bool maximized;

		public MoveWindow(System.Windows.Forms.Form form, System.Windows.Forms.Label caption)
		{
            this.form = form;
            this.caption = caption;
            maximized = form.WindowState == System.Windows.Forms.FormWindowState.Maximized;
            caption.MouseDown +=new System.Windows.Forms.MouseEventHandler(caption_MouseDown);
            caption.MouseUp +=new System.Windows.Forms.MouseEventHandler(caption_MouseUp);
            caption.MouseMove +=new System.Windows.Forms.MouseEventHandler(caption_MouseMove);
            caption.MouseClick += new System.Windows.Forms.MouseEventHandler(caption_MouseClick);
		}

        void caption_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (maximized)
            {
                if (form.WindowState == System.Windows.Forms.FormWindowState.Maximized)
                {
                    form.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }
                else
                {
                    form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
            }
        }

        private void caption_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            xOffset = e.X;
            yOffset = e.Y;
        }

        private void caption_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void caption_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(mouseDown)
            {
                Point move = form.PointToScreen(new Point(e.X, e.Y));
                move.Offset(xOffset * -1, yOffset * -1);
                form.Location = move;
            }
        }

        public void CleanUp()
        {
            caption.MouseDown -= new System.Windows.Forms.MouseEventHandler(caption_MouseDown);
            caption.MouseUp -= new System.Windows.Forms.MouseEventHandler(caption_MouseUp);
            caption.MouseMove -= new System.Windows.Forms.MouseEventHandler(caption_MouseMove);
            caption.MouseClick -= new System.Windows.Forms.MouseEventHandler(caption_MouseClick);
        }
    }
}
