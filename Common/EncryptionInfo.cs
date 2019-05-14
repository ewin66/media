using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Summarizes encryption information to be shared. This is a *plaintext* storage!
    /// </summary>
    /// <author>kdixon 02/01/2011</author>
    [Serializable]
    [DataContract]
    public class EncryptionInfo
    {
        /// <summary>
        /// Creates a blank EncryptionInfo
        /// </summary>
        public EncryptionInfo() { }

        /// <summary>
        /// Clones an existing EncryptionInfo
        /// </summary>
        /// <param name="clone">info to clone</param>
        public EncryptionInfo(EncryptionInfo clone)
        {
            if (clone.DecryptionKey != null)
            {
                this.DecryptionKey = new byte[clone.DecryptionKey.Length];
                Array.Copy(clone.DecryptionKey, this.DecryptionKey, clone.DecryptionKey.Length);
            }
            if (clone.EncryptionKey != null)
            {
                this.EncryptionKey = new byte[clone.EncryptionKey.Length];
                Array.Copy(clone.EncryptionKey, this.EncryptionKey, clone.EncryptionKey.Length);
            }
            this.KeyLength = clone.KeyLength;
            this.Type = clone.Type;
        }

        private EncryptionType _type;
        /// <summary>
        /// Algorithim
        /// </summary>
        [DataMember]
        public EncryptionType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private int _keyLen;
        /// <summary>
        /// Key length, in bits.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int KeyLength
        {
            get
            {
                return _keyLen;
            }
            set
            {
                _keyLen = value;
            }
        }

        private byte[] _decryptKey;
        /// <summary>
        /// Decryption key to use. MSB first.
        /// </summary>
        [DataMember(IsRequired = false)]
        public byte[] DecryptionKey
        {
            get
            {
                return _decryptKey;
            }
            set
            {
                _decryptKey = value;
            }
        }

        private byte[] _encryptKey;
        /// <summary>
        /// Encrypition key to use, if a non-symmetric algorithm. MSB first.
        /// </summary>
        [DataMember(IsRequired = false)]
        public byte[] EncryptionKey
        {
            get
            {
                return _encryptKey;
            }
            set
            {
                _encryptKey = value;
            }
        }

        /// <summary>
        /// Converts a string of hex digits to the corresponding byte array
        /// </summary>
        /// <param name="hexDigits">digits to parse</param>
        /// <returns>the byte-array representation of the hex digits</returns>
        public static byte[] ParseHex(string hexDigits)
        {
            return ParseHex(hexDigits, hexDigits.Length * 4);
        }

        /// <summary>
        /// Converts a string of hex digits to the corresponding byte array,
        /// forward padding with 0-bytes to acheive the specified keylength in bits
        /// </summary>
        /// <param name="hexDigits">digits to parse</param>
        /// <param name="keyLengthBits">target key length, in bits</param>
        /// <returns></returns>
        public static byte[] ParseHex(string hexDigits, int keyLengthBits)
        {
            byte[] o = new byte[keyLengthBits / 4];

            if (o.Length < hexDigits.Length) throw new ArgumentException("Key length is too short for supplied digits");

            int zeroPaddingEnd = o.Length - hexDigits.Length;
            for (int i = 0, k = 0; (i < o.Length) && (k < hexDigits.Length); i++)
            {
                if (i >= zeroPaddingEnd)
                {
                    o[i] = byte.Parse(hexDigits[k].ToString(), System.Globalization.NumberStyles.HexNumber);
                    k++;
                }
            }

            return o;
        }

        /// <summary>
        /// shows a key as a string of hex characters
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ToHex(byte[] key)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in key)
            {
                s.Append(b.ToString("X"));
            }
            return s.ToString();
        }
    }
}
