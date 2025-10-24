using System.Diagnostics;

namespace RiceTools;

public class ShellManager
{
    public static void RunCommand(string command)
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
}