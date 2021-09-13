using CommandLine;

namespace Overlord.UI.ConsoleApp
{
    public class CmdOptions
    {
        [Option('v', "video source", Required = true, Default = "", HelpText = "video source uri")]
        public string VideoSource { get; set; }

        [Option('r', "road definition", Required = true, Default = "", HelpText = "road definition file")]
        public string RoadDefinitionFile { get; set; }

        [Option('n', "no display", Required = false, Default = "", HelpText = "no display")]
        public bool NoDisplay { get; set; }
    }
}
