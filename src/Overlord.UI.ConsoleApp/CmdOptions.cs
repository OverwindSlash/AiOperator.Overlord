using CommandLine;

namespace Overlord.UI.ConsoleApp
{
    public class CmdOptions
    {
        [Option('s', "setting file", Required = false, Default = "appSettings.json", HelpText = "app setting file")]
        public string AppSettingFile { get; set; }
        
        [Option('i', "pipeline index", Required = false, Default = "0", HelpText = "pipeline index")]
        public int PipelineIndex { get; set; }

        [Option('v', "video source", Required = false, Default = "", HelpText = "video source uri")]
        public string VideoUri { get; set; }

        [Option('r', "road definition", Required = false, Default = "", HelpText = "road definition file")]
        public string RoadDefinitionFile { get; set; }

        [Option('n', "no display", Required = false, Default = true, HelpText = "no display")]
        public bool NoDisplay { get; set; }
    }
}
