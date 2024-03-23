using DashcamVideoArchive.Core;

namespace DashcamVideoArchive.Viofo
{
    public class ViofoVideoFile : DashcamVideoFile
    {
        public override bool IsEventRecording => Path.Contains("RO");
    }
}
