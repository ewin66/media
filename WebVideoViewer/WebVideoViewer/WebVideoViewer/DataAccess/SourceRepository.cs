using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebVideoViewer.Models;

namespace WebVideoViewer.DataAccess
{
    public class SourceRepository : IDisposable
    {
        private video_streamingEntities _context;

        public SourceRepository(video_streamingEntities context)
        {
            _context = context;
        }

        private int GetClientID(String userHostAddress)
        {
 //           if (userHostAddress == "::1" || userHostAddress == "127.0.0.1")
            {
                userHostAddress = "67.168.69.238"; // localhost -- probably debugging -- FIXME
            }
            String[] parts = userHostAddress.Split('.');
            String subnet = parts[0] + '.' + parts[1] + '.' + parts[2];
            int last_octet = Int32.Parse(parts[3]);

            int result = (from c in _context.clients
                          where c.subnet == subnet && last_octet >= c.min_ip && last_octet <= c.max_ip
                          select c.id).FirstOrDefault();
            return result;
        }

        private long GetMinimumTimestamp()
        {
            long result = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - 500;
            return result;
        }

        private Boolean CanClientAccessGroup(int clientId, int groupId)
        {
            if (clientId == groupId)
            {
                return true;
            }
            var count = (from p in _context.permissions
                         where clientId == p.client_id && p.allowed_id == groupId
                         select p).Count();

            return count > 0;
        }

        public source GetSourceByID(String id)
        {
            if (id != null)
            {
                Int32 idAsInt = Int32.Parse(id);
                var result = (from s in _context.sources
                              where s.id == idAsInt
                              select s).FirstOrDefault();
                return result;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<source> GetSourcesForRemoteClient(String userHostAddress, String mimeType)
        {
            List<source> result = new List<source>();

            int client_id = GetClientID(userHostAddress);

            long minimumTimestamp = GetMinimumTimestamp();

            var online_clients_set = (from oc in _context.online_clients
                                      where oc.last_reported >= minimumTimestamp && oc.client_id != client_id
                                      select oc).ToList();

            foreach (online_clients item in online_clients_set)
            {
                if (CanClientAccessGroup(client_id, item.client_id))
                {
                    var groupSources = from s in _context.sources
                                       where s.client_id == item.client_id && s.mime_type == mimeType
                                       select s;
                    result.AddRange(groupSources);
                }
            }
            return result;
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}