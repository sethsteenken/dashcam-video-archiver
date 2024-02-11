using DashcamVideoArchive.Core;
using DashcamVideoArchive.Viofo.Xml;
using System.Xml.Serialization;

namespace DashcamVideoArchive.Viofo
{
    public class ViofoASeriesDashcam : IDashcam
    {
        private readonly HttpClient _httpClient;

        public ViofoASeriesDashcam(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var response = await _httpClient.SendCommandAsync(CommandCodes.Heartbeat);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<IEnumerable<FootageVideoFile>> GetFilesAsync()
        {
            var response = await _httpClient.SendCommandAsync(CommandCodes.GetFiles);

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<FootageVideoFile>();

            string xml = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(FileList));

            var result = serializer.Deserialize<FileList>(xml);
            if (result is null)
                return Enumerable.Empty<FootageVideoFile>();

            return result.AllFiles.Select(f => f.File).Select(f => new FootageVideoFile()
            {
                Name = f.Name ?? throw new NullReferenceException("Name is required."),
                Path = f.FilePath ?? throw new NullReferenceException("FilePath is required."),
                Time = DateTime.Parse(f.Time),
                Size = f.Size
            });
        }
    }
}
