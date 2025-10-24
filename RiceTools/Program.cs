using System.Diagnostics;
using Spectre.Console;

namespace RiceTools;

internal class Program
{
    private static string configDirectory;

    private static void Main(string[] args)
    {
        AnsiConsole.Write(new Rule());
        AnsiConsole.Write(new FigletText("RiceTools!").Centered().Color(Color.DarkGoldenrod));
        AnsiConsole.Write(new Rule());

        if (args.Contains("--help") || args.Contains("?") || args.Contains("-h"))
        {
            AnsiConsole.Markup(
                "[bold darkgoldenrod]RiceTools[/] is a simple, easy to use solution \nto launch/run commands commands from the terminal in a bookmark like manner\n" +
                "You can customise the names and commands in \".config/ricetools/ricetools.config\"" +
                "   --help/-h/?            offers help and guidance about the ricetools utility");
            return;
        }

        configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                          "/.config/ricetools/ricetools.config";
        if (!File.Exists(configDirectory))
        {
            AnsiConsole.Markup(
                "[bold blue]It appears that your config has not been setup yet \nWould you like RiceTools to create one?[/]\n");
            var createConfig = AnsiConsole.Prompt(
                new TextPrompt<bool>("Generate Config?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "Y" : "n"));
            if (createConfig)
            {
                Directory.CreateDirectory(configDirectory.Replace("/ricetools.config", string.Empty));
                File.Create(configDirectory).Dispose();
                File.AppendAllLines(configDirectory, new List<string>
                {
                    "# Add your executable options here",
                    "# In the format: name, command",
                    "# name = the way it will appear in the list",
                    "# command = what will be executed once that has been selected"
                });
            }
        }

        var config = File.ReadAllLines(configDirectory);
        var configParseCollection = ParseConfig(config);

        // Creates a selection menu to select the different command options
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to execute?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to show more tools!)[/]")
                .AddChoices(configParseCollection.names.ToArray()).EnableSearch());

        var selectionIndex = configParseCollection.names.IndexOf(selection);
        RunCommand(configParseCollection.commands[selectionIndex]);
    }

    private static void RunCommand(string command)
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

            process.WaitForExit();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running command: {ex.Message}");
        }

        Console.Clear();
    }

    public static ConfigParseCollection ParseConfig(string[] configLines)
    {
        var ItemNames = new List<string>();
        var ItemCommands = new List<string>();

        foreach (var line in configLines)
        {
            var alteredLine = line;

            // Remove/Disguard of ANY comments
            var commentPosition = line.IndexOf("#");
            if (commentPosition != -1)
                alteredLine = line.Remove(commentPosition);
            if (alteredLine == string.Empty)
                continue;

            // Parse the Name and Command
            var parsedLine = alteredLine.Split(",");
            if (parsedLine.Length < 2)
                // Format is incorrect if it is less than 2
                AnsiConsole.MarkupLine("[red]Config Error: \"" + alteredLine + "\" is formatted incorrectly[/]");

            // Trim the white space
            var name = parsedLine[0].Trim();
            var command = parsedLine[1].Trim();

            ItemNames.Add(name);
            ItemCommands.Add(command);
        }

        var configParseCollection = new ConfigParseCollection();
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