using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// Extension methods for the Microwave related classes in the Contract namespace
    /// </summary>
    /// <author>kdixon 02/16/2011</author>
    public static class MicrowaveExtensions
    {
        /// <summary>
        /// Checks if a parameter is set on a MicrowaveLinkQuality object
        /// </summary>
        /// <param name="linkQuality">LQ to check</param>
        /// <param name="parameter">parameter to check</param>
        /// <returns>true if the specified parameter is set, false otherwise</returns>
        public static bool IsSet(this MicrowaveLinkQuality linkQuality, MicrowaveLinkQuality.Parameters parameter)
        {
            return MicrowaveLinkQuality.IsParameterValid(linkQuality.ValidParameterData, parameter);
        }

        /// <summary>
        /// Checks if a MicrowaveLinkQuality parameter is set on MicrowaveCapabilities.SupportedLinkQualityParameters property
        /// </summary>
        /// <param name="caps">capabilities to check</param>
        /// <param name="linkQualityParameter">link quality parameter to check for</param>
        /// <returns>true if set in MicrowaveCapabilities.SupportedLinkQualityParameters, false otherwise</returns>
        public static bool IsSet(this MicrowaveCapabilities caps, MicrowaveLinkQuality.Parameters linkQualityParameter)
        {
            return MicrowaveLinkQuality.IsParameterValid(caps.SupportedLinkQualityParameters, linkQualityParameter);
        }

        /// <summary>
        /// Checks if a MicrowaveTuning parameter is set on MicrowaveCapabilities.SupportedTuningParameters property
        /// </summary>
        /// <param name="caps">capabilities to check</param>
        /// <param name="tuningParameter">tuning parameter to check for</param>
        /// <returns>true if set in MicrowaveCapabilities.SupportedTuningParameters, false otherwise</returns>
        public static bool IsSet(this MicrowaveCapabilities caps, MicrowaveTuning.Parameters tuningParameter)
        {
            return MicrowaveTuning.IsParameterValid(caps.SupportedTuningParameters, tuningParameter);
        }

        /// <summary>
        /// Checks if a parameter is set on a MicrowaveTuning object
        /// </summary>
        /// <param name="tuning">tuning to check</param>
        /// <param name="parameter">parameter to check for</param>
        /// <returns>true if the parameter is set, false otherwise</returns>
        public static bool IsSet(this MicrowaveTuning tuning, MicrowaveTuning.Parameters parameter)
        {
            return MicrowaveTuning.IsParameterValid(tuning.ValidParameterData, parameter);
        }

        /// <summary>
        /// Returns each set flag in the specified Tuning Parameters
        /// </summary>
        /// <param name="tuningParams">tuning parameters</param>
        /// <returns>an enumerable of all the set flags</returns>
        public static IEnumerable<MicrowaveTuning.Parameters> EnumerateSetFlags(this MicrowaveTuning.Parameters tuningParams)
        {
            foreach (int value in Enum.GetValues(typeof(MicrowaveTuning.Parameters)))
            {
                if ((value & (int)tuningParams) == value)
                {
                    yield return (MicrowaveTuning.Parameters)value;
                }
            }
        }

        /// <summary>
        /// Returns each set flag in the specified Link Quality Parameters
        /// </summary>
        /// <param name="linkParams">link parameters</param>
        /// <returns>an enumerable of all the set flags</returns>
        public static IEnumerable<MicrowaveLinkQuality.Parameters> EnumerateSetFlags(this MicrowaveLinkQuality.Parameters linkParams)
        {
            foreach (int value in Enum.GetValues(typeof(MicrowaveLinkQuality.Parameters)))
            {
                if ((value & (int)linkParams) == value)
                {
                    yield return (MicrowaveLinkQuality.Parameters)value;
                }
            }
        }
    }
}
