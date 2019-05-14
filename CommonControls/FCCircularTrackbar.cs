/*
 *  FCCircularTrackbar
 *  Kevin Dixon
 *  06/04/2008
 * 
 *  Note: This control and its padding must always be a square, or else it won't work very accurately
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;


namespace FutureConcepts.Media.CommonControls
{
    public partial class FCCircularTrackbar : UserControl
    {
        private int _thumbWidth;
        private int _thumbHeight;

        public FCCircularTrackbar()
        {

            BackColor = Color.Black;
            TrackColor = Color.Gray;
            ForeColor = Color.White;
            DisabledColor = Color.DarkGray;

            BackgroundImage = null;
            TrackImage = null;
            ThumbImage = null;

            TrackWidth = 6;
            _thumbWidth = (int)(_circleWidth * 1.5);
            _thumbHeight = _circleWidth * 3;

            ThumbAlignment = ContentAlignment.MiddleCenter;

            Padding = new Padding(10, 10, 10, 10);
            Value = 0;
            Size = new Size(150, 150);
            TrackDiameter = 100;

            DisplayValue = false;
            DisplayValueFormatString = "-0.#°";
            DisplayValue180 = false;

            InitializeComponent();


        }

        private Color _disabledColor;
        /// <summary>
        /// Use this property to set the color everything draws in if disabled.
        /// Doesn't affect images.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("The color the components are drawn in when the control is disabled")]
        public Color DisabledColor
        {
            get { return _disabledColor; }
            set { _disabledColor = value; }
        }

        private int _diameter;
        /// <summary>
        /// Sets or Returns the track diameter. Use with a TrackImage to calibrate hit-testing
        /// </summary>
        /// <remarks>
        /// When using a TrackImage, set the diameter to the diameter of your track in pixels.
        /// This is used for hit-testing to see if the user is clicking on the thumb.
        /// If you are not using a TrackImage, changing the property will have no effect, since it
        /// will be re-cacluated every time the track is drawn.
        /// </remarks>
        [Category("Behavior"), Browsable(true), Description("When TrackImage is set, use this property to calibrate hit-testing")]
        public int TrackDiameter
        {
            get { return _diameter; }
            set { _diameter = value; }
        }

        private int _circleWidth;
        /// <summary>
        /// Controls the width of the track (visually only) when not using a TrackImage
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("Controls the apparent width of the track when TrackImage is Not set.")]
        public int TrackWidth
        {
            get { return _circleWidth; }
            set
            {
                _circleWidth = value;
                if (ThumbImage == null)
                    ThumbImage = null;
            }
        }


        /// <summary>
        /// Paints the entire control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FCCircularTrackbar_Paint(object sender, PaintEventArgs e)
        {
            //the system will draw the background image and/or color

            //draw the thumb's track
            if (TrackImage == null)
            {
                Bitmap trackbar = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                Graphics g = Graphics.FromImage(trackbar);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                //draw track
                Pen p_trackCircle = new Pen(Enabled ? TrackColor : DisabledColor, _circleWidth);

                int x = (ClientRectangle.Width / 2) - (TrackDiameter / 2);
                int y = (ClientRectangle.Height /2 ) - (TrackDiameter / 2);
                Rectangle r_trackCircle = new Rectangle(x, y, TrackDiameter, TrackDiameter);

                g.DrawEllipse(p_trackCircle, r_trackCircle);

                e.Graphics.DrawImage(trackbar, 0, 0);
            }
            else
            {
                e.Graphics.DrawImage(TrackImage, ClientRectangle, 0, 0, TrackImage.Width, TrackImage.Height, GraphicsUnit.Pixel);
            }

            //draw the thumb          
            //determine thumbnail origin
            Point thumbOrigin = GetThumbOriginForPainting();


            //setup intermidate bitmap with neccesary transformation
            Bitmap thumb = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics t = Graphics.FromImage(thumb);
            t.SmoothingMode = SmoothingMode.AntiAlias;

            t.TranslateTransform(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
            t.RotateTransform((float)CurrentValue);

            //draw the thumbnail
            if (ThumbImage != null)
            {
                t.DrawImage(ThumbImage, thumbOrigin.X, thumbOrigin.Y);
            }
            else
            {
                Brush thumbColor = new SolidBrush(Enabled ? ForeColor : DisabledColor);
                t.FillRectangle(thumbColor, thumbOrigin.X, thumbOrigin.Y, _thumbWidth, _thumbHeight);
            }
            
            //draw thumbnail
            e.Graphics.DrawImage(thumb, 0, 0);

            //draw the current value if desired
            if (DisplayValue)
            {
                try
                {
                    double value = CurrentValue;
                    if ((DisplayValue180) && (CurrentValue > 180))
                    {
                        value = CurrentValue - 360;
                    }

                    StringFormat sf = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
                    SizeF strSize = e.Graphics.MeasureString(value.ToString(DisplayValueFormatString), this.Font, ClientRectangle.Width, sf);
                    e.Graphics.DrawString(value.ToString(DisplayValueFormatString),
                                            this.Font,
                                            new SolidBrush(Enabled ? ForeColor : DisabledColor),
                                            (ClientRectangle.Width / 2) - (strSize.Width / 2) + 5,
                                            (ClientRectangle.Height / 2) - (strSize.Height / 2),
                                            sf);
                }
                catch (Exception)
                {
                    throw new Exception("Exception while drawing current value! Check DisplayValueFormatString.");
                }
            }


        }

        /// <summary>
        /// Calculates the origin of the thumb for use in the Paint handler
        /// </summary>
        /// <returns>a Point describing the top-left corner of the thumb</returns>
        private Point GetThumbOriginForPainting()
        {
            int x, y;
            bool useThumbImage = (ThumbImage != null);

            //determine X alignment
            switch (ThumbAlignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    x = -_thumbWidth;
                    break;
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                default:
                    x = -(_thumbWidth / 2);
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    x = 0;
                    break;
            }

            //determine Y alignment
            y = -(TrackDiameter / 2);
            switch (ThumbAlignment)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomRight:
                    break;
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleRight:
                default:
                    y -= _thumbHeight / 2;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopRight:
                    y -= _thumbHeight;
                    break;
            }

            return new Point(x, y);
        }

        private Color _trackColor;
        /// <summary>
        /// Use this property to set the color for the groove color (the ring the track-thumb rests on)
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("The color of the track.")]
        public Color TrackColor
        {
            get { return _trackColor; }
            set { _trackColor = value; }
        }

        private Image _trackImage;
        /// <summary>
        /// Define an image to draw for the track.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("When set, draws an image instead of the circular track. See TrackDiameter")]
        public Image TrackImage
        {
            get { return _trackImage; }
            set { _trackImage = value; }
        }

        private Image _thumbImage;
        /// <summary>
        /// Define an image to draw for the thumb.
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("When set, draws an image for the thumb, instead of the normal rectangle.")]
        public Image ThumbImage
        {
            get { return _thumbImage; }
            set
            {
                _thumbImage = value;
                if (_thumbImage != null)
                {
                    _thumbWidth = ThumbImage.Width;
                    _thumbHeight = ThumbImage.Height;
                }
                else
                {
                    _thumbWidth = (int)(TrackWidth * 1.5);
                    _thumbHeight = TrackWidth * 3;
                }
            }
        }

        private ContentAlignment _thumbAlignment;
        [Category("Appearance"), Browsable(true), Description("The alignment of the thumbnail relative to the track.")]
        public ContentAlignment ThumbAlignment
        {
            get { return _thumbAlignment; }
            set
            {
                _thumbAlignment = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// This event is fired whenever the Value property is changed.
        /// </summary>
        [Category("Action"), Browsable(true), Description("Fired when the Value property has changed.")]
        public event EventHandler ValueChanged;

        private double _value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Category("Behavior"),Browsable(true)]
        public double Value
        {
            get { return _value; }
            set
            {
                _value = FitValueToCircle(value);
                CurrentValue = Value;
                this.Invalidate();
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }
            }
        }

        private double _currentValue;
        /// <summary>
        /// This property is used to track the current value of the thumb before the change is finalized.
        /// If the thumb is not in transition, this will be the same as Value
        /// </summary>
        /// <remarks>
        /// If responding to the Scroll event, you must check this property for the value of the thumb.
        /// ValueChanged will be fired when scrolling is compelete.
        /// </remarks>
        [Category("Behavior"), Browsable(true), Description("The value where the thumb is moving.")]
        public double CurrentValue
        {
            get { return _currentValue; }
            private set
            {
                _currentValue = FitValueToCircle(value);
                this.Invalidate();
            }
        }

        private bool _showValueInCenter;
        /// <summary>
        /// If this property is set to true, the CurrentValue of the control will be displayed in
        /// the middle of the control.
        /// </summary>
        /// <remarks>
        /// Use the DisplayValueFormatString to change the way the numbers are formatted.
        /// Use the Font and ForeColor to change the text.
        /// </remarks>
        [Category("Appearance"), Browsable(true), Description("If set, the CurrentValue will be displayed in the middle of the control")]
        public bool DisplayValue
        {
            get { return _showValueInCenter; }
            set
            {
                _showValueInCenter = value;
                this.Invalidate();
            }
        }

        private string _displayValueFormatString;
        [Category("Appearance"), Browsable(true), Description("A format string that will be applied to the CurrentValue. See DisplayValue")]
        public string DisplayValueFormatString
        {
            get { return _displayValueFormatString; }
            set
            {
                _displayValueFormatString = value;
                this.Invalidate();
            }
        }

        private bool _display180;
        /// <summary>
        /// Set to true to display the value in the range of (-180, 180]. When set to false, it is displayed in the default range of [0, 360).
        /// </summary>
        [Category("Appearance"), Browsable(true), Description("When set to true, the value will be shown in the range (-180, 180] instead of [0, 360).")]
        public bool DisplayValue180
        {
            get
            {
                return _display180;
            }
            set
            {
                _display180 = value;
                this.Invalidate();
            }
        }



        /// <summary>
        /// Forces the input value into the range of [0,360)
        /// </summary>
        /// <param name="value">any positive or negative number of degrees</param>
        /// <returns>the equal representation in the range of [0,360)</returns>
        public static double FitValueToCircle(double value)
        {
            if (value >= 360)
                return value % 360;
            else if (value < 0)
                return 360 + (value % 360);

            return value;
        }

        /// <summary>
        /// Converts Degrees to Radians
        /// </summary>
        /// <param name="degrees">degrees</param>
        /// <returns>radians</returns>
        private double DegToRad(double degrees)
        {
            return (degrees / 360) * (Math.PI * 2);
        }

        /// <summary>
        /// Converts Radians to Degrees
        /// </summary>
        /// <param name="radians">radians</param>
        /// <returns>degrees</returns>
        private double RadToDeg(double radians)
        {
            return (radians / (Math.PI * 2)) * 360;
        }

        /// <summary>
        /// Returns the X/Y coordinates for the point on a circle of a given <paramref>radius</paramref>,
        /// at a particular <paramref>angleInDegrees</paramref>
        /// </summary>
        /// <param name="angleInDegrees">angle in degrees, where 0 is at the top of the circle, and increases positively clockwise</param>
        /// <param name="radius">radius of the circle located at the center of <paramref>coordPlane</paramref></param>
        /// <param name="coordPlane">the rectangle that contains the circle, assuming the circle is centered in this rectangle</param>
        /// <returns></returns>
        private Point AngleToCoords(double angleInDegrees, int radius, Rectangle coordPlane)
        {
            //calculate point on circle indicated by angle
            int cX, cY;
            cX = (int)(radius * Math.Sin(DegToRad(angleInDegrees)));
            cX += (coordPlane.Width / 2);
            cY = (int)(radius * Math.Cos(DegToRad(angleInDegrees)));
            if (cY > 0)
                cY = (coordPlane.Height / 2) - cY;
            else
                cY = (-1 * cY) + (coordPlane.Height / 2);

            return new Point(cX, cY);
        }

        /// <summary>
        /// Converts a point's <paramref>coords</paramref> within a given <paramref>coordPlane</paramref>,
        /// </summary>
        /// <param name="coords">coordinates to find angle for</param>
        /// <param name="coordPlane">a rectangle that has a circle centered in it</param>
        /// <returns></returns>
        private double CoordsToAngle(Point coords, Rectangle coordPlane)
        {
            double angle;
            //convert origin to middle of image
            int x = coords.X - (coordPlane.Width / 2);
            int y = coords.Y - (coordPlane.Height / 2);
            y *= -1;

            angle = RadToDeg(Math.Atan2(x, y));

            if ((x < 0) && (angle < 180) && (angle > 0))
                angle += 180;

            return angle;
        }

        /// <summary>
        /// Calculates the distance from the center
        /// </summary>
        /// <param name="coords">coordinates to determine distance</param>
        /// <param name="coordPlane">plane where the circle is centered on</param>
        /// <returns>The distance from the center in pixels</returns>
        private int DistanceFromCenter(Point coords, Rectangle coordPlane)
        {
            int x = coords.X - (coordPlane.Width / 2);
            int y = -1 * (coords.Y - coordPlane.Height / 2);

            return (int)Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// Called when the mouse button is pressed down
        /// </summary>
        /// <remarks>
        /// If the mouse button is pressed on the track-thumb, then we begin changing the CurrentValue
        /// as we receive MouseMove events.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FCCircularTrackbar_MouseDown(object sender, MouseEventArgs e)
        {
            if(PointInTrackBounds(e.Location, ClientRectangle))
            {
                MouseCapture();
                CalculateCurrentValue(e.Location);
                if (Scroll != null)
                {
                    Scroll(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Determines if a point is within the Track's bounds, e.g. if the track should respond to clicks
        /// at this point
        /// </summary>
        /// <param name="point">the point in question</param>
        /// <param name="rect">the rectangle containing the track, centered within it</param>
        /// <returns>true if the point is in the track's bounds, false if not</returns>
        private bool PointInTrackBounds(Point point, Rectangle rect)
        {
            int clickRadius = DistanceFromCenter(point, rect);

            return ((clickRadius > ((TrackDiameter / 2) - _thumbHeight)) &&
                    (clickRadius < ((TrackDiameter / 2) + _thumbHeight)));
        }

        /// <summary>
        /// Called when the mouse button is released
        /// </summary>
        /// <remarks>
        /// If we are currently "listening" to the mouses's movements, stop
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FCCircularTrackbar_MouseUp(object sender, MouseEventArgs e)
        {
            if(HasMouse)
                MouseRelease();
        }

        /// <summary>
        /// The Scroll event is fired while the track-thumb is moving. Access the CurrentValue property
        /// for the current position. Value is set when the user stops moving the track-thumb, and
        /// ValueChanged is fired.
        /// </summary>
        [Category("Action"), Browsable(true), Description("This event is fired while the thumb is moving. See CurrentValue for the current value. See Value for the last-set value.")]
        public new event EventHandler Scroll;

        /// <summary>
        /// Called when the mouse moves 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FCCircularTrackbar_MouseMove(object sender, MouseEventArgs e)
        {
            if (HasMouse)
            {
                CalculateCurrentValue(e.Location);
                if (Scroll != null)
                {
                    Scroll(this, new EventArgs());
                }
            }
            else if (PointInTrackBounds(e.Location, ClientRectangle))
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = this.DefaultCursor;
            }
        }
        
        /// <summary>
        /// Indicates that we should start caring about the mouse movements.
        /// </summary>
        private void MouseCapture()
        {
            this.Cursor = Cursors.Hand;
            HasMouse = true;
        }

        /// <summary>
        /// Indicates that we should no longer be caring what the mouse does.
        /// </summary>
        private void MouseRelease()
        {
            this.Cursor = this.DefaultCursor;
            HasMouse = false;
            Value = CurrentValue;
        }

        private bool _hasMouse;
        /// <summary>
        /// This property is used to determine if the user is holding down the mouse button
        /// on the track-thumb.
        /// </summary>
        private bool HasMouse
        {
            get { return _hasMouse; }
            set { _hasMouse = value; }
        }

        /// <summary>
        /// Calculates and sets the current value that is closest to the point specified in the control
        /// </summary>
        /// <remarks>
        /// Causes the control to re-draw.
        /// </remarks>
        /// <param name="point">the Point to calculate for</param>
        private void CalculateCurrentValue(Point point)
        {
            CurrentValue = CoordsToAngle(point, ClientRectangle);

            this.Invalidate();
        }

        private void FCCircularTrackbar_Resize(object sender, EventArgs e)
        {
            int hPadding = Padding.Left + Padding.Right;
            int vPadding = Padding.Top + Padding.Bottom;
            int maxDiameter = ((ClientRectangle.Width - hPadding) < (ClientRectangle.Height - vPadding)) ? ClientRectangle.Width - hPadding : ClientRectangle.Height - vPadding;
            
            TrackDiameter = (maxDiameter - (TrackWidth * 2));
        }
    }
}
