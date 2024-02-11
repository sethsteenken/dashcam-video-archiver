namespace DashcamVideoArchive.Core
{
    public class FootageVideoFile
    {
        public required string Name { get; set; }

        public required string Path { get; set; }

        public long Size { get; set; }

        public DateTime Time { get; set; }
    }
}
