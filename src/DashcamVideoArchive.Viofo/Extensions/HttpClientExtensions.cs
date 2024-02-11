using DashcamVideoArchive.Viofo.Xml;

namespace DashcamVideoArchive.Viofo
{
    internal static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> SendCommandAsync(
            this HttpClient httpClient,
            CommandCodes commandCode,
            string? url = null)
        {
            return httpClient.GetAsync($"{url}?custom=1&cmd={(int)commandCode}");
        }
    }
}
