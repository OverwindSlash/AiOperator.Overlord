namespace Overlord.Domain.Settings
{
    public class PipelineSettings
    {
        public string Name { get; set; }
        public string VideoUri { get; set; }
        public string RoadDefinitionFile { get; set; }
        public int Fps { get; set; }
        public int MinTriggerIntervalSecs { get; set; }
        public string CaptureRoot { get; set; }
        public string PublishApiUri { get; set; }
    }
}