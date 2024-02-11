﻿using System.Xml.Serialization;

namespace DashcamVideoArchive.Viofo.Xml
{
    public class File
    {
        [XmlElement("NAME")]
        public string? Name { get; set; }

        [XmlElement("FPATH")]
        public string? FilePath { get; set; }

        [XmlElement("SIZE")]
        public long Size { get; set; }

        [XmlElement("TIMECODE")]
        public long TimeCode { get; set; }

        [XmlElement("TIME")]
        public string? Time { get; set; }

        [XmlElement("ATTR")]
        public int Attribute { get; set; }
    }
}
