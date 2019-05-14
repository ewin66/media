using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FutureConcepts.Media.CommonControls
{
    public partial class FCTrackbar : UserControl
    {
        public FCTrackbar()
        {
            this.Size = new Size(150, 40);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            TrackWidth = 5;
            InitializeComponent();
            DoubleBuffered = true;
        }

        #region Properties

        private double _value;
        /// <summary>
        /// The last intentionally-set value of the control
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public double Value
        {
            get { return _value; }
            set
            {
                if (RestrictToCustomTicks)
                    _value = ForceValueToClosestTickMark(value);
                else
                    _value = ForceValueIntoRange(value);

                CurrentValue = _value;

                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }
            }
        }
      
        private double _currentValue;
        /// <summary>
        /// The current value of the control, even while changing
        /// </summary>
        [Category("Behavior"), Browsable(false)]
        public double CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (RestrictToCustomTicks)
                    _currentValue = ForceValueToClosestTickMark(value);
                else
                    _currentValue = ForceValueIntoRange(value);

                this.Invalidate();
            }
        }

        private double _min = 0;
        /// <summary>
        /// The minimum allowed value
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public double Minimum
        {
            get { return _min; }
            set
            {
                _min = value;

                if(Value == CurrentValue)
                    Value = ForceValueIntoRange(Value);
            }
        }

        private double _max = 10;
        /// <summary>
        /// The maximum allowed value
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public double Maximum
        {
            get { return _max; }
            set
            {
                _max = value;

                if(Value == CurrentValue)
                    Value = ForceValueIntoRange(Value);
            }
        }

        private TickStyle _tickMarks = TickStyle.BottomRight;
        /// <summary>
        /// Determines where the tickmarks will be placed.
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public TickStyle TickStyle
        {
            get { return _tickMarks; }
            set { _tickMarks = value; }
        }

        private double _freq = 1;
        /// <summary>
        /// The frequency at which the tick marks should be drawn
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public double TickFrequency
        {
            get { return _freq; }
            set { _freq = value; }
        }
        
        private double[] _customTicks = null;
        /// <summary>
        /// A list of values that indicate where tickmarks should be drawn.
        /// If non-null then this overrides <c>TickFrequency</c>
        /// </summary>
        /// <remarks>
        /// If using the <c>RestrictToCustomTicks</c> property, this list *must* be sorted low-to-high.
        /// </remarks>
        [Category("Appearance"), Browsable(true)]
        public double[] CustomTicks
        {
            get { return _customTicks; }
            set
            {
                _customTicks = value;
                this.Invalidate();
            }
        }

        private bool _restrictToCustomTicks = false;
        /// <summary>
        /// If this value is true, and CustomTicks is non-null, then the only values the user may select are those
        /// in the CustomTicks list.
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public bool RestrictToCustomTicks
        {
            get { return _restrictToCustomTicks; }
            set { _restrictToCustomTicks = value; }
        }
        
        private Orientation _orientation = Orientation.Horizontal;
        /// <summary>
        /// Used to determine if the track bar is laid out horizontally or vertically
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                TrackWidth = TrackWidth;
                this.Invalidate();
            }
        }

        private double _trackClickStep = 0.0;
        /// <summary>
        /// When set to 0, the thumb jumps to any click on the bar. When non-zero, the Value is incremented by
        /// TrackClickStepValue in the indicated direction
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("When set to 0, the thumb jumps to any click on the bar. When non-zero, the Value is incremented by TrackClickStepValue in the indicated direction")]
        public double TrackClickStepValue
        {
            get
            {
                return _trackClickStep;
            }
            set
            {
                _trackClickStep = value;
            }
        }



        private Color _disabledColor = Color.Gray;
        /// <summary>
        /// Use this property to set the color everything draws in if disabled.
        /// Doesn't affect images.
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public Color DisabledColor
        {
            get { return _disabledColor; }
            set { _disabledColor = value; }
        }

        private Color _tickColor = Color.DarkGray;
        /// <summary>
        /// The color of the tick marks
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public Color TickColor
        {
            get { return _tickColor; }
            set { _tickColor = value; }
        }

        private Color _trackColor = Color.FromArgb(0, 96, 160);
        /// <summary>
        /// Use this property to set the color for the groove color (the track)
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public Color TrackColor
        {
            get { return _trackColor; }
            set { _trackColor = value; }
        }

        private int _trackWidth;
        protected int _thumbWidth;
        protected int _thumbHeight;
        /// <summary>
        /// Defines the width of the track, the thumb is then 2 x 4 times the width.
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public int TrackWidth
        {
            get { return _trackWidth; }
            set
            {
                _trackWidth = value;

                _thumbWidth = value * ((Orientation == Orientation.Vertical) ? 4 : 2);
                _thumbHeight = value * ((Orientation == Orientation.Vertical) ? 2 : 4);
            }
        }

        #endregion

        #region CustomTicks Support

        /// <summary>
        /// Forces a value to the closest tick mark
        /// </summary>
        /// <remarks>
        /// <c>CustomTicks</c> must have 1 or more elements, and <c>RestrictToCustomTicks</c> must be true, else does nothing
        /// </remarks>
        /// <param name="value">value to force</param>
        /// <returns>one of the values in <c>CustomTicks</c></returns>
        protected double ForceValueToClosestTickMark(double value)
        {
            if (CustomTicks == null)
                return ForceValueIntoRange(value);
            if ((CustomTicks.Length < 1) || (!RestrictToCustomTicks))
                return ForceValueIntoRange(value);

            //do binary search
            int valueIndex = SearchTickMarks(value);

            if (valueIndex > -1)
                return ForceValueIntoRange(CustomTicks[valueIndex]);
            else
                return ForceValueIntoRange(value);
        }

        /// <summary>
        /// Locates the index where a given value would be found in the CustomTicks array, whether or not its there.
        /// </summary>
        /// <param name="find">value to find</param>
        /// <returns>
        /// the index of this value, or the index containing the value closest to <c>find</c>.
        /// </returns>
        private int SearchTickMarks(double find)
        {
            if (CustomTicks.Length == 1)
                return 0;

            return SearchTickMarks(find, 0, CustomTicks.Length - 1);
        }

        /// <summary>
        /// Searches the <c>CustomTicks</c> array for the best index for this value to be represented by
        /// </summary>
        /// <param name="find"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        private int SearchTickMarks(double find, int lo, int hi)
        {
            if (lo > hi)
                return -1;

            //1 element left
            if (lo == hi)
            {
                return lo;
            }
            else
            {
                int middle = (hi - lo) / 2 + lo;
                
                return CompareTwoTicks(find,
                                       SearchTickMarks(find, lo, middle),
                                       SearchTickMarks(find, middle + 1, hi));
            }
        }

        /// <summary>
        /// Compares a given value and returns the value a or b, depending on which represents
        /// an index in CustomTicks that is closest to the value to find.
        /// </summary>
        /// <param name="find">value to find</param>
        /// <param name="a">index "a" in CustomTicks</param>
        /// <param name="b">index "b" in CustomTicks</param>
        /// <returns>
        /// the index containing the value closest to <c>find</c>.
        /// Returns -1 if both <c>a</c> and <c>b</c> are invalid indicies
        /// </returns>
        private int CompareTwoTicks(double find, int a, int b)
        {
            if((a > -1) && (b < 0))
                return a;
            if((a < 0) && (b > -1))
                return b;

            try
            {
                if (Math.Abs(CustomTicks[a] - find) < Math.Abs(CustomTicks[b] - find))
                    return a;
                else
                    return b;
            }
            catch
            {
                return -1;
            }
        }

        #endregion

        #region Painting

        /// <summary>
        /// called when we need to paint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void FCTrackbar_Paint(object sender, PaintEventArgs e)
        {
            //system draws the background

            Pen trackPen = new Pen(Enabled ? TrackColor : DisabledColor, (float)TrackWidth);
            Brush thumbBrush = new SolidBrush(Enabled ? ForeColor : DisabledColor);

            if (Orientation == Orientation.Vertical)
            {
                //draw the track
                e.Graphics.DrawLine(trackPen, (ClientRectangle.Width / 2), Padding.Top + TrackWidth,
                                              (ClientRectangle.Width / 2), ClientRectangle.Height - Padding.Bottom - TrackWidth);

                //draw the tick marks
                DrawTickMarks(e.Graphics);

                //draw the thumb
                e.Graphics.FillRectangle(thumbBrush,
                                         (ClientRectangle.Width / 2) - (_thumbWidth / 2),
                                         ValueToPoint(CurrentValue) - (_thumbHeight / 2),
                                         _thumbWidth, _thumbHeight);
                                         
            }
            else
            {
                //draw the track
                e.Graphics.DrawLine(trackPen, Padding.Left + TrackWidth, (ClientRectangle.Height / 2),
                                              (ClientRectangle.Width - Padding.Right - TrackWidth), (ClientRectangle.Height / 2));


                //draw tick marks
                DrawTickMarks(e.Graphics);

                //draw the thumb
                e.Graphics.FillRectangle(thumbBrush,
                                         ValueToPoint(CurrentValue) - (_thumbWidth / 2),
                                         (ClientRectangle.Height / 2) - (_thumbHeight / 2),
                                         _thumbWidth, _thumbHeight);
            }
        }

        /// <summary>
        /// Determines if tick marks should be drawn, and if so, draws them.
        /// </summary>
        /// <param name="g"><c>Graphics</c> object to draw on</param>
        protected virtual void DrawTickMarks(Graphics g)
        {
            if (TickStyle == TickStyle.None)
                return;

            Pen tickPen = new Pen(Enabled ? TickColor : DisabledColor, 1);

            if (CustomTicks == null)
            {
                //if anything is infinity return without drawing, because it will overflow otherwise
                if (double.IsInfinity(Minimum) || double.IsInfinity(Maximum) || double.IsInfinity(TickFrequency))
                {
                    return;
                }

                if (TickFrequency <= 0)
                {
                    DrawTick(g, ValueToPoint(Minimum), tickPen);
                    DrawTick(g, ValueToPoint(Maximum), tickPen);
                    return;
                }

                for (double i = Minimum; i < Maximum; i += TickFrequency)
                {
                    DrawTick(g, ValueToPoint(i), tickPen);
                }
                DrawTick(g, ValueToPoint(Maximum), tickPen);
            }
            else
            {
                foreach (double value in CustomTicks)
                {
                    DrawTick(g, ValueToPoint(value), tickPen);
                }
            }
        }

        /// <summary>
        /// Draws a tick mark at the specified location, according to <c>this.TickStyle</c>
        /// </summary>
        /// <param name="g"><c>Graphics</c> object to draw on</param>
        /// <param name="coord">The pixel coordinate that corresponds to the tick's value.</param>
        /// <param name="tickPen"><c>Pen</c> to draw the tick mark with</param>
        protected void DrawTick(Graphics g, int coord, Pen tickPen)
        {
            DrawTick(g, coord, tickPen, TickStyle);
        }
        /// <summary>
        /// Draws a tick mark at the specified location, according to the specified TickStyle
        /// </summary>
        /// <param name="g"><c>Graphics</c> object to draw on</param>
        /// <param name="coord">The pixel coordinate that corresponds to the tick's value</param>
        /// <param name="tickPen"><c>Pen</c> to draw the tick mark with</param>
        /// <param name="position"><c>TickStyle</c> to draw the tick in</param>
        protected void DrawTick(Graphics g, int coord, Pen tickPen, TickStyle position)
        {
            if (position == TickStyle.Both)
            {
                DrawTick(g, coord, tickPen, TickStyle.TopLeft);
                DrawTick(g, coord, tickPen, TickStyle.BottomRight);
            }
            else if (position == TickStyle.TopLeft)
            {
                if (Orientation == Orientation.Vertical)
                {
                    g.DrawLine(tickPen, ClientRectangle.Left + Padding.Left,
                                        coord,
                                        ClientRectangle.Left + Padding.Left + TrackWidth,
                                        coord);
                }
                else
                {
                    g.DrawLine(tickPen, coord,
                                        ClientRectangle.Top + Padding.Top,
                                        coord,
                                        ClientRectangle.Top + Padding.Top + TrackWidth);
                }
            }
            else if (position == TickStyle.BottomRight)
            {
                if (Orientation == Orientation.Vertical)
                {
                    g.DrawLine(tickPen, ClientRectangle.Width,
                                        coord,
                                        ClientRectangle.Width - TrackWidth,
                                        coord);
                }
                else
                {
                    g.DrawLine(tickPen, coord,
                                        ClientRectangle.Bottom,
                                        coord,
                                        ClientRectangle.Bottom - TrackWidth);
                }
            }
        }

        /// <summary>
        /// Converts a point in the control to a given value
        /// </summary>
        /// <param name="point">the point to convert</param>
        /// <returns>the value nearest the given point</returns>
        protected double PointToValue(Point point)
        {
            if (Orientation == Orientation.Vertical)
            {
                double temp = ((point.Y + Padding.Bottom + TrackWidth) - ClientRectangle.Height) / -GetPixelsPerValue();
                return (double.IsNaN(temp) ? 0 : temp) + Minimum;
            }
            else
            {
                double temp = ((point.X - Padding.Left - TrackWidth) / GetPixelsPerValue());
                return (double.IsNaN(temp) ? 0 : temp) + Minimum;
            }
        }

        /// <summary>
        /// Returns the position in pixels away from the bottom or left of the trackbar that represents a given value.
        /// </summary>
        /// <param name="value">value to find a location for</param>
        /// <returns>Returns the position in pixels away from the bottom or left of the trackbar that represents a given value.</returns>
        protected int ValueToPoint(double value)
        {
            //if a given value is infinite, move it off the control's surface so it won't be seen
            if (double.IsInfinity(value))
            {
                return -1;
            }

            if (Orientation == Orientation.Vertical)
            {
                return (int)(ClientRectangle.Height - (GetPixelsPerValue() * (value - Minimum))) - Padding.Bottom - TrackWidth;
            }
            else
            {
                return (int)(GetPixelsPerValue() * (value - Minimum)) + Padding.Left + TrackWidth;
            }
        }

        /// <returns>
        /// Returns the number of pixels that represent a value of "1" given the current maximum and minimum
        /// </returns>
        protected double GetPixelsPerValue()
        {
            if ((Maximum - Minimum) <= 0)
            {
                return 0;
            }
            else
            {
                return GetTrackLength() / (Maximum - Minimum);
            }
        }

        /// <summary>
        /// Calculates the length of the track
        /// </summary>
        /// <returns>returns the length of the track, in pixels</returns>
        protected int GetTrackLength()
        {
            if (Orientation == Orientation.Vertical)
            {
                return ClientRectangle.Height - Padding.Top - Padding.Bottom - (TrackWidth * 2);
            }
            else
            {
                return ClientRectangle.Width - Padding.Right - Padding.Left - (TrackWidth * 2);
            }
        }

        #endregion

        #region User Input

        [Category("Action"), Browsable(true)]
        public new event EventHandler Scroll;

        [Category("Action"), Browsable(true)]
        public event EventHandler ValueChanged;

        private bool _hasMouse;
        /// <summary>
        /// handles the status of whether or not we should listen to the mouse
        /// </summary>
        protected bool HasMouse
        {
            get { return _hasMouse; }
            set { _hasMouse = value; }
        }

        private void FCTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if ((TrackClickStepValue != 0) && (!PointIsInThumb(e.Location)))
            {
                double clickedValue = PointToValue(e.Location);
                if (clickedValue > Value)
                {
                    Value += TrackClickStepValue;
                }
                else
                {
                    Value -= TrackClickStepValue;
                }

                HasMouse = false;
            }
            else
            {
                HasMouse = true;
                FCTrackbar_MouseMove(this, e);
            }

        }

        /// <summary>
        /// Returns true if the given point is in/on the thumb
        /// </summary>
        /// <param name="p">point to check</param>
        /// <returns>true if the point is in/on the thumb, false if not</returns>
        private bool PointIsInThumb(Point p)
        {
            int pos = ValueToPoint(Value);
            int lo, hi;

            if (Orientation == Orientation.Horizontal)
            {
                lo = pos - (_thumbWidth / 2);
                hi = pos + (_thumbWidth / 2);
                return (p.X >= lo) && (p.X <= hi);
            }
            else
            {
                lo = pos - (_thumbHeight / 2);
                hi = pos + (_thumbHeight / 2);
                return (p.Y >= lo) && (p.Y <= hi);
            }
        }

        private void FCTrackbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (HasMouse)
            {
                CurrentValue = PointToValue(e.Location);

                if (Scroll != null)
                {
                    Scroll(this, new EventArgs());
                }
            }

        }

        private void FCTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            Value = CurrentValue;
            HasMouse = false;
        }

        /// <summary>
        /// Forces the specified value to fall within the <c>Minimum</c> and <c>Maximum</c> values.
        /// </summary>
        /// <param name="value">value in any range</param>
        /// <returns>value that is in the range [Minimum, Maximum]</returns>
        protected double ForceValueIntoRange(double value)
        {
            if (value < Minimum)
            {
                value = Minimum;
            }
            else if (value > Maximum)
            {
                value = Maximum;
            }
            return value;
        }

        #endregion
    }
}
