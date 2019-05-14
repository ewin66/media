using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FutureConcepts.Media.Server
{
    /// <summary>
    /// Describes a media source 
    /// </summary>
    [Serializable]
    public class StartSource
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public StartSource()
        {
        }

        private string _sourceName;
        /// <summary>
        /// A unique name identifying this source
        /// </summary>
        [XmlElement]
        public string SourceName
        {
            get
            {
                return _sourceName;
            }
            set
            {
                if (_sourceName != value)
                {
                    _sourceName = value;
                }
            }
        }
    }
}
