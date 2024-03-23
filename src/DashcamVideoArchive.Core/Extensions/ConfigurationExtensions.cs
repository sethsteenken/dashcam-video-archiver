using Microsoft.Extensions.Configuration;

namespace DashcamVideoArchive
{
    public static class ConfigurationExtensions
    {
        public static string GetRequiredValue(this IConfiguration configuration, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return configuration[key]
                ?? throw new InvalidOperationException($"Missing environment value '{key}'.");
        }

        public static string GetDashcamEndpoint(this IConfiguration configuration)
        {
            return GetRequiredValue(configuration, "DASHCAM_ENDPOINT");
        }

        public static string GetRideDateDirectoryFormat(this IConfiguration configuration)
        {
            return configuration.GetValue("RIDE_DATE_DIRECTORY_FORMAT", "yyyy_MM_dd_HH_mm")!;
        }

        public static int GetRideTimeSeparation(this IConfiguration configuration)
        {
            return configuration.GetValue("RIDE_TIME_SEPARATION", 5);
        }

        public static bool ShutdownOnCompleted(this IConfiguration configuration)
        {
            return configuration.GetValue("SHUTDOWN_ON_COMPLETED", false);
        }

        public static string GetDownloadDirectory(this IConfiguration configuration)
        {
            return configuration.GetValue("DOWNLOAD_DIRECTORY", "/downloads/dashcam")!;
        }
    }
}
