using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Global Video Standards for Broadcast
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    [Serializable]
    public enum RFVideoStandard
    {
        /// <summary>
        /// Used to represent an unknown television standard
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Auto detect (use only where applicable)
        /// </summary>
        Auto = 0,
        /// <summary>
        /// US NTSC analog
        /// </summary>
        NTSC,
        /// <summary>
        /// German PAL analog
        /// </summary>
        PAL,
        /// <summary>
        /// French SECAM analog
        /// </summary>
        SECAM,
        /// <summary>
        /// ATSC digital
        /// </summary>
        ATSC,
        /// <summary>
        /// ATSC mobile handheld digital
        /// </summary>
        ATSC_MH,
        /// <summary>
        /// DVB-Satellite digital
        /// </summary>
        DVB_S,
        /// <summary>
        /// DVB 2nd Gen Satellite digital
        /// </summary>
        DVB_S2,
        /// <summary>
        /// DVB-Terrestrial digital
        /// </summary>
        DVB_T,
        /// <summary>
        /// DVB 2nd Gen Terrestrial digital
        /// </summary>
        DVB_T2,
        /// <summary>
        /// DVB Cable digital
        /// </summary>
        DVB_C,
        /// <summary>
        /// DVB 2nd Gen Cable digital
        /// </summary>
        DVB_C2,
        /// <summary>
        /// DVB Handheld
        /// </summary>
        DVB_H,
        /// <summary>
        /// DVB Satellite-Handheld
        /// </summary>
        DVB_SH,
        /// <summary>
        /// DVB - Asynchronous Serial Interface. Hi-Speed interface similar to SDI
        /// </summary>
        DVB_ASI,
        /// <summary>
        /// Japan ISDB Satellite
        /// </summary>
        ISDB_S,
        /// <summary>
        /// Japan ISDB Terrestrial
        /// </summary>
        ISDB_T,
        /// <summary>
        /// Japan ISDB handheld
        /// </summary>
        ISDB_1SEG,
        /// <summary>
        /// Japan ISDB cable
        /// </summary>
        ISDB_C,
        /// <summary>
        /// Japan ISDB-T International, aka SBTVD (Brazil)
        /// </summary>
        ISDB_International,
        /// <summary>
        /// China Digital Terrestrial Multimedia Broadcast
        /// </summary>
        DTMB,
        /// <summary>
        /// China Multimedia Mobile Broadcasting
        /// </summary>
        CMMB,
        /// <summary>
        /// So. Korea Digital Multimedia Broadcasting - Terrestrial handheld
        /// </summary>
        DMB_T,
        /// <summary>
        /// So. Korea Digital Multimedia Broadcasting - Satellite handheld
        /// </summary>
        DMB_S,
        /// <summary>
        /// Signal Code Modulation -- Analog+Digital Hybrid
        /// </summary>
        SCM,
        /// <summary>
        /// Signal Code Modulation -- Analog+Digital Hybrid
        /// </summary>
        SCM_QCD,
        /// <summary>
        /// Vislink/MRC/Link Research Proprietary Encoding/Modulation scheme
        /// </summary>
        LMST,
        /// <summary>
        /// Standard Definition SDI (SMPTE 259M); 480i/576i
        /// </summary>
        SDI_SD,
        /// <summary>
        /// Enhanced Definition SDI (SMPTE 344M); 480p/576p
        /// </summary>
        SDI_ED,
        /// <summary>
        /// High Definition SDI (SMPTE 292M); 720p/1080i
        /// </summary>
        SDI_HD,
        /// <summary>
        /// Dual Link High Definition SDI (SMPTE 372M); 1080p
        /// </summary>
        SDI_HD_DualLink,
        /// <summary>
        /// 3G SDI; 1080p
        /// </summary>
        SDI_3G

    }
}
