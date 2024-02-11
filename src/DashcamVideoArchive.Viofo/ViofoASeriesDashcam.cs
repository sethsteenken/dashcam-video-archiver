using System.Xml.Serialization;
using DashcamVideoArchive.Viofo.Xml;
using File = DashcamVideoArchive.Viofo.Xml.File;

namespace DashcamVideoArchive.Viofo
{
    public class ViofoASeriesDashcam
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

        public async Task PrintFilesAsync()
        {
            var files = await GetFilesAsync();

            foreach (var file in files)
            {
                Console.WriteLine(file.Name);
                Console.WriteLine($" - {file.FilePath}");
                Console.WriteLine($" - {file.Size}");
                Console.WriteLine($" - {file.Time}");
            }
        }

        private async Task<IEnumerable<File>> GetFilesAsync()
        {
            var response = await _httpClient.SendCommandAsync(CommandCodes.GetFiles);

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<File>();

            string xml = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(FileList));

            using (TextReader reader = new StringReader(xml))
            {
                var result = serializer.Deserialize(reader) as FileList;
                if (result is null)
                    return Enumerable.Empty<File>();

                return result.AllFiles.Select(f => f.File);
            }
        }
    }
}
