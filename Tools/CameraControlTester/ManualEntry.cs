using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Tools.CameraControlTester
{
    public partial class ManualEntry : Form
    {
        public ManualEntry()
        {
            InitializeComponent();
        }

        private ICameraControl _client;

        public ICameraControl Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                
            }
        }

        public void ClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentPanAngle":
                    udPan.Value = (decimal)Client.CurrentPanAngle;
                    break;
                case "CurrentTiltAngle":
                    udTilt.Value = (decimal)Client.CurrentTiltAngle;
                    break;
                case "CurrentZoomPosition":
                    udZoom.Value = (decimal)Client.CurrentZoomPosition;
                    break;
            }
        }

        private void btnYeah_Click(object sender, EventArgs e)
        {
            Client.PanTiltAbsolute((double)udPan.Value, (double)udTilt.Value);
            Client.ZoomAbsolute((double)udZoom.Value);
        }
    }
}
