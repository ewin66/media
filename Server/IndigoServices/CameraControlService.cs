using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Xml.Serialization;

using FutureConcepts.Media;
using FutureConcepts.Media.CameraControls;
using FutureConcepts.Tools;
using FutureConcepts.Media.Contract;

namespace FutureConcepts.Media.Server.IndigoServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CameraControlService : ICameraControl, ICameraControlProperties, IDisposable
    {
        private ClientConnectRequest clientRequest;

        private int _keepAlives = 0;

        private object _userLock = new Object();

        private ICameraControlCallback _callback;

        public ICameraControlCallback Callback
        {
            get
            {
                _callback = OperationContext.Current.GetCallbackChannel<ICameraControlCallback>();
                Debug.Assert(_callback != null);
                return _callback;
            }
            set
            {
                _callback = value;
            }
        }

        private ICameraControlPlugin _plugin;

        public ICameraControlPlugin Plugin
        {
            get
            {
                Debug.Assert(_plugin != null);
                return _plugin;
            }
            set
            {
                _plugin = value;
            }
        }

        private string ResourceID { get; set; }

        #region ICameraControlProperties

        private bool _infraredEnabled;

        public bool InfraredEnabled
        {
            get
            {
                return _infraredEnabled;
            }
            set
            {
                _infraredEnabled = value;
                Callback.InfraredChanged(value);
                AppLogger.Message(String.Format("Sent InfraredChanged={0}", value));
            }
        }

        private bool _stabilizerEnabled;

        public bool StabilizerEnabled
        {
            get
            {
                return _stabilizerEnabled;
            }
            set
            {
                _stabilizerEnabled = value;
                Callback.StabilizerChanged(value);
                AppLogger.Message(String.Format("Sent StabilizerChanged={0}", value));
            }
        }

        private bool _wiperEnabled;

        public bool WiperEnabled
        {
            get
            {
                return _wiperEnabled;
            }
            set
            {
                _wiperEnabled = value;
                Callback.WiperChanged(value);
                AppLogger.Message(String.Format("Sent WiperChanged={0}", value));
            }
        }

        private bool _emitterEnabled;

        public bool EmitterEnabled
        {
            get
            {
                return _emitterEnabled;
            }
            set
            {
                _emitterEnabled = value;
                Callback.EmitterChanged(value);
                AppLogger.Message(String.Format("Sent EmitterChanged={0}", value));
            }
        }

        private bool _invertedEnabled;

        public bool InvertedEnabled
        {
            get
            {
                return _invertedEnabled;
            }
            set
            {
                _invertedEnabled = value;
                Callback.OrientationChanged(value);
                AppLogger.Message(String.Format("Sent OrientationChanged={0}", value));
            }
        }

        private double _currentPanAngle;

        public double CurrentPanAngle
        {
            get
            {
                return _currentPanAngle;
            }
            set
            {
                _currentPanAngle = value;
                Callback.PanAngleChanged(_currentPanAngle);
                AppLogger.Message(String.Format("Sent PanAngleChanged={0}", _currentPanAngle));
            }
        }

        private double _currentTiltAngle;

        public double CurrentTiltAngle
        {
            get
            {
                return _currentTiltAngle;
            }
            set
            {
                _currentTiltAngle = value;
                Callback.TiltAngleChanged(_currentTiltAngle);
                AppLogger.Message(String.Format("Sent TiltAngleChanged={0}", _currentTiltAngle));
            }
        }

        private double _currentZoomPosition;

        public double CurrentZoomPosition
        {
            get
            {
                return _currentZoomPosition;
            }
            set
            {
                _currentZoomPosition = value;
                Callback.ZoomPositionChanged(_currentZoomPosition);
                AppLogger.Message(String.Format("Sent ZoomPositionChanged={0}", value));
            }
        }

        private string _message;

        public string StatusMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                Callback.StatusMessageChanged(_message);
                AppLogger.Message(String.Format("Sent StatusMessage={0}", value));
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (resourceAcquired)
            {
                try
                {
                    ResourceManager.Release(this.ResourceID, typeof(CameraControlService), clientRequest.UserName);
                }
                catch (Exception ex)
                {
                    AppLogger.Dump(ex);
                }
            }

            if (PresetItems != null)
            {
                PersistSavedPositions();
            }

            if (_plugin != null)
            {
                try
                {
                    _plugin.Dispose();
                    _plugin = null;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                }
            }
        }

        #endregion

        #region ICameraControl

        /// <summary>
        /// holds the loaded Camera Config, as loaded in Open
        /// </summary>
        private CameraControlInfo CameraControl
        {
            get;
            set;
        }

        /// <summary>
        /// True if the resource was acquired. Checked in Dispose to see if we need to release the resource
        /// </summary>
        private bool resourceAcquired = false;

        public void Open(ClientConnectRequest request)
        {
            try
            {
                clientRequest = request;
                AppLogger.Message(request.UserName + " CameraControlService.Open " + request.SourceName);

                StreamSourceInfo sourceConfig = StreamSources.LoadFromFile().FindSource(request.SourceName);
                if (sourceConfig.CameraControl == null)
                {
                    throw new SourceConfigException("Source does not have CameraControl defined!");
                }

                //lock against the camera control address -- this allows multiple sources to share the same camera.
                //    if the particular camera control does not require an address, then lock against the source name
                this.ResourceID = string.IsNullOrEmpty(sourceConfig.CameraControl.Address) ?
                                        request.SourceName :
                                        sourceConfig.CameraControl.Address;

                if (!ResourceManager.Acquire<ResourceManager.MutexRule>(this.ResourceID,
                                                                        typeof(CameraControlService),
                                                                        request.UserName))
                {
                    string owner = ResourceManager.GetOwner(this.ResourceID, typeof(CameraControlService));
                    throw new SourceHasMaxClientsException("The camera control is in use by " +
                                                           (string.IsNullOrEmpty(owner) ? "<unknown>" : owner) + ".");
                }
                resourceAcquired = true;

                CameraControl = sourceConfig.CameraControl;
                Plugin = CameraControls.PluginFactory.Create(CameraControl, this);
                LoadSavedPositions();
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                //throw new Exception("Unable to open camera control for " + sourceName, exc);
                throw;
            }
        }

        public void Initialize()
        {
            Plugin.Initialize();
            PanTiltZoomPositionInquire();
            Callback.SavedPositionsChanged(PresetItems);
            AppLogger.Message("CameraControlService.Initialize sent saved positions");
        }

        public void SetInfrared(bool enabled)
        {
            lock (_userLock)
            {
                bool oldValue = InfraredEnabled;
                try
                {
                    Plugin.SetInfrared(enabled);
                    InfraredEnabled = enabled;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    InfraredEnabled = oldValue;
                }
            }
        }

        public void SetStabilizer(bool enabled)
        {
            lock (_userLock)
            {
                bool oldValue = StabilizerEnabled;
                try
                {
                    Plugin.SetStabilizer(enabled);
                    StabilizerEnabled = enabled;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    StabilizerEnabled = oldValue;
                }
            }
        }

        public void SetWiper(bool enabled)
        {
            lock (_userLock)
            {
                bool oldValue = WiperEnabled;
                try
                {
                    Plugin.SetWiper(enabled);
                    WiperEnabled = enabled;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    WiperEnabled = oldValue;
                }
            }
        }

        public void SetEmitter(bool enabled)
        {
            lock (_userLock)
            {
                bool oldValue = EmitterEnabled;
                try
                {
                    Plugin.SetEmitter(enabled);
                    EmitterEnabled = enabled;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    EmitterEnabled = oldValue;
                }
            }
        }

        public void SetOrientation(bool inverted)
        {
            lock (_userLock)
            {
                bool oldValue = InvertedEnabled;
                try
                {
                    Plugin.SetOrientation(inverted);
                    InvertedEnabled = inverted;
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    InvertedEnabled = oldValue;
                }
            }
        }

        public void PanTiltAbsolute(double panAngle, double tiltAngle)
        {
            lock (_userLock)
            {
                double oldPanValue = CurrentPanAngle;
                double oldTiltValue = CurrentTiltAngle;
                try
                {
                    if (((panAngle >= 0) && (panAngle < 360)) &&
                        ((tiltAngle >= CameraControl.Capabilities.TiltMinAngle) &&
                         (tiltAngle <= CameraControl.Capabilities.TiltMaxAngle)))
                    {
                        Plugin.PanTiltAbsolute(panAngle, tiltAngle);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("panAngle or tiltAngle", "Expected range is [0,360], [" +
                                                              CameraControl.Capabilities.TiltMinAngle.ToString() + "," +
                                                              CameraControl.Capabilities.TiltMaxAngle.ToString() + "].");
                    }
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    CurrentPanAngle = oldPanValue;
                    CurrentTiltAngle = oldTiltValue;
                }
            }
        }

        public void ZoomAbsolute(double zoomPosition)
        {
            lock (_userLock)
            {
                double oldValue = CurrentZoomPosition;
                try
                {
                    if ((zoomPosition >= CameraControl.Capabilities.ZoomMinLevel) &&
                        (zoomPosition <= CameraControl.Capabilities.ZoomMaxLevel))
                    {
                        Plugin.ZoomAbsolute(zoomPosition);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("zoomPosition", "Expected range is [" +
                                                              CameraControl.Capabilities.ZoomMinLevel.ToString() + "," +
                                                              CameraControl.Capabilities.ZoomMaxLevel.ToString() + "].");
                    }
                }
                catch (Exception exc)
                {
                    AppLogger.Dump(exc);
                    CurrentZoomPosition = oldValue;
                }
            }
        }

        public void PanTiltZoomPositionInquire()
        {
            lock (_userLock)
            {
                AppLogger.Message("CameraControlService.PanTiltZoomPositionInquire");
                Plugin.PanTiltZoomPositionInquire();
                AppLogger.Message("Sending PanAngle " + CurrentPanAngle.ToString());
                AppLogger.Message("Sending TiltAngle " + CurrentTiltAngle.ToString());
                AppLogger.Message("Sending ZoomPosition " + CurrentZoomPosition.ToString());
                Callback.PanAngleChanged(CurrentPanAngle);
                Callback.TiltAngleChanged(CurrentTiltAngle);
                Callback.ZoomPositionChanged(CurrentZoomPosition);
            }
        }

        public void KeepAlive()
        {
            _keepAlives++;
        }

        #endregion

        #region Camera Saved Position Presets

        /// <summary>
        /// This is the backing store for plugins that don't do their own presets
        /// </summary>
        private UserPresetStore _savedPositions;

        /// <summary>
        /// Fetches the correct UserPresetStore to use, either our default, or the plugin's
        /// </summary>
        private UserPresetStore PresetItems
        {
            get
            {
                if (_plugin is IPresetProviderItems)
                {
                    return ((IPresetProviderItems)_plugin).PresetItems;
                }
                else
                {
                    return _savedPositions;
                }
            }
            set
            {
                if (_plugin is IPresetProviderItems)
                {
                    ((IPresetProviderItems)_plugin).PresetItems = value;
                }
                else
                {
                    _savedPositions = value;
                }
            }
        }

        /// <summary>
        /// If the plugin is an IPresetProvider 
        /// </summary>
        /// <returns></returns>
        public UserPresetItem SavePreset()
        {
            lock (_userLock)
            {
                AppLogger.Message("CameraControlService.SavePreset");
                try
                {
                    UserPresetItem item = null;
                    if (_plugin is IPresetProvider)
                    {
                        item = ((IPresetProvider)_plugin).SavePreset();
                    }
                    else
                    {
                        item = new CameraPositionPreset(CurrentPanAngle, CurrentTiltAngle, CurrentZoomPosition);
                        PresetItems.Add(item);
                    }
                    return item;
                }
                catch (IndexOutOfRangeException ex)
                {
                    AppLogger.Dump(ex);
                    this.StatusMessage = ex.Message;
                    return null;
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
            try
            {
                AppLogger.Message("CameraControlService.RestorePreset " + id);

                if (_plugin is IPresetProvider)
                {
                    lock (_userLock)
                    {
                        ((IPresetProvider)_plugin).RestorePreset(id);
                    }
                }
                else
                {
                    CameraPositionPreset item = null;
                    lock (_userLock)
                    {
                        try
                        {
                            item = PresetItems[id] as CameraPositionPreset;
                        }
                        catch (Exception exc)
                        {
                            AppLogger.Dump(exc);
                        }
                    }
                    if (item != null)
                    {
                        PanTiltAbsolute(item.PanAngle, item.TiltAngle);
                        ZoomAbsolute(item.ZoomPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Dump(ex);
            }
        }

        public bool UpdatePreset(UserPresetItem newItem)
        {
            try
            {
                AppLogger.Message(String.Format("CameraControlService.UpdatePreset {0} = {1}", newItem.ID, newItem));
                lock (_userLock)
                {
                    if (_plugin is IPresetProvider)
                    {
                        return ((IPresetProvider)_plugin).UpdatePreset(newItem);
                    }
                    else
                    {
                        UserPresetItem oldItem = PresetItems[newItem.ID];
                        if (oldItem != null)
                        {
                            PresetItems.Remove(oldItem);
                        }
                        else
                        {
                            AppLogger.Message("   Warning! No item with ID " + newItem.ID + " pre-existed!");
                        }

                        PresetItems.Add(newItem);
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                return false;
            }
        }

        public bool DeletePreset(Guid id)
        {
            try
            {
                AppLogger.Message("CameraControlService.DeletePreset " + id);

                lock (_userLock)
                {
                    if (_plugin is IPresetProvider)
                    {
                        return ((IPresetProvider)_plugin).DeletePreset(id);
                    }
                    else
                    {
                        UserPresetItem item = PresetItems[id];
                        return PresetItems.Remove(item);
                    }
                }
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                return false;
            }
        }

        public void DeleteAllPresets()
        {
            try
            {
                AppLogger.Message("CameraControlService.DeleteAllPresets");
                lock (_userLock)
                {
                    if (_plugin is IPresetProvider)
                    {
                        ((IPresetProvider)_plugin).DeleteAllPresets();
                    }
                    else
                    {
                        PresetItems.Clear();
                    }
                }
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
        }

        private string SavedPositionsFileName
        {
            get
            {
                return PathMapper.AppData("SavedCameraPositions_" + clientRequest.SourceName + ".xml");
            }
        }

        private void PersistSavedPositions()
        {
            XmlTextWriter file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPresetStore));
                file = new XmlTextWriter(SavedPositionsFileName, null);
                xmlSerializer.Serialize(file, PresetItems);
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        public void LoadSavedPositions()
        {
            XmlTextReader reader = null;
            try
            {
                if (!File.Exists(SavedPositionsFileName))
                {
                    throw new FileNotFoundException("The file \"" + SavedPositionsFileName + "\" does not exist.");
                }
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserPresetStore));
                reader = new XmlTextReader(SavedPositionsFileName);
                PresetItems = (UserPresetStore)xmlSerializer.Deserialize(reader);
            }
            catch (Exception exc)
            {
                AppLogger.Dump(exc);
                PresetItems = new UserPresetStore();
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        #endregion
    }
}
