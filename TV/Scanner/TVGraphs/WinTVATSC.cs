using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using FutureConcepts.Media.TV.Scanner.Config;
using FutureConcepts.Media.TV.AtscPsip;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.BDA;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    /// <summary>
    /// Happauage PVR 1600 -- ATSC
    /// </summary>
    public class WinTVATSC : ATSCTuner
    {
        public WinTVATSC(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
 
        }
   
        public override int GetSignalStrength()
        {
            int baseResult = base.GetSignalStrength();
            if (baseResult == 0)
            {
                return 0;
            }

            baseResult = (-1 * baseResult) + 4000;
            if (baseResult == 0)
            {
                baseResult += 1;
            }
            return baseResult;
        }
    }
}
























