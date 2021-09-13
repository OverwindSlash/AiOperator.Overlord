namespace Overlord.Domain.Settings
{
    public class PipelineSetting
    {
        public string Name { get; set; }
        public string VideoSource { get; set; }
        public string RoadDefinitionFile { get; set; }
        public int Fps { get; set; }
    }
}