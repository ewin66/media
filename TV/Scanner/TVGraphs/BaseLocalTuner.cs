using System;
using System.Collections.Generic;
using System.Text;
using FutureConcepts.Tools;
using FutureConcepts.Media.TV.Scanner.Config;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace FutureConcepts.Media.TV.Scanner.TVGraphs
{
    public abstract class BaseLocalTuner : BaseGraph
    {
        public BaseLocalTuner(Config.Graph sourceConfig, Control hostControl)
            : base(sourceConfig, hostControl)
        {
            LoadKnownChannels();
        }

        public override void Render()
        {
            try
            {
                if ((CurrentChannelIndex < KnownChannels.Items.Count) && (CurrentChannelIndex >= 0))
                {
                    Channel = KnownChannels.Items[CurrentChannelIndex];
                }
                else if (KnownChannels.Items.Count > 0)
                {
                    Channel = KnownChannels.Items[0];
                }
                else
                {
                    Channel = new Channel(2);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// The lowest channel that could work, for the type of TV.
        /// </summary>
        public abstract Channel MinChannel { get; }

        /// <summary>
        /// The highest channel that could work, for the type of TV.
        /// </summary>
        public abstract Channel MaxChannel { get; }

        /// <summary>
        /// Attempts to Set the channel. Gets null if there was an error.
        /// </summary>
        public abstract Channel TryChannel { get; set; }

        /// <summary>
        /// The list of known-good channels
        /// </summary>
        public virtual ChannelCollection KnownChannels { get; set; }

        public override void ChannelUp()
        {
            if (CurrentChannelIndex == (KnownChannels.Items.Count - 1))
            {
                Channel = KnownChannels.Items[0];
            }
            else if (CurrentChannelIndex > -1)
            {
                Channel = KnownChannels.Items[CurrentChannelIndex + 1];
            }
            else
            {
                Channel = KnownChannels.FindClosest(Channel);
            }
        }

        public override void ChannelDown()
        {
            if (CurrentChannelIndex == 0)
            {
                Channel = KnownChannels.Items[KnownChannels.Items.Count - 1];
            }
            else if (CurrentChannelIndex > -1)
            {
                Channel = KnownChannels.Items[CurrentChannelIndex - 1];
            }
            else
            {
                int virtualIndex = KnownChannels.FindClosestIndex(Channel);
                Channel = KnownChannels.Items[virtualIndex - 1];
            }
        }

        protected virtual int CurrentChannelIndex
        {
            get
            {
                return KnownChannels.IndexOf(Channel);
            }
        }

        /// <summary>
        /// Returns the full file name used to persist the KnownChannels collection
        /// </summary>
        /// <returns>a string of the filename to save/load the KnownChannels from</returns>
        protected virtual string GetKnownChannelsStoreFilename()
        {
            return Config.AppUser.TVScannerSettingsRoot + this.GetType().Name + "_KnownChannels.xml";
        }

        /// <summary>
        /// Called when the KnownChannels collection should be loaded
        /// </summary>
        public virtual void LoadKnownChannels()
        {
            try
            {
                this.KnownChannels = ChannelCollection.LoadFromFile(GetKnownChannelsStoreFilename());
            }
            catch (Exception ex)
            {
                this.KnownChannels = new ChannelCollection();
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Called when the KnownChannels collection should be saved.
        /// </summary>
        public virtual void SaveKnownChannels()
        {
            try
            {
                if (CurrentChannelIndex != Config.AppUser.Current.ChannelIndex)
                {
                    Config.AppUser.Current.ChannelIndex = CurrentChannelIndex;
                }

                if (KnownChannels != null)
                {
                    KnownChannels.SaveToFile(GetKnownChannelsStoreFilename());
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.DumpToDebug(ex);
            }
        }

        /// <summary>
        /// Retreives the signal strength.
        /// </summary>
        /// <returns>Returns Less Than 0 if strength cannot be determined. Returns 0 if there is no signal strength. Returns Greater Than zero to indicate signal is present.</returns>
        public abstract int GetSignalStrength();
    }
}
