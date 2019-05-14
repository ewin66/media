using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

using FutureConcepts.Media.SourceDiscoveryCommon;
using FutureConcepts.Settings;
using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.MasterSourceDiscovery
{
    public class MasterSourceDiscoveryPlugin : SourceDiscoveryPlugin
    {
        private Dictionary<String, SourceDiscoveryGroup> _groups;
        private SourceDiscoveryDefinition _definition;
        private QueryHandler _queryHandler;
        private Boolean _abort = false;
        private Thread _thread;

        public MasterSourceDiscoveryPlugin(SourceDiscoveryDefinition definition)
        {
            _definition = definition;
            _groups = new Dictionary<String, SourceDiscoveryGroup>();
            _queryHandler = new QueryHandler();
        }

        public override void Start()
        {
            var thread = new Thread(new ThreadStart(DoQuery));
            thread.Start();
        }

        private void CheckForNewOrChangedGroups(List<SourceDiscoveryGroup> newGroups)
        {
            foreach (SourceDiscoveryGroup newGroup in newGroups)
            {
                String name = newGroup.Name;
                if (name != null)
                {
                    if (_groups.ContainsKey(name))
                    {
                        SourceDiscoveryGroup existingGroup = _groups[name];
                        if (newGroup.Version > existingGroup.Version)
                        {
                            _groups.Remove(name);
                            _groups.Add(name, newGroup);
                            FireGroupChanged(newGroup);
                        }
                    }
                    else
                    {
                        _groups.Add(name, newGroup);
                        FireGroupOnline(newGroup);
                    }
                }
            }
        }

        private void CheckForDeletedGroups(List<SourceDiscoveryGroup> groups)
        {
            Dictionary<String, SourceDiscoveryGroup> dict = groups.ToDictionary(l=>l.Name);
            List<String> deletedGroups = new List<String>();
            foreach (SourceDiscoveryGroup checkGroup in _groups.Values)
            {
                if (dict.ContainsKey(checkGroup.Name) == false)
                {
                    FireGroupOffline(checkGroup);
                    deletedGroups.Add(checkGroup.Name);
                }
            }
            foreach (String deletedGroup in deletedGroups)
            {
                _groups.Remove(deletedGroup);
            }
        }

        public override void Stop()
        {
            Debug.WriteLine("MasterSourceDiscoveryPlugin Stop Begin");
            foreach (SourceDiscoveryGroup group in _groups.Values)
            {
                FireGroupOffline(group);
            }
            _groups.Clear();
            _abort = true;
            Debug.WriteLine("MasterSourceDiscoveryPlugin Stop Complete");
        }

        private void DoQuery()
        {
            while (_abort == false)
            {
                try
                {
                    List<SourceDiscoveryGroup> groups = QueryHandler.Query(_definition.URL, "", "");
                    CheckForNewOrChangedGroups(groups);
                    CheckForDeletedGroups(groups);
                }
                catch (Exception e)
                {
                    ErrorLogger.DumpToDebug(e);
                }
                Thread.Sleep(_definition.PollInterval);
            }
        }
    }
}
