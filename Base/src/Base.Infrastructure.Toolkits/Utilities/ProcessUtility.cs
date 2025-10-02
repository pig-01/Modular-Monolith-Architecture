using System.Diagnostics;
using Base.Domain.Exceptions;

namespace Base.Infrastructure.Toolkits.Utilities;

public class ProcessUtility
{
    /// <summary>
    /// Start a process with the given file name and arguments.
    /// </summary>
    /// <param name="workingDirectory">working directory</param>
    /// <param name="fileName">file name</param>
    /// <param name="arguments">arguments</param>
    /// <param name="beforeStart">before start action</param>
    /// <param name="afterFinish">after finish action</param>
    /// <exception cref="ProcessException">process exception</exception>
    /// <exception cref="Exception">exception</exception>
    public static string StartProcess(string workingDirectory, string fileName, string arguments = "", Action? beforeStart = null, Action? afterFinish = null)
    {
        // Invoke the before start action.
        beforeStart?.Invoke();

        // Setup the process start info.
        ProcessStartInfo processStartInfo = new()
        {
            WorkingDirectory = workingDirectory,
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Start the process.
        using Process process = new() { StartInfo = processStartInfo };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new ProcessException($"Process exited with code {process.ExitCode}: {error}");
        }

        // Invoke the after finish action.
        afterFinish?.Invoke();

        return output;
    }
}
