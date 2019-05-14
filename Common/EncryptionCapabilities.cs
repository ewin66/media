using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Used to describe the capabilities of an enryption system
    /// </summary>
    /// <author>kdixon 02/02/2011</author>
    [Serializable]
    [DataContract]
    public class EncryptionCapabilities
    {
        private Dictionary<EncryptionType, List<int>> _supportKeyLen;
        /// <summary>
        /// Contains the supported types of encryption in the Keys, and each type's supported
        /// key lengths (in bits) in the Value list.
        /// </summary>
        [DataMember]
        public Dictionary<EncryptionType, List<int>> SupportedTypesAndKeyLengths
        {
            get
            {
                return _supportKeyLen;
            }
            set
            {
                _supportKeyLen = value;
            }
        }

        /// <summary>
        /// Accessor for supported types of encryption
        /// </summary>
        [IgnoreDataMember]
        public IEnumerable<EncryptionType> SupportedTypes
        {
            get
            {
                if (SupportedTypesAndKeyLengths != null)
                {
                    return SupportedTypesAndKeyLengths.Keys;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
