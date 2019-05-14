using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;

using FutureConcepts.Tools;

namespace FutureConcepts.Media.Client.CameraControlClients.Transport
{
    public class DebugTest1 : ITransport
    {
        public DebugTest1(String url)
        {
            Logit("Constructor", url);
        }

        private void Logit(String method, String msg)
        {
            Debug.WriteLine(String.Format("DebugTest1 {0}: {1}", method, msg));
        }

        public void Open()
        {
            Logit("Open", "Open");
        }

        public void Close()
        {
            Logit("Close", "Close");
        }

        public void Dispose()
        {
            Close();
            Logit("Dispose", "Dispose");
        }

        public long BytesToRead
        {
            get
            {
                return 0;
            }
        }

        public int ReadByte()
        {
            return -1;
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            try
            {
                Logit("Write", BitConverter.ToString(bytes));
            }
            catch (Exception e)
            {
                ErrorLogger.DumpToDebug(e);
                Close();
                throw;
            }
        }
    }
}
