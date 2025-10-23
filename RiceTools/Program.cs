using System.Diagnostics;
using Spectre.Console;

namespace RiceTools
{
    class Program
    {
        private static string configDirectory;
        
        static void Main(string[] args)
        {
            
            
            AnsiConsole.Write(new FigletText("RiceTools!").Centered().Color(Color.DarkGoldenrod));

            if (args.Contains("--help") || args.Contains("?") || args.Contains("-h"))
            {
                AnsiConsole.Markup(
                    "[bold darkgoldenrod]RiceTools[/] is a simple, easy to use solution \nto launch/run commands commands from the terminal in a bookmark like manner\n" +
                    "You can customise the names and commands in \".config/ricetools/ricetools.config\"" +
                    "   --help/-h/?            offers help and guidance about the ricetools utility");
                return;
            }
            
            configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.config/ricetools/ricetools.config";
            if (!File.Exists(configDirectory))
            {
                AnsiConsole.Markup("[bold blue]It appears that your config has not been setup yet \nWould you like RiceTools to create one?[/]\n");
                bool createConfig = AnsiConsole.Prompt(
                    new TextPrompt<bool>("Generate Config?")
                        .AddChoice(true)
                        .AddChoice(false)
                        .DefaultValue(true)
                        .WithConverter(choice => choice ? "Y" : "n"));
                if (createConfig)
                {
                    Directory.CreateDirectory(configDirectory.Replace("/ricetools.config", string.Empty));
                    File.Create(configDirectory).Dispose();
                    File.AppendAllLines(configDirectory, new List<string>()
                    {
                        "# Add your executable options here",
                        "# In the format: name, command",
                        "# name = the way it will appear in the list",
                        "# command = what will be executed once that has been selected"
                    } );
                }
            }
            string[] config = File.ReadAllLines(configDirectory);
            ConfigParseCollection configParseCollection = ParseConfig(config);
            
            // Creates a selection menu to select the different command options
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to execute?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to show more tools!)[/]")
                    .AddChoices(configParseCollection.names.ToArray()));
            
            int selectionIndex = configParseCollection.names.IndexOf(selection);
            RunCommand(configParseCollection.commands[selectionIndex]);
        }

        static void RunCommand(string command)
        {
            // Directory to BASH SHELL
            var shell = "/bin/bash"; 
    
            // Need to use shell with -c command for it to work?
            var args = $"-c \"{command}\"";

            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = args, 
                    UseShellExecute = false, // Must be false for redirecting streams/process control
                    RedirectStandardOutput = false, // Keep false if the external command handles its own output
                    RedirectStandardError = false, // Keep false if the external command handles its own errors
                    CreateNoWindow = false
                });

                // Wait for the process to exit before returning to the menu
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running command: {ex.Message}");
            }
            Console.Clear();
        }

        public static ConfigParseCollection ParseConfig(string[] configLines)
        {
            List<string> ItemNames = new List<string>();
            List<string> ItemCommands = new List<string>();
            
            foreach (var line in configLines)
            {
                string alteredLine = line;
                
                // Remove/Disguard of ANY comments
                int commentPosition = line.IndexOf("#");
                if (commentPosition != -1)
                    alteredLine = line.Remove(commentPosition);
                if (alteredLine == String.Empty)
                    continue;
                
                // Parse the Name and Command
                string[] parsedLine = alteredLine.Split(",");
                if (parsedLine.Length < 2)
                {
                    // Format is incorrect if it is less than 2
                    AnsiConsole.MarkupLine($"[red]Config Error: \"" + alteredLine + "\" is formatted incorrectly[/]");
                }
                
                // Trim the white space
                string name = parsedLine[0].Trim();
                string command = parsedLine[1].Trim();
                
                ItemNames.Add(name);
                ItemCommands.Add(command);
            }

            ConfigParseCollection configParseCollection = new ConfigParseCollection();
            configParseCollection.names = ItemNames;
            configParseCollection.commands = ItemCommands;
                        
            return configParseCollection;
        }
    }

    public struct ConfigParseCollection
    {
        public List<string> names;
        public List<string> commands;
    }
}