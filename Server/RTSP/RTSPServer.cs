using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using FutureConcepts.Tools;

namespace FutureConcepts.Media.Server.RTSP
{
    [DataContract]
    [Serializable]
    public class RTSPServer : IExtensibleDataObject
    {
        [NonSerialized]
        private ExtensionDataObject _extensionDataObject;

        public virtual ExtensionDataObject ExtensionData
        {
            get
            {
                return _extensionDataObject;
            }
            set
            {
                _extensionDataObject = value;
            }
        }

        private List<String> _contentFolders;

        [DataMember]
        [XmlElement]
        public List<String> ContentFolders
        {
            get
            {
                return _contentFolders;
            }
            set
            {
                _contentFolders = value;
            }
        }

        private String _targetAddress = "127.0.0.1";

        [DataMember]
        public String TargetAddress
        {
            get
            {
                return _targetAddress;
            }
            set
            {
                _targetAddress = value;
            }
        }

        private int _port = 554;

        [DataMember]
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        private double _liveLatencySeconds = 0.5;

        [DataMember]
        public double LiveLatencySeconds
        {
            get
            {
                return _liveLatencySeconds;
            }
            set
            {
                _liveLatencySeconds = value;
            }
        }

        /// <summary>
        /// Fetches the file name where this collection should be persisted.
        /// </summary>
        public static string PersistFileName
        {
            get
            {
                return Server.PathMapper.Config("RTSP.xml");
            }
        }
        /// <summary>
        /// Loads the StreamSources for the application, prefering the memory cached copy if present.
        /// </summary>
        /// <returns>the StreamSources for the application</returns>
        public static RTSPServer LoadFromFile()
        {
            return LoadFromFile(PersistFileName);
        }

        /// <summary>
        /// Loads StreamSources from a specified file.
        /// </summary>
        /// <param name="fileName">The filename to load. If you are trying to load multiple distinct files, set bypassCache to true.</param>
        /// Set to true to always read from the disk and return a distinct copy
        /// Set to false to use the cached copy if available.
        /// </param>
        /// <returns>the RTSPServer from the indicated file</returns>
        public static RTSPServer LoadFromFile(string fileName)
        {
            RTSPServer result = null;
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(RTSPServer));
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                result = (RTSPServer)xmlSerializer.Deserialize(file);
            }
            catch (Exception e)
            {
                AppLogger.Message(String.Format("RTSPServer was not loaded -- the configuration file {0} could not be loaded", PersistFileName));
                AppLogger.Dump(e);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }
            }
            return result;
        }

//        private Leadtools.Multimedia.RTSPServer _ltRtspServer;

        public void Start()
        {
            try
            {
           //     _ltRtspServer = new Leadtools.Multimedia.RTSPServer();
           //     _ltRtspServer.SetSecurity(-1, null);
                AppLogger.Message("RTSPServer security set to None");
                for (int i = 0; i < ContentFolders.Count; i++)
                {
              //      _ltRtspServer.SetSourceFolder(i, ContentFolders[0]);
              //      _ltRtspServer.SetSecurity(i, null);
                    AppLogger.Message(String.Format("RTSPServer added Content folder {0}", ContentFolders[i]));
                }
                int idx = ContentFolders.Count;
           //     _ltRtspServer.SetSourceFolder(idx, Graphs.BaseDVRSinkGraph.Settings.RootFolder);
                LiveLatencySeconds = 0;
            //    _ltRtspServer.SetLiveLatency(idx, LiveLatencySeconds);
             //   _ltRtspServer.SetSecurity(idx, null);
                AppLogger.Message(String.Format("RTSPServer added DVR root folder {0}", Graphs.BaseDVRSinkGraph.Settings.RootFolder));
             //   _ltRtspServer.TargetAddress = TargetAddress;
             //   _ltRtspServer.StartServer(Port);
                AppLogger.Message("RTSPServer started");
            }
            catch (Exception e)
            {
                FutureConcepts.Tools.AppLogger.Dump(e);
            }
        }

        public void Stop()
        {
      //      if (_ltRtspServer != null)
            {
          //      _ltRtspServer.StopServer(_port);
         //       _ltRtspServer = null;
                AppLogger.Message("RTSPServer stopped");
            }
        }
    }
}
