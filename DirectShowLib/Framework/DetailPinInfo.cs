using System;
using FutureConcepts.Media.DirectShowLib;
using FutureConcepts.Media.DirectShowLib.Framework;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.DirectShowLib
{
    /// <summary>
    /// Encapsulates all relevant information for a particular pin
    /// </summary>
    /// <author>kdixon 12/22/2009</author>
    /// <remarks>This class only exposes the first advertised AMMediaType for a pin.</remarks>
    public class DetailPinInfo : IDisposable
    {
        /// <summary>
        /// Constructs a blank DetailPinInfo
        /// </summary>
        public DetailPinInfo() { }

        /// <summary>
        /// Constructs a DetailPinInfo for the specified pin
        /// </summary>
        /// <param name="pin">IPin interface to get info about</param>
        public DetailPinInfo(IPin pin)
        {
            PinInfo pinInfo = new PinInfo();
            int hr = pin.QueryPinInfo(out pinInfo);
            DsError.ThrowExceptionForHR(hr);

            IEnumMediaTypes enumMediaTypes;
            hr = pin.EnumMediaTypes(out enumMediaTypes);
            DsError.ThrowExceptionForHR(hr);

            AMMediaType[] curMediaType = new AMMediaType[1];

            IntPtr fetched2 = Marshal.AllocCoTaskMem(4);
            try
            {
                if (enumMediaTypes.Next(1, curMediaType, fetched2) == 0)
                {
                    if (Marshal.ReadInt32(fetched2) != 1)
                    {
                        throw new Exception("Cannot enumerate media types for pin!");

                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(fetched2);
            }

            this.Pin = pin;
            this.Info = pinInfo;
            this.Type = curMediaType[0];

            BaseDSGraph.Release(enumMediaTypes);
        }

        /// <summary>
        /// Interface to the pin itself
        /// </summary>
        public IPin Pin { get; set; }

        /// <summary>
        /// The PinInfo additional information
        /// </summary>
        public PinInfo Info { get; set; }

        /// <summary>
        /// The AMMediaType information
        /// </summary>
        public AMMediaType Type { get; set; }

        /// <summary>
        /// Releases all refernces and memory held by this structure
        /// </summary>
        public void Dispose()
        {
            BaseDSGraph.Release(this.Pin);
            DsUtils.FreePinInfo(this.Info);
            if (this.Type != null)
            {
                DsUtils.FreeAMMediaType(this.Type);
                this.Type = null;
            }
        }

        /// <summary>
        /// Releases all resources except the reference to IBaseFilter in the PinInfo.
        /// This is because only 1 reference exists in the RCW, so if you were to query the
        /// pin info twice, and release it twice, any future touching of the original
        /// filter would explode.
        /// </summary>
        public void ReleaseExceptingFilter()
        {
            BaseDSGraph.Release(this.Pin);
            if (this.Type != null)
            {
                DsUtils.FreeAMMediaType(this.Type);
                this.Type = null;
            }
            this.Info = default(PinInfo);
        }
    }
}
