using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class LED : UserControl
    {
        public LED()
        {
            InitializeComponent();
        }

        private Color ledcolor = Color.Lime;
        public Color LEDColor
        {
            get { return ledcolor; }
            set
            {
                ledcolor = value;
                DrawLED(value);
            }
        }

        private void DrawLED(Color color)
        {
            if (!this.IsDisposed)
            {
                Graphics g = this.CreateGraphics();
                g.Clear(this.BackColor);
                SolidBrush brush = new SolidBrush(color);
                Rectangle r = this.ClientRectangle;
                r.Width -= 2; r.Height -= 2;
                r.X += 1; r.Y += 1;
                g.FillEllipse(brush, r);
                g.DrawArc(new Pen(FadeColor(color, Color.White, 1, 2), 2), 3, 3, Width - 7, Height - 7, -90f, -90f);
                g.DrawEllipse(new Pen(FadeColor(color, Color.Black), 1.5f), r);
            }
        }

        private void LED_Paint(object sender, PaintEventArgs e)
        {
            DrawLED(ledcolor);
        }

        #region helper color functions
        public static Color FadeColor(Color c1, Color c2, int i1, int i2)
        {
            int r = (i1 * c1.R + i2 * c2.R) / (i1 + i2);
            int g = (i1 * c1.G + i2 * c2.G) / (i1 + i2);
            int b = (i1 * c1.B + i2 * c2.B) / (i1 + i2);

            return Color.FromArgb(r, g, b);
        }

        public static Color FadeColor(Color c1, Color c2)
        {
            return FadeColor(c1, c2, 1, 1);
        }
        #endregion
    }
}
