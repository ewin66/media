using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;
using FutureConcepts.Media.Contract;
using FutureConcepts.SystemTools.Devices.Microwave.MicrowaveReceiverController;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.IndigoServices
{
    /// <summary>
    /// This class manages the control of a microwave receiver, as directed by a client
    /// </summary>
    /// <author>kdixon 02/02/2011</author>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class MicrowaveControlService2 : IMicrowaveControl2, IDisposable
    {
        private ulong _keepAlives;

        private readonly object _userLock = new object();

        /// <summary>
        /// The currently opened source
        /// </summary>
        private ClientConnectRequest clientRequest;

        /// <summary>
        /// The microwave config for the currently selected source
        /// </summary>
        private MicrowaveControlInfo microwaveConfig;

        /// <summary>
        /// Persisted microwave frequency presets
        /// </summary>
        private UserPresetStore frequencyPresets;

        /// <summary>
        /// the microwave receiver we're dealing with
        /// </summary>
        private MicrowaveReceiver microwaveReceiver;

        /// <summary>
        /// This is used to search for valid frequencies
        /// </summary>
        private PeakScan scanner;

        /// <summary>
        /// Creates an instance of the required MicrowaveReceiver class
        /// </summary>
        /// <param name="config">the configuration info for the receiver</param>
        /// <returns>a microwave receiver class instance</returns>
        /// <exception cref="System.Exception">Thrown if config.ReceiverType is unrecognized</exception>
        private static MicrowaveReceiver CreateInstance(MicrowaveControlInfo config)
        {
            switch (config.ReceiverType)
            {
                case MicrowaveReceiverType.PMR_AR100:
                    return new PMR_AR100(config.TCPAddress,
                                         config.TCPPort,
                                         config.UsesTCP,
                                         config.ComPort,
                                         config.BaudRate);

                case MicrowaveReceiverType.Vislink_HDR1000:
                    return new Vislink_HDR1000(config.TCPAddress,
                                               config.TCPPort,
                                               config.UsesTCP,
                                               config.ComPort,
                                               config.BaudRate, 
                                               config.BlockDownConverterFrequency);

                case MicrowaveReceiverType.Test:
                    return new TestReciever(config.TCPAddress,
                                            config.TCPPort,
                                            config.UsesTCP,
                                            config.ComPort,
                                            config.BaudRate);

                default:
                    throw new Exception("Unsupported Microwave Receiver type");
            }
        }

        /// <summary>
        /// handle to our client's callback interface
        /// </summary>
        private IMicrowaveControl2Callback clientCallback;

        #region IMicrowaveControl Members

        /// <summary>
        /// True if the resource was acquired. Checked in Dispose to see if we need to release the resource
        /// </summary>
        private bool resourceAcquired = false;

        /// <summary>
        /// Opens the microwave control service
        /// </summary>
        /// <param name="sourceName">source name to open</param>
        public void Open(ClientConnectRequest request)
        {
            try
            {
                this.clientRequest = request;
                AppLogger.Message(request.UserName + " MicrowaveControlService.Open " + request.SourceName);

                if (!ResourceManager.Acquire<ResourceManager.MutexRule>(request.SourceName,
                                                                        typeof(MicrowaveControlService),
                                                                        request.UserName))
                {
                    string owner = ResourceManager.GetOwner(request.SourceName, typeof(MicrowaveControlService));
                    throw new SourceHasMaxClientsException("The microwave receiver is in use by " + ((owner == null) ? "<unknown>" : owner) + ".");
                }
                resourceAcquired = true;

                StreamSourceInfo sourceConfig = StreamSources.LoadFromFile().FindSource(request.SourceName);
                if (sourceConfig.MicrowaveControl == null)
                {
                    throw new SourceConfigException("Source does not have MicrowaveControl section defined!");
                }
                microwaveConfig = sourceConfig.MicrowaveControl;

                //get client callback
                clientCallback = OperationContext.Current.GetCallbackChannel<IMicrowaveControl2Callback>();

                //create microwave receiver instance
                microwaveReceiver = MicrowaveControlService2.CreateInstance(microwaveConfig);
                if (!microwaveReceiver.Connected)
                {
                    throw new SourceConfigException("Communication with the microwave receiver could not be established.");
                }
                microwaveReceiver.ReceiverTuningChange += new EventHandler<MicrowaveReceiver.ReceiverEventArgs>(microwaveReceiver_ReceiverTuningChange);
                microwaveReceiver.ReceiverLinkQualityChange += new EventHandler<MicrowaveReceiver.ReceiverEventArgs>(microwaveReceiver_ReceiverLinkQualityChange);
                microwaveReceiver.ReceiverConnectionChange += new EventHandler(microwaveReceiver_ReceiverConnectionChange);

                scanner = new PeakScan(microwaveReceiver);
                scanner.ScanCompleted += new EventHandler<ScanCompleteEvent>(scanner_ScanCompleted);

                //load cached presets
                LoadSavedPresets();

                microwaveReceiver.StartPollingLinkQuality();
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                throw;
            }
        }


        public void ForceUpdate()
        {
            AppLogger.Message("MicrowaveControlService.ForceUpdate");
            AppLogger.Message("MicrowaveReceiver.Connected = " + microwaveReceiver.Connected);

            //tell client what's up
            FireCapabilitiesChanged(microwaveReceiver.GetCapabilities());  
            FireTuningChanged(microwaveReceiver.GetTuning());
            FireLinkQualityChanged(microwaveReceiver.GetLinkQuality());
            FirePresetsChanged();
        }

        /// <summary>
        /// Tunes the microwave receiver to the specified frequency
        /// </summary>
        /// <param name="freq">the frequency, in MHz</param>
        public void SetTuning(MicrowaveTuning tuning)
        {
            try
            {
                AppLogger.Message("MCS2.SetTuning");
                microwaveReceiver.SetTuning(tuning);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
                this.FireTuningChanged(null);
            }
        }

        /// <summary>
        /// Used to indicate that frequency sweep is in progress
        /// </summary>
        private bool scanInProgress = false;

        private UInt64 scanStart, scanEnd;

        /// <summary>
        /// Does a sweep of the frequencies (inclusive) in a given range
        /// </summary>
        /// <param name="start">start frequency in Hz</param>
        /// <param name="end">end frequency in Hz</param>
        /// <param name="threshold">percentage of minimum signal strength</param>
        public void StartSweep(MicrowaveTuning settings, UInt64 start, UInt64 end, int threshold)
        {
            AppLogger.Message("MCS2.StartSweep");
            scanStart = start;
            scanEnd = end;

            scanner.PeakScanAsync(settings, start, end, threshold);

            scanInProgress = true;
        }

        public void CancelSweep()
        {
            AppLogger.Message("MCS2.CancelSweep");
            scanInProgress = false;
            scanner.CancelScan();
        }

        public void KeepAlive()
        {
            AppLogger.Message("MCS2.KeepAlive");
            _keepAlives++;
        }

        #endregion

        #region Event Handling

        void scanner_ScanCompleted(object sender, ScanCompleteEvent e)
        {
            AppLogger.Message("MicrowaveControlService scanner_ScanCompleted");
            ChannelScanCompleteEventArgs argsToClient = new ChannelScanCompleteEventArgs();
            argsToClient.Cancelled = !scanInProgress;

            argsToClient.ChannelsFound = e.PeakDictionary.Count;

            scanInProgress = false;

            FireChannelScanCompleted(argsToClient);

            lock (_userLock)
            {
                AppLogger.Message("    removing old scan presets");
                List<Guid> autoGeneratedIDs = new List<Guid>();
                foreach (UserPresetItem i in frequencyPresets)
                {
                    if (i is MicrowaveTuningPreset)
                    {
                        if (((MicrowaveTuningPreset)i).AutoGenerated)
                        {
                            autoGeneratedIDs.Add(i.ID);
                        }
                    }
                }
                foreach (Guid id in autoGeneratedIDs)
                {
                    frequencyPresets.Remove(id);
                }

                AppLogger.Message("    adding new presets");
                foreach (MicrowaveTuning freq in e.PeakDictionary.Keys)
                {
                    if (!HasPresetWithFrequency(freq.Frequency))
                    {
                        MicrowaveTuningPreset p = new MicrowaveTuningPreset(freq);
                        p.AutoGenerated = true;
                        frequencyPresets.Add(p);
                    }
                }
            }

            FirePresetsChanged();
        }

        /// <summary>
        /// Returns true if a preset pre-exists with a given frequency
        /// </summary>
        /// <param name="freq">frequency to check for</param>
        /// <returns>true if found, false if not</returns>
        private bool HasPresetWithFrequency(UInt64 freq)
        {
            foreach (UserPresetItem i in frequencyPresets)
            {
                MicrowaveTuningPreset p = i as MicrowaveTuningPreset;
                if (p != null)
                {
                    if (p.Tuning.Frequency == freq)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void microwaveReceiver_ReceiverConnectionChange(object sender, EventArgs e)
        {
            //TODO determine any further action
            AppLogger.Message("MicrowaveControlService : recevier connected: " + microwaveReceiver.Connected);
        }

        private void microwaveReceiver_ReceiverTuningChange(object sender, MicrowaveReceiver.ReceiverEventArgs e)
        {
            AppLogger.Message("MicrowaveControlService: microwaveReceiver_ReceiverTuningChange");
            if (scanInProgress)
            {
                FireChannelScanProgressUpdate((int)((double)(e.Tuning.Frequency - scanStart) / (scanEnd - scanStart + 1) * 100));
            }

            //TODO determine if this is too much traffic
            FireTuningChanged(e.Tuning);
            FireLinkQualityChanged(microwaveReceiver.GetLinkQuality());
        }

        void microwaveReceiver_ReceiverLinkQualityChange(object sender, MicrowaveReceiver.ReceiverEventArgs e)
        {
            FireLinkQualityChanged(e.LinkQuality);
        }


        #endregion 

        #region IMicrowaveControl2Callback members

        private void FireCapabilitiesChanged(MicrowaveCapabilities capabilities)
        {
            try
            {
                AppLogger.Message("  MicrowaveControlService2.FireCapabilitiesChanged");
                clientCallback.CapabilitiesChanged(capabilities);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        private void FireTuningChanged(MicrowaveTuning tuning)
        {
            try
            {
                AppLogger.Message("  MicrowaveControlService2.FireTuningChanged");
                clientCallback.TuningChanged(tuning);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        private void FireLinkQualityChanged(MicrowaveLinkQuality linkQuality)
        {
            try
            {
                AppLogger.Message("  MicrowaveControlService2.FireLinkQualityChanged");
                clientCallback.LinkQualityChanged(linkQuality);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        private void FirePresetsChanged()
        {
            try
            {
                clientCallback.SavedPresetsChanged(this.frequencyPresets);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        private void FireChannelScanProgressUpdate(int progress)
        {
            try
            {
                clientCallback.ChannelScanProgressUpdate(progress);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        private void FireChannelScanCompleted(ChannelScanCompleteEventArgs args)
        {
            try
            {
                clientCallback.ChannelScanComplete(args);
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        #endregion

        #region IPresetProvider Members

        #region Preset Persistance

        /// <summary>
        /// returns the filename where the presets are stored for this source
        /// </summary>
        private string FrequencyPresetsFilename
        {
            get
            {
                return PathMapper.AppData("MicrowavePresets_" + clientRequest.SourceName + ".xml");
            }
        }

        /// <summary>
        /// persists the microwave presets
        /// </summary>
        private void PersistPresets()
        {
            XmlTextWriter file = null;
            try
            {
                string dir = Path.GetDirectoryName(FrequencyPresetsFilename);
                Directory.CreateDirectory(dir);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPresetStore));
                file = new XmlTextWriter(FrequencyPresetsFilename, null);
                xmlSerializer.Serialize(file, frequencyPresets);
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
            finally
            {
                if(file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// fetches the persisted microwave presests
        /// </summary>
        private void LoadSavedPresets()
        {
            XmlTextReader file = null;
            try
            {
                if (!File.Exists(FrequencyPresetsFilename))
                {
                    throw new FileNotFoundException("Presets file \"" + FrequencyPresetsFilename + "\" does not exist!");
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPresetStore));
                file = new XmlTextReader(FrequencyPresetsFilename);
                frequencyPresets = (UserPresetStore)xmlSerializer.Deserialize(file);
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                frequencyPresets = new UserPresetStore();
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        #endregion

        public UserPresetItem SavePreset()
        {
            lock (_userLock)
            {
                AppLogger.Message("MicrowaveControlService2.SavePreset");
                try
                {
                    MicrowaveTuningPreset item = new MicrowaveTuningPreset(microwaveReceiver.GetTuning());
                    frequencyPresets.Add(item);
                    return item;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    return null;
                }
            }
        }

        public void RestorePreset(Guid id)
        {
            MicrowaveTuningPreset item = null;
            lock (_userLock)
            {
                AppLogger.Message("MicrowaveControlService2.RestorePreset " + id);

                try
                {
                    item = frequencyPresets[id] as MicrowaveTuningPreset;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                }
            }

            if (item != null)
            {
                AppLogger.Message("Preset found, restoring...");
                this.SetTuning(item.Tuning);
            }
        }

        public bool UpdatePreset(UserPresetItem updatedItem)
        {
            lock (_userLock)
            {
                AppLogger.Message(String.Format("MicrowaveControlService2.UpdatePreset {0} = {1}", updatedItem.ID, updatedItem));
                try
                {
                    if(!frequencyPresets.Remove(updatedItem.ID))
                    {
                        AppLogger.Message("   Warning! No item with ID " + updatedItem.ID + " pre-existed!");
                    }

                    frequencyPresets.Add(updatedItem);
                    return true;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    return false;
                }
            }
        }

        public bool DeletePreset(Guid id)
        {
            lock (_userLock)
            {
                try
                {
                    AppLogger.Message("MicrowaveControlService2.DeletePreset " + id);
                    UserPresetItem item = frequencyPresets[id];
                    return frequencyPresets.Remove(item);
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    return false;
                }
            }
        }

        public void DeleteAllPresets()
        {
            lock (_userLock)
            {
                try
                {
                    AppLogger.Message("MicrowaveControlService2.DeleteAllPresets");
                    frequencyPresets.Clear();
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            AppLogger.Message("MicrowaveControlService2.Dispose");

            if (scanner != null)
            {
                scanner.ScanCompleted -= new EventHandler<ScanCompleteEvent>(scanner_ScanCompleted);
                scanner.CancelScan();
                scanner = null;
            }

            if (microwaveReceiver != null)
            {
                microwaveReceiver.StopPollingLinkQuality();
                microwaveReceiver.Dispose();
                microwaveReceiver = null;
            }

            if (resourceAcquired)
            {
                try
                {
                    ResourceManager.Release(clientRequest.SourceName, typeof(MicrowaveControlService), clientRequest.UserName);
                }
                catch (Exception ex)
                {
                    AppLogger.Dump(ex);
                }
            }

            if (frequencyPresets != null)
            {
                PersistPresets();
            }
        }

        #endregion
    }
}
