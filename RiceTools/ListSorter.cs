namespace RiceTools;

public class ListSorter
{
    public static ConfigParseCollection Sort(ConfigParseCollection configParseCollection)
    {
        MenuSorting listSorting = Program.ListSorting;
        
        if (listSorting == MenuSorting.Default)
            return configParseCollection;

        if (listSorting == MenuSorting.Alphabetic)
        {
            // TODO: Code Alphabetic sort functionality
            // Make sure to discard any coloring that may have been done to the name
            // Sort the commands the exact same way the names were
            return configParseCollection;
        }

        if (listSorting == MenuSorting.MostUsed)
        {
            // TODO: Code most used functionality
            // Make a file which stores how many times a certain command was run
            // Sort by how many times that command was run
            return configParseCollection;
        }
        return configParseCollection;
    }
}