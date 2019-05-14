using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Client.CameraControlClients.Transport
{
    public interface ITransport : IDisposable
    {
        void Open();
        void Close();
        void Write(byte[] bytes, int offset, int length);
        long BytesToRead { get; }
        int ReadByte();
    }
}
