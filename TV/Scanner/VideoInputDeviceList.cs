using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Media.TV.Scanner.Config;
using System.Configuration;
using FutureConcepts.Media.DirectShowLib;

namespace FutureConcepts.Media.TV.Scanner
{
    class VideoInputDeviceList
    {
        /// <summary>
        /// The Source.Name for a network device
        /// </summary>
        public static readonly string NetworkDeviceName = "Network";

        public VideoInputDeviceList()
        {
            PopulateSupportedDevices();
        }

        /// <summary>
        /// Refreshes the list of available video input devices.
        /// </summary>
        /// <remarks>
        /// Only populates devices that we have a configuration entry for
        /// </remarks>
        public void PopulateSupportedDevices()
        {
            _deviceList.Clear();
            SourcesConfig config = SourcesConfig.LoadFromFile();
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                if (config.HasSource(device.Name))
                {
                    _deviceList.Add(device.Name);
                }
            }

            if (config.HasSource(NetworkDeviceName))
            {
                _deviceList.Add(NetworkDeviceName);
                HasNetworkDevice = true;
            }
        }

        public void PopulateAllDevices()
        {
            _deviceList.Clear();
            SourcesConfig config = SourcesConfig.LoadFromFile();
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                 _deviceList.Add(device.Name);
            }
        }

        /// <summary>
        /// Returns true if a device by a given name is in the list.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if the in the list, false if not</returns>
        public bool HasDevice(string name)
        {
            foreach (string s in Items)
            {
                if (s.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }

        private bool _hasNetworkDevice = false;
        /// <summary>
        /// True if the Network device is in the device list.
        /// </summary>
        public bool HasNetworkDevice
        {
            get
            {
                return _hasNetworkDevice;
            }
            private set
            {
                _hasNetworkDevice = value;
            }
        }

        List<string> _deviceList = new List<string>();
        public List<string> Items
        {
            get { return _deviceList; }
        }

        /// <summary>
        /// True if there are no devices on the system that we have configured.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _deviceList.Count == 0;
            }
        }
    }
}
