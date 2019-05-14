using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Used to describe levels of compression quality
    /// </summary>
    [Serializable]
    public enum VideoQuality
    {
        /// <summary>
        /// Lossless
        /// </summary>
        Lossless = 1,
        /// <summary>
        /// 18:1
        /// </summary>
        PerfectQuality_1 = 18,
        /// <summary>
        /// 24:1
        /// </summary>
        PerfectQuality_2 = 24,
        /// <summary>
        /// 30:1
        /// </summary>
        QualityFarMoreImportantThanSize = 30,
        /// <summary>
        /// 40:1
        /// </summary>
        QualityMoreImportantThanSize = 40,
        /// <summary>
        /// 55:1
        /// </summary>
        QualityAndSizeAreEquallyImportant = 55,
        /// <summary>
        /// 70:1
        /// </summary>
        SizeMoreImportantThanQuality_1 = 70,
        /// <summary>
        /// 90:1
        /// </summary>
        SizeMoreImportantThanQuality_2 = 90,
        /// <summary>
        /// 110:1
        /// </summary>
        HighCompressionKeepQuality = 110,
        /// <summary>
        /// 140:1
        /// </summary>
        HighCompression = 140,
        /// <summary>
        /// 180:1
        /// </summary>
        HighCompressionFast = 180,
        /// <summary>
        /// 220:1
        /// </summary>
        HyperCompressionFast = 220,
        /// <summary>
        /// Undefined, not applicable, or don't care
        /// </summary>
        Undefined = 9999
    }
}
