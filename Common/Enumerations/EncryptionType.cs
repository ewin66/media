using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public enum EncryptionType
    {
        /// <summary>
        /// unknown
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// no encryption
        /// </summary>
        None = 0,
        /// <summary>
        /// AES Legacy
        /// </summary>
        AES_Legacy,
        /// <summary>
        /// AES Bcrypt
        /// </summary>
        AES_Bcrypt,
        /// <summary>
        /// Basic Interoperable Scrambling System 1
        /// </summary>
        BISS_1,
        /// <summary>
        /// Basic Interoperable Scrambling System E
        /// </summary>
        BISS_E,
        /// <summary>
        /// Data Encryption Standard
        /// </summary>
        DES,
        /// <summary>
        /// Data Encryption Standard, applied 3 times per block
        /// </summary>
        TripleDES,
        /// <summary>
        /// Blowfish
        /// </summary>
        Blowfish,
        /// <summary>
        /// Twofish
        /// </summary>
        Twofish,
        
    }
}
