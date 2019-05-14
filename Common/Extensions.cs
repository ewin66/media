
namespace FutureConcepts.Media
{
    /// <summary>
    /// Extensions for classes in Media
    /// </summary>
    /// <author>kdixon 11/16/2009</author>
    public static class Extensions
    {
        /// <summary>
        /// Gets the most common file extension associated with the container codec
        /// </summary>
        /// <param name="codec">codec to get file extension for</param>
        /// <returns>a string for the extension, or an empty string</returns>
        public static string GetFileExtension(this ContainerCodecType codec)
        {
            switch (codec)
            {
                case ContainerCodecType.MPEG_PS:
                case ContainerCodecType.MPEG2_TS:
                    return ".mpg";
                case ContainerCodecType.MP4:
                    return ".mp4";
                case ContainerCodecType.ThreeGPP:
                    return ".3gp";
                case ContainerCodecType.ThreeGPP2:
                    return ".3g2";
                case ContainerCodecType.LTStream:
                    return ".lts";
                case ContainerCodecType.AVI:
                    return ".avi";
                case ContainerCodecType.ASF:
                    return ".asf";
                case ContainerCodecType.OGG:
                    return ".ogg";
                case ContainerCodecType.OGM:
                    return ".ogm";
                case ContainerCodecType.QuickTime:
                    return ".mov";
                case ContainerCodecType.Matroska:
                    return ".mkv";
                case ContainerCodecType.Flash:
                    return ".flv";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns an english human-readable display string for an encryption type
        /// </summary>
        /// <param name="encryptionType">type to get string for</param>
        /// <returns>string to display for the given type</returns>
        public static string ToDisplayString(this EncryptionType encryptionType)
        {
            switch (encryptionType)
            {
                case EncryptionType.AES_Legacy:
                    return "AES (Legacy)";
                case EncryptionType.AES_Bcrypt:
                    return "AES (Bcrypt)";
                case EncryptionType.BISS_1:
                    return "BISS-1";
                case EncryptionType.BISS_E:
                    return "BISS-E";
                case EncryptionType.TripleDES:
                    return "Triple-DES";
                default:
                    return encryptionType.ToString();
            }
        }
    }
}
