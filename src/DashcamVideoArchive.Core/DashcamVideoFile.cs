namespace DashcamVideoArchive.Core
{
    public class DashcamVideoFile
    {
        public required string Name { get; set; }

        public required string Path { get; set; }

        public long Size { get; set; }

        public DateTime Time { get; set; }

        public virtual bool IsParkingRecording => Path.Contains("Parking");
        public virtual bool IsEventRecording => Path.Contains("RO");

        public virtual string GetStoragePath(string rideName)
        {
            if (string.IsNullOrEmpty(rideName))
                throw new ArgumentNullException(nameof(rideName));

            if (IsParkingRecording)
                return $"rides/{rideName}/parking/{Name}";
            else
                return $"rides/{rideName}/{Name}";
        }
    }
}
