using System;

namespace FutureConcepts.Media.Contract
{
    /// <summary>
    /// This interface is an addition to the normal IPresetProvider that allows it to expose its collection of items.
    /// </summary>
    public interface IPresetProviderItems : IPresetProvider
    {
        /// <summary>
        /// Gets or sets the items associated with this provider.
        /// </summary>
        UserPresetStore PresetItems { get; set; }
    }
}
