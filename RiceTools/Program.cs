using Spectre.Console;

namespace RiceTools;

internal class Program
{
    private static void Main(string[] args)
    {
        // Display the title screen
        AnsiConsole.Write(new FigletText("RiceTools!").Centered().Color(Color.DarkGoldenrod));
        AnsiConsole.Write(new Rule().RuleTitle("ricetools v0.2").HeavyBorder());
        AnsiConsole.WriteLine();

        // Handle command arguments (e.g. --help)
        if (args.Contains("--help") || args.Contains("?") || args.Contains("-h"))
        {
            AnsiConsole.Markup(
                "[bold darkgoldenrod]RiceTools[/] is a simple, easy to use solution \nto launch/run commands commands from the terminal in a bookmark like manner\n" +
                "You can customise the names and commands in \".config/ricetools/ricetools.config\"" +
                "   --help/-h/?            offers help and guidance about the ricetools utility");
            return;
        }

        // Locate where the .config file should be
        string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                          "/.config/ricetools/ricetools.config";
        // If the config does NOT exist, prompt the user to auto generate it for them
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
                // Create the config directories and file
                Directory.CreateDirectory(configDirectory.Replace("/ricetools.config", string.Empty));
                File.Create(configDirectory).Dispose();
                // Then write some basic comments to get the user started
                // TODO: Maybe make this clone a config preset from github
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
        var configParseCollection = ConfigParser.Parse(config);

        // Creates a selection menu to select the different command options
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to execute?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to show more tools!)[/]")
                .AddChoices(configParseCollection.Names.ToArray()).EnableSearch());

        var selectionIndex = configParseCollection.Names.IndexOf(selection);
        ShellManager.RunCommand(configParseCollection.Commands[selectionIndex]);
    }
}