using Spectre.Console;

namespace RiceTools;

public class ConfigParser
{
    public static ConfigParseCollection Parse(string[] configLines)
    {
        var ItemNames = new List<string>();
        var ItemCommands = new List<string>();
        ParseMode parsingStage = ParseMode.None;

        foreach (var line in configLines)
        {
            var alteredLine = line;

            // Remove/Disguard of ANY comments
            var commentPosition = line.IndexOf("#");
            if (commentPosition != -1)
                alteredLine = line.Remove(commentPosition);
            if (alteredLine == string.Empty)
                continue;

            // Utilize the last parse state to see if it changed at all
            ParseMode prevParseStage = parsingStage;
            parsingStage = CheckParseStage(alteredLine, parsingStage);
            
            // If it did change we need to ignore this line so we continue
            if (prevParseStage != parsingStage)
            {
                continue;
            }
            
            // Parse each area accordingly
            if (parsingStage ==  ParseMode.MenuOptions)
            {
                ParseMenuOptions(alteredLine, ItemNames, ItemCommands);
            }
            else if (parsingStage == ParseMode.MenuTheme)
            {
                if (line.Contains("MainColor="))
                    Theme.MainColor = ParseColor(line);
                else if (line.Contains("SelectionColor="))
                    Theme.SelectionColor = ParseColor(line);
            }
            else if (parsingStage == ParseMode.MenuSort)
            {
                if (line.Contains("sort"))
                {
                    string sort = line;
                    sort = sort.Replace("sort", "");
                    sort = sort.Replace("=", "");
                    sort = sort.Replace("\"", "");
                    sort = sort.Trim();
                    if (sort.ToLower() == "default")
                        Program.ListSorting = MenuSorting.Default;
                    else if (sort.ToLower() == "alphabetic")
                        Program.ListSorting = MenuSorting.Alphabetic;
                    else if (sort.ToLower() == "most used")
                        Program.ListSorting = MenuSorting.MostUsed;
                }
            }
        }

        var configParseCollection = new ConfigParseCollection();
        configParseCollection.Names = ItemNames;
        configParseCollection.Commands = ItemCommands;

        return configParseCollection;
    }

    private static string ParseColor(string line)
    {
        string color = line;
        color = color.Replace("MainColor", "");
        color = color.Replace("=", "");
        color = color.Replace("\"", "");
        return color.Trim();
    }
    
    private static void ParseMenuOptions(string alteredLine, List<string> ItemNames, List<string> ItemCommands)
    {
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

    private static ParseMode CheckParseStage(string line, ParseMode currentParseStage)
    {
        return line.Trim() switch
        {
            "[menu.options]" => ParseMode.MenuOptions,
            "[menu.theme]" => ParseMode.MenuTheme,
            "[menu.sort]" => ParseMode.MenuSort,
            _ => currentParseStage
        };
    }
}