using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.ServiceModel;
using System.Xml.Serialization;

using Antares.Video.Config;

namespace Antares.Video
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class RecorderControlService : IRecorderControl
    {
        public void Record(string sourceName, string channel, string title, string mediaName)
        {
            Debug.WriteLine("RecorderControlService.Record " + sourceName + ", " + channel + ", " + title + ", " + mediaName);
            SourceConfig source = SourcesConfig.FindSource(sourceName);
            if (source != null)
            {
                SourceRecorder sourceRecorder = new SourceRecorder(source, channel, title, mediaName);
                if (sourceRecorder != null)
                {
                    sourceRecorder.Record();
                }
            }
        }

        public void Stop(string sourceName)
        {
            Debug.WriteLine("RecorderControlService.Stop " + sourceName);
            try
            {
                SourceConfig source = SourcesConfig.FindSource(sourceName);
                if (source != null)
                {
                    SourceRecorder recorder = SourceRecorder.FindRecorder(source);
                    if (recorder != null)
                    {
                        recorder.Stop();
                        recorder.Dispose();
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("RecorderControlService.Stop Exception:");
                Debug.WriteLine(exc.Message);
            }
        }

        public List<RecordingSession> QueryRecordingSessions(string sourceName)
        {
            List<RecordingSession> recordingSessions = new List<RecordingSession>();
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["RecordingMetaDataPath"] + sourceName);
            foreach (string filename in files)
            {
                RecordingSession recordingSession;
                XmlSerializer sessionSerializer = new XmlSerializer(typeof(RecordingSession));
                FileStream fileStream = new FileStream(filename, FileMode.Open);
                recordingSession = (RecordingSession)sessionSerializer.Deserialize(fileStream);
                fileStream.Close();
                fileStream = null;
                recordingSessions.Add(recordingSession);
            }
            Debug.WriteLine("QueryRecordingSessions return " + recordingSessions.Count + " sessions");
            return recordingSessions;
        }

        public void Delete(string sourceName, Guid id)
        {
            throw new Exception("RecorderControlService.Delete not implemeneted.");
        }

        public ICollection GetMediaNames()
        {
            return SourceRecorder.GetMediaNames();
        }
    }
}
