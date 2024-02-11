using System.Xml.Serialization;

namespace DashcamVideoArchive
{
    public static class XmlSerializerExtensions
    {
        public static T Deserialize<T>(this XmlSerializer serializer, string xml) where T : class
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentNullException(nameof(xml));

            using var reader = new StringReader(xml);

            return serializer.Deserialize(reader) as T ?? throw new NullReferenceException();
        }
    }
}
