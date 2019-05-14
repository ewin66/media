using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FutureConcepts.Media.CommonControls
{
    public partial class GradientProgressBar : UserControl
    {
        public GradientProgressBar()
        {
            InitializeComponent();
        }

        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        private int minimum = 0;
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        private int maximum = 100;
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }
        private int progvalue;
        public int Value
        {
            get { return progvalue; }
            set
            {
                if (value < minimum || value > maximum)
                {
                    throw new Exception("Value must be greater than " + minimum + " and less than " + maximum + ".");
                }
                progvalue = value;
                DrawBar(value);
            }
        }

        private string _labelSuffix = "%";
        /// <summary>
        /// Use to change the label suffix
        /// </summary>
        public string LabelSuffix
        {
            get
            {
                return _labelSuffix;
            }
            set
            {
                _labelSuffix = value;
            }
        }



        private void DrawBar(int value)
        {
            bool vert = (orientation == Orientation.Vertical);

            float percentage = ((float)value - (float)minimum) / ((float)maximum - (float)minimum);


            int angle, startX, startY, midX, midY, endX, endY;
            Rectangle rStartToMid, rMidToEnd;
            if (vert)
            {
                value = (int)(percentage * this.Height);
                angle = -90;
                startX = 0;
                startY = this.Height;// this.Height - value;
                midX = this.Width;
                midY = this.Height / 2;
                endX = this.Width;
                endY = this.Height - value;

                rStartToMid = new Rectangle(0, this.Height / 2, this.Width, this.Height / 2 + 1);

                rMidToEnd = new Rectangle(0, -1,
                                          this.Width,
                                          this.Height / 2 + 1);
            }
            else
            {
                value = (int)(percentage * this.Width);
                angle = 0;
                startX = 0;
                startY = 0;
                midX = this.Width / 2;
                midY = this.Height;
                endX = value;
                endY = this.Height;

                rStartToMid = new Rectangle(0, 0, this.Width / 2, this.Height);
                rMidToEnd = new Rectangle(this.Width / 2 - 1, 0, this.Width / 2 + 1, this.Height);
            }

            Brush bZeroTo50 = new LinearGradientBrush(rStartToMid, StartColor, MiddleColor, angle);
            Brush b50to100 = new LinearGradientBrush(rMidToEnd, MiddleColor, EndColor, angle);


            Graphics g = this.CreateGraphics();
            g.Clear(this.BackColor);

            if (percentage <= 0.5)
            {
                g.FillRectangle(bZeroTo50, startX, vert ? endY : startY, endX, vert ? startY : endY);
            }
            else
            {
                if (vert)
                {
                    g.FillRectangle(bZeroTo50, startX, midY, midX, startY);
                    g.FillRectangle(b50to100, startX, endY, endX, midY - endY);
                }
                else
                {
                    g.FillRectangle(bZeroTo50, startX, startY, midX, midY);
                    g.FillRectangle(b50to100, midX, 0, endX - midX, endY);
                }
            }


            if (displayvalue)
            {
                string label = progvalue + this.LabelSuffix;
                SizeF size = g.MeasureString(label, this.Font);
                g.DrawString(label, this.Font, new SolidBrush(this.BackColor), (this.ClientRectangle.Width - size.Width) / 2 + 1, (this.ClientRectangle.Height - size.Height) / 2 + 1);
                g.DrawString(label, this.Font, new SolidBrush(this.BackColor), (this.ClientRectangle.Width - size.Width) / 2 + 1, (this.ClientRectangle.Height - size.Height) / 2 + 2);
                g.DrawString(label, this.Font, new SolidBrush(this.ForeColor), (this.ClientRectangle.Width - size.Width) / 2, (this.ClientRectangle.Height - size.Height) / 2);
            }
            g.Dispose();
            bZeroTo50.Dispose();
        }

        private Color startcolor = Color.LimeGreen;
        public Color StartColor
        {
            get { return startcolor; }
            set { startcolor = value; }
        }

        private Color _middleColor = Color.Yellow;

        public Color MiddleColor
        {
            get
            {
                return _middleColor;
            }
            set
            {
                _middleColor = value;
            }
        }

        private Color endcolor = Color.Red;
        public Color EndColor
        {
            get { return endcolor; }
            set { endcolor = value; }
        }

        private bool displayvalue;
        public bool DisplayValue
        {
            get { return displayvalue; }
            set { displayvalue = value; }
        }
	

        private void GradientProgressBar_Paint(object sender, PaintEventArgs e)
        {
            DrawBar(progvalue);
        }

    }
}
