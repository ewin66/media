using System;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Different types of radio-frequency modulation
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    public enum RFModulationType
    {
        /// <summary>
        /// Used when the modulation type is unknown
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Used to auto-detect modulation type, when applicable
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Amplitude Modulation
        /// </summary>
        AM,
        /// <summary>
        /// Frequency Modulation
        /// </summary>
        FM,
        /// <summary>
        /// Quad Phase Shift Keying
        /// </summary>
        QPSK,
        /// <summary>
        /// 8-Phase Shift Keying
        /// </summary>
        PSK8,
        /// <summary>
        /// 16-Phase Shift Keying
        /// </summary>
        PSK16,
        /// <summary>
        /// Analog Quadrature Amplitude Modulation
        /// </summary>
        QAMAnalog,
        /// <summary>
        /// 16 position (4 bit) Quadrature Amplitude Modulation
        /// </summary>
        QAM16,
        /// <summary>
        /// 32 position (5 bit) Quadrature Amplitude Modulation
        /// </summary>
        QAM32,
        /// <summary>
        /// 64 position (6 bit) Quadrature Amplitude Modulation
        /// </summary>
        QAM64,
        /// <summary>
        /// 128 position (7 bit) Quadrature Amplitude Modulation
        /// </summary>
        QAM128,
        /// <summary>
        /// 256 position (8 bit) Quadrature Amplitude Modulation
        /// </summary>
        QAM256,
        /// <summary>
        /// 8 position (3 bit) Vestigal Sideband Modulation
        /// </summary>
        VSB8,
        /// <summary>
        /// 16 position (4 bit) Vestigal Sideband Modulation
        /// </summary>
        VSB16
    }
}
