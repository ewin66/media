using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureConcepts.Media.Client.StreamViewer
{
    public class ServerGraphException : Exception
    {
        public ServerGraphException(String message)
            : base(message)
        {
        }
    }
}
