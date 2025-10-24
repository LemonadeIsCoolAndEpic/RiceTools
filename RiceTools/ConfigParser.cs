using Spectre.Console;

namespace RiceTools;

public class ConfigParser
{
    public static ConfigParseCollection Parse(string[] configLines)
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
        configParseCollection.Names = ItemNames;
        configParseCollection.Commands = ItemCommands;

        return configParseCollection;
    }
}