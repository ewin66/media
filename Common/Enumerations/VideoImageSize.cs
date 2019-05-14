using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Supported resolutions for video streams
    /// </summary>
    [Serializable]
    public enum VideoImageSize
    {
        /// <summary>
        /// Variable, Irrelevant, or Don't Care
        /// </summary>
        Undefined = -1,
        /// <summary>
        /// Sub-Quarter CIF 128 x 96
        /// </summary>
        SQCIF = 0,
        /// <summary>
        /// Quarter CIF 176 x 144
        /// </summary>
        QCIF = 1,
        /// <summary>
        /// Common Intermediate Format 352 x 288
        /// </summary>
        CIF = 2,
        /// <summary>
        /// Double CIF
        /// </summary>
        TwoCIF = 3,
        /// <summary>
        /// Quadruple CIF 704 x 576
        /// </summary>
        FourCIF = 4,
        /// <summary>
        /// Quarter VGA 320 x 240
        /// </summary>
        QVGA = 5,
        /// <summary>
        /// VGA 640 x 480
        /// </summary>
        VGA = 6,
        /// <summary>
        /// Sony D1 NTSC = 720 × 486 PAL = 720 × 576
        /// </summary>
        D1 = 7,
        /// <summary>
        /// ATSC/HDTV 1920 x 1080
        /// </summary>
        HD1080,
        /// <summary>
        /// ATSC/HDTV 1280 x 720
        /// </summary>
        HD720,
        /// <summary>
        /// ATSC/HDTV 704 x 480
        /// </summary>
        HD480
    }
}
