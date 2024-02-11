using System.Xml.Serialization;

namespace DashcamVideoArchive.Viofo.Xml
{
    [Serializable, XmlRoot("LIST")]
    public class FileList
    {
        [XmlElement("ALLFile")]
        public required List<AllFile> AllFiles { get; set; }
    }
}
