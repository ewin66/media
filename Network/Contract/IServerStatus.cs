using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FutureConcepts.Media.Network.Contract
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IServerStatus 
    {
        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        ServerStatusProperties Query(string sourceName);
    }
}
