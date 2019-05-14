/*
 *  PanTiltAbsolute.cs
 *  This control replaces the PanTilt control and allows for absolute positioning of the PTZ camera
 *  Kevin Dixon
 *  06/09/2008
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using FutureConcepts.Media.CommonControls;

namespace FutureConcepts.Media.SVD.Controls
{
    public partial class PanTiltZoomAbsoluteControl : UserControl
    {
        private Color DisabledColor = Color.Gray;

        public PanTiltZoomAbsoluteControl()
        {
            TiltAngleFormatString = "0°";
            ZoomLevelFormatString = "0x";

            InitializeComponent();
            TB_panAngle.DisabledColor = DisabledColor;
            TB_tiltAngle.DisabledColor = DisabledColor;
            TB_zoomLevel.DisabledColor = DisabledColor;
        }

        #region Pan Interface

        [Category("Action"), Browsable(true)]
        public event EventHandler PanAngleChanged;

        private void TB_panAngle_ValueChanged(object sender, System.EventArgs e)
        {
            if (PanAngleChanged != null)
            {
                PanAngleChanged(this, e);
            }
        }

        /// <summary>
        /// This property is the actually set value (as communicated to/from the server) of the pan angle.
        /// When setting, PanAngleOffset is subtracted, when getting, PanAngleOffset is added.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("The currently set Pan Angle")]
        public double PanAngle
        {
            get
            {
                //make sure this value is in the proper range
                return FCCircularTrackbar.FitValueToCircle(TB_panAngle.Value + PanAngleOffset);
            }
            set
            {
                TB_panAngle.Value = value - PanAngleOffset;
            }
        }

        private double _panAngleOffset = 0.0;
        [Category("Behavior"), Browsable(true), Description("An offset to apply when displaying pan angles.")]
        public double PanAngleOffset
        {
            get
            {
                return _panAngleOffset;
            }
            set
            {
                _panAngleOffset = value;
            }
        }


        #endregion

        #region Pan Backend

#if UNUSED_FUNCTIONALITY
        /// <summary>
        /// Calculates the shortest distance in degrees between two angles
        /// </summary>
        /// <param name="from">starting angle</param>
        /// <param name="to">ending angle</param>
        private void CalcPanAngleDelta(int from, int to)
        {
            PanAngleDelta = -1 * (FCCircularTrackbar.FitValueToCircle((from + 180.0 - to)) - 180);
        }


        /// <summary>
        /// Calculates the distance from zero of a given angle.
        /// </summary>
        /// <param name="angle">angle to calculate</param>
        /// <returns>an angle in the range (-180,180]</returns>
        private int AngleDistanceFromZero(int angle)
        {
            angle = (int)FCCircularTrackbar.FitValueToCircle((int)angle);
            if (angle > 180)
                angle = -(360 - angle);

            return angle;
        }
#endif

        private double _panNudgeAmount;
        /// <summary>
        /// The amount to move the pan angle per nudge
        /// </summary>
        private double PanNudgeAmount
        {
            get
            {
                return _panNudgeAmount;
            }
            set
            {
                _panNudgeAmount = value;
            }
        }



        /// <summary>
        /// Called when one of the nudgePan buttons is clicked
        /// </summary>
        /// <param name="sender">the button that was clicked to initiate the nudge</param>
        /// <param name="e">don't care</param>
        private void B_nudgePan_Click(object sender, System.EventArgs e)
        {
            double x = PanNudgeAmount;
            if (sender == B_nudgePanCounterclockwise)
            {
                x *= -1;
            }
            PanAngle += x;
        }

        private void B_nudgePan_MouseEnter(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void B_nudgePan_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = this.DefaultCursor;
        }

        private void B_nudgePan_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }


        #endregion

        #region Tilt Interface

        [Category("Action"), Browsable(true)]
        public event EventHandler TiltAngleChanged;

        private void TB_tiltAngle_ValueChanged(object sender, System.EventArgs e)
        {
            L_tiltValue.Text = (TB_tiltAngle.Value).ToString(TiltAngleFormatString);

            if (TiltAngleChanged != null)
            {
                TiltAngleChanged(this, e);
            }
        }

        private void TB_tiltAngle_Scroll(object sender, System.EventArgs e)
        {
            L_tiltValue.Text = (TB_tiltAngle.CurrentValue).ToString(TiltAngleFormatString);
        }

        [Category("Behavior"), Browsable(true)]
        public double TiltAngle
        {
            get
            {
                return TB_tiltAngle.Value;
            }
            set
            {
                TB_tiltAngle.Value = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public double TiltAngleMaximum
        {
            get
            {
                return TB_tiltAngle.Maximum;
            }
            set
            {
                TB_tiltAngle.Maximum = value;
            }
        }

        /// <summary>
        /// Changes the minimum tilt angle
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public double TiltAngleMinimum
        {
            get
            {
                return TB_tiltAngle.Minimum;
            }
            set
            {
                TB_tiltAngle.Minimum = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public double TiltAngleTickFrequency
        {
            get
            {
                return TB_tiltAngle.TickFrequency;
            }
            set
            {
                TB_tiltAngle.TickFrequency = value;
            }
        }

        [Category("Behavior"), Browsable(false)]
        public double[] TiltAngleCustomTicks
        {
            get
            {
                return TB_tiltAngle.CustomTicks;
            }
            set
            {
                TB_tiltAngle.CustomTicks = value;
            }
        }

        #endregion

        private double _cameraFieldOfView;
        [Category("Behavior"), Browsable(true), Description(@"The Camera's horizontal field of view @ 1x magnification.")]
        public double CameraFieldOfView
        {
            get
            {
                return _cameraFieldOfView;
            }
            set
            {
                _cameraFieldOfView = value;
                CalibrateNudgeValues();
            }
        }

        #region Zoom Interface

        [Category("Action"), Browsable(true)]
        public event EventHandler ZoomLevelChanged;

        private void TB_zoomLevel_ValueChanged(object sender, System.EventArgs e)
        {
            L_zoomValue.Text = (TB_zoomLevel.Value).ToString(ZoomLevelFormatString);

            if (ZoomLevelChanged != null)
            {
                ZoomLevelChanged(this, e);
            }

            CalibrateNudgeValues();
        }

        private void TB_zoomLevel_Scroll(object sender, System.EventArgs e)
        {
            L_zoomValue.Text = (TB_zoomLevel.CurrentValue).ToString(ZoomLevelFormatString);
        }

        [Category("Behavior"), Browsable(true)]
        public double ZoomLevel
        {
            get
            {
                return TB_zoomLevel.Value;
            }
            set
            {
                TB_zoomLevel.Value = value;
                CalibrateNudgeValues();
            }
        }

        /// <summary>
        /// Returns the currently calculated horizontal field of view.
        /// </summary>
        /// <returns>0 if it cannot be determined</returns>
        public double GetCurrentFieldOfView()
        {
            if ((ZoomLevel == 0) || (CameraFieldOfView == 0))
            {
                return 0;
            }
            else
            {
                return CameraFieldOfView / ZoomLevel;
            }
        }

        /// <summary>
        /// Call this method to re-calibrate the nudge values based on the currently set ZoomLevel
        /// </summary>
        private void CalibrateNudgeValues()
        {
            //horizontal field of view - degrees @ current zoom level
            double hfov = GetCurrentFieldOfView();

            //if we can't calculate nudge values, use some that are useful at high zooms
            if (hfov == 0)
            {
                PanNudgeAmount = 0.5;
                TB_tiltAngle.TrackClickStepValue = 0.2;
                TB_zoomLevel.TrackClickStepValue = 4;
                return;
            }

            //amount of screen to shift
            double relativeChangeInFov = 1.0 / 8.0;

            //calculate vertical field of view, assume 4:3 ratio
            double vfov = (4.0 * hfov) / 3.0;

            PanNudgeAmount = hfov * relativeChangeInFov;
            //TODO figure out good thresholds and/or calculate
            if (PanNudgeAmount < 0.3)
            {
                PanNudgeAmount = 0.3;
            }

            TB_tiltAngle.TrackClickStepValue = vfov * relativeChangeInFov;
            //TODO figure out good thresholds and/or calculate
            if (TB_tiltAngle.TrackClickStepValue < 0.2)
            {
                TB_tiltAngle.TrackClickStepValue = 0.2;
            }

            TB_zoomLevel.TrackClickStepValue = 4.0 / ZoomLevel;
        }

        [Category("Behavior"), Browsable(true)]
        public double ZoomLevelMaximum
        {
            get
            {
                return TB_zoomLevel.Maximum;
            }
            set
            {
                TB_zoomLevel.Maximum = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public double ZoomLevelMinimum
        {
            get
            {
                return TB_zoomLevel.Minimum;
            }
            set
            {
                TB_zoomLevel.Minimum = value;
            }
        }

        [Category("Behavior"), Browsable(true)]
        public double ZoomLevelTickFrequency
        {
            get
            {
                return TB_zoomLevel.TickFrequency;
            }
            set
            {
                TB_zoomLevel.TickFrequency = value;
            }
        }

        [Category("Behavior"), Browsable(false)]
        public double[] ZoomLevelCustomTicks
        {
            get
            {
                return TB_zoomLevel.CustomTicks;
            }
            set
            {
                TB_zoomLevel.CustomTicks = value;
            }
        }

        #endregion

        #region GUI Properties

        /// <summary>
        /// Changes the enabled state of the pan controls
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public bool PanEnabled
        {
            get
            {
                return TB_panAngle.Enabled;
            }
            set
            {
                TB_panAngle.Enabled = value;
                L_panAngle.ForeColor = value ? this.ForeColor : DisabledColor;
                B_nudgePanClockwise.Enabled = value;
                B_nudgePanCounterclockwise.Enabled = value;
            }
        }

        /// <summary>
        /// Changes the enabled state of the tilt controls
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public bool TiltEnabled
        {
            get
            {
                return TB_tiltAngle.Enabled;
            }
            set
            {
                TB_tiltAngle.Enabled = value;
                L_tiltAngle.ForeColor = value ? this.ForeColor : DisabledColor;
                L_tiltValue.ForeColor = value ? this.ForeColor : DisabledColor;
            }
        }

        /// <summary>
        /// Changes the enabled state of zoom controls
        /// </summary>
        [Category("Behavior"), Browsable(true)]
        public bool ZoomEnabled
        {
            get
            {
                return TB_zoomLevel.Enabled;
            }
            set
            {
                TB_zoomLevel.Enabled = value;
                L_zoom.ForeColor = value ? this.ForeColor : DisabledColor;
                L_zoomValue.ForeColor = value ? this.ForeColor : DisabledColor;
            }
        }

        /// <summary>
        /// Use this property to change the way the Pan Angle is formatted
        /// </summary>
        [Category("Appearance"), Browsable(true)]
        public string PanAngleFormatString
        {
            get { return TB_panAngle.DisplayValueFormatString; }
            set { TB_panAngle.DisplayValueFormatString = value; }
        }

        /// <summary>
        /// Use this property to change the way the Tilt Angle is formatted
        /// </summary>
        private string _tiltAngleFormatString;
        [Category("Appearance"), Browsable(true)]
        public string TiltAngleFormatString
        {
            get { return _tiltAngleFormatString; }
            set { _tiltAngleFormatString = value; }
        }

        /// <summary>
        /// Use this property to change the way the Zoom Level is formatted
        /// </summary>
        private string _zoomLevelFormatString;
        [Category("Appearance"), Browsable(true)]
        public string ZoomLevelFormatString
        {
            get { return _zoomLevelFormatString; }
            set { _zoomLevelFormatString = value; }
        }

        /// <summary>
        /// Forwards font changes to internal controls
        /// </summary>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                L_panAngle.Font = value;
                TB_panAngle.Font = value;
                L_tiltAngle.Font = value;
                L_zoom.Font = value;
                L_zoomValue.Font = value;
            }
        }

        /// <summary>
        /// Forwards fore-color changes to internal controls
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                TB_panAngle.ForeColor = value;
                L_panAngle.ForeColor = value;
                
                L_tiltAngle.ForeColor = value;
                TB_tiltAngle.ForeColor = value;
                L_tiltValue.ForeColor = value;

                L_zoom.ForeColor = value;
                TB_zoomLevel.ForeColor = value;
                L_zoomValue.ForeColor = value;
            }
        }

        #endregion
    }
}
