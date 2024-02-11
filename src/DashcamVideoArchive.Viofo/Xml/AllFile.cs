using System.Xml.Serialization;

namespace DashcamVideoArchive.Viofo.Xml
{
    public class AllFile
    {
        [XmlElement("File")]
        public required File File { get; set; }
    }
}
