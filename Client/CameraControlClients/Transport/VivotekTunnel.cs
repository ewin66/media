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
    public class VivotekTunnel : ITransport
    {
        private Uri _uri;
        private HttpWebRequest _downChannel;
        private HttpWebRequest _upChannel;
        private String _sessionCookie;
        private Stream _downStream;
        private Stream _upStream;

        private ManualResetEvent _gotRequestStream = new ManualResetEvent(false);

        public VivotekTunnel(String url)
        {
            Uri inputUri = new Uri(url);
//        http://10.0.201.135/cgi-bin/operator/uartchannel.cgi?channel=0
            _uri = new Uri(String.Format("http://{0}/cgi-bin/operator/uartchannel.cgi?channel=0", inputUri.Host));
            Debug.WriteLine("VivotekTunnel URL: " + _uri.ToString());
            _sessionCookie = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        #region ITransport

        public void Open()
        {
            InitDownChannel();
            InitUpChannel();
            if (_downStream.CanRead)
            {
                char[] status = new char[20];
                for (int i = 0; i < 20; i++)
                {
                    status[i] = (char)ReadByte();
                }
                String got = new String(status);
                if (got != "http tunnel accept=1")
                {
                    throw new ProtocolException();
                }
                Debug.WriteLine("VivotekTunnel READY");
            }
        }

        public void Close()
        {
            if (_upStream != null)
            {
                try
                {
                    _upStream.Close();
                }
                catch { }
            }
            if (_downStream != null)
            {
                try
                {
                    _downStream.Close();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            Close();
            if (_upStream != null)
            {
                _upStream.Dispose();
                _upStream = null;
            }
            if (_downStream != null)
            {
                _downStream.Dispose();
                _downStream = null;
            }
        }

        public long BytesToRead
        {
            get
            {
                return _downStream.Length;
            }
        }

        public int ReadByte()
        {
            return _downStream.ReadByte();
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            try
            {
                String b64string = Convert.ToBase64String(bytes, offset, length);
                byte[] b64bytes = Encoding.ASCII.GetBytes(b64string);
                Debug.WriteLine(String.Format("Sending: {0} ({1})", b64string, ByteArrayToString(bytes)));
                _upStream.Write(b64bytes, 0, b64bytes.Length);
                _upStream.Flush();
            }
            catch (Exception e)
            {
                ErrorLogger.DumpToDebug(e);
                Close();
                throw;
            }
        }

        private static String ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2} ", b);
            }
            return hex.ToString();
        }

        #endregion

        private void WriteString(String message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            Write(bytes, 0, bytes.Length);
        }

        private void WriteStringAsIs(String message)
        {
            Debug.WriteLine("Sending: " + message);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            _upStream.Write(bytes, 0, bytes.Length);
            _upStream.Flush();
        }

        private void InitDownChannel()
        {
            _downChannel = (HttpWebRequest)WebRequest.Create(_uri.ToString());
            _downChannel.ProtocolVersion = HttpVersion.Version10;
            _downChannel.Method = "GET";
            _downChannel.Host = _uri.Host;
            _downChannel.UserAgent = "FC VVTK Tunnel";
            _downChannel.Headers.Add("x-sessioncookie", _sessionCookie);
            _downChannel.Accept = "application/x-vvtk-tunnelled";
            _downChannel.Headers.Add("Pragma", "no-cache");
            _downChannel.Headers.Add("Cache-Control", "no-cache");
            _downChannel.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)_downChannel.GetResponse();
            _downStream = response.GetResponseStream();
//            _downStream.ReadTimeout = 10000;
            Debug.WriteLine("VivotekTunnel download channel initialized");
        }

        private void GetRequestStreamCallback(IAsyncResult asyncResult)
        {
            _gotRequestStream.Set();
        }

        private void InitUpChannel()
        {
            _upChannel = (HttpWebRequest)WebRequest.Create(_uri.ToString());
            _upChannel.AllowWriteStreamBuffering = false;
            _upChannel.ProtocolVersion = HttpVersion.Version10;
            _upChannel.Method = "POST";
            _upChannel.Host = _uri.Host;
            _upChannel.ContentType = "application/x-vvtk-tunnelled";
            _upChannel.Headers.Add("Expires", "Sun, 9 Jan 1972 00:00:00 GMT");
            _upChannel.Headers.Add("x-sessioncookie", _sessionCookie);
            _upChannel.Headers.Add("Pragma", "no-cache");
            _upChannel.Headers.Add("Cache-Control", "no-cache");
            _upChannel.ContentLength = 32767;
            _upChannel.KeepAlive = true;
            IAsyncResult asyncResult = _upChannel.BeginGetRequestStream(GetRequestStreamCallback, _upChannel);
            _gotRequestStream.WaitOne();
            _upStream = _upChannel.EndGetRequestStream(asyncResult);
            WriteStringAsIs("channel=0");
            Debug.WriteLine("VivotekTunnel upload channel initialized");
        }
    }
}
