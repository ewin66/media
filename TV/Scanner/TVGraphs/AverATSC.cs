using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// BDA implementation for ATSC reception on AVerMedia A3xx series cards
    /// </summary>
    public class AverATSC : ATSCTuner
    {
        public AverATSC(Config.Graph sourceConfig, Control host)
            : base(sourceConfig, host)
        {
        }

        /// <summary>
        /// Tries to Set the indicated channel. Gets the actually set channel. Returns null if no Major channel was found on this physical channel.
        /// </summary>
        public override Channel TryChannel
        {
            set
            {
                SetCurrentChannel(value);
            }
            get
            {
                return GetCurrentChannel();
            }
        }

        /// <summary>
        /// good signal strengths are typically 5000000 and higher.
        /// </summary>
        /// <returns></returns>
        public override int GetSignalStrength()
        {
            int str = base.GetSignalStrength();
            if (str > 0 && str < 1000000)
            {
                Debug.WriteLine("AverATSC: signal too weak (" + str + "), zeroing out...");
                return 0;
            }
            return str;
        }

        //public override List<Channel> GetVirtualChannels()
        //{
        //    List<Channel> temp = base.GetVirtualChannels();
        //    if (temp.Count > 0)
        //    {
        //        Channel physical = GetCurrentChannel();
        //        foreach (Channel c in temp)
        //        {
        //            c.CarrierFrequency = physical.CarrierFrequency;
        //            c.PhysicalChannel = physical.PhysicalChannel;
        //        }
        //    }
        //    return temp;
        //}
    }
}
