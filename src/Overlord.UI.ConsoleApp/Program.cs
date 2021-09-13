using System;
using CommandLine;

namespace Overlord.UI.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(Run);
        }

        private static void Run(CmdOptions option)
        {
        }
    }
}
