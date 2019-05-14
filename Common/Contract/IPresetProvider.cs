using System;
using System.ServiceModel;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// This is a generic interface for classes that provide presets of any kind.
    /// It can also be used as a contract for WCF services that provide presets.
    /// kdixon 05/27/2009
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IPresetProvider
    {
        /// <summary>
        /// Indicates that the provider should use its current settings to save a preset.
        /// </summary>
        /// <returns>
        /// The UserPresetItem that can be used to perform further operations on the new preset.
        /// Returns null if the operation cannot be performed.
        /// </returns>
        [OperationContract]
        UserPresetItem SavePreset();

        /// <summary>
        /// Indicates that the provider should restore the settings associated with the preset identifier indicated.
        /// </summary>
        /// <param name="id">preset identifier</param>
        [OperationContract(IsOneWay = true)]
        void RestorePreset(Guid id);

        /// <summary>
        /// Indicates that the provider should update the fields in the passed item (matching id) to the ones passed in.
        /// Does nothing if a preset with the passed in ID is not known.
        /// </summary>
        /// <param name="updatedItem">Item to update. ID field must match a currently known item.</param>
        /// <returns>true on success, false if the item did not exist.</returns>
        [OperationContract]
        bool UpdatePreset(UserPresetItem updatedItem);

        /// <summary>
        /// Indicates that the provider should delete the preset with given ID.
        /// </summary>
        /// <param name="id">ID of the preset to delete</param>
        /// <returns>true on success, false if the item did not exist</returns>
        [OperationContract]
        bool DeletePreset(Guid id);

        /// <summary>
        /// Indicates that the provider should delete all presets.
        /// </summary>
        [OperationContract]
        void DeleteAllPresets();
    }
}
