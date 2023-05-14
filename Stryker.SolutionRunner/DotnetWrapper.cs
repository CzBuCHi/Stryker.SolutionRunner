using System.Diagnostics;

namespace Stryker.SolutionRunner;

internal static class DotnetWrapper
{
    // TODO: Can be this path loaded from somewhere?
    public const string Dotnet = @"C:\Program Files\dotnet\dotnet.exe";

    /// <summary>Uses 'dotnet sln list' to resolve projects in solution.</summary>
    /// <param name="solutionFullPath">Full path to solution.</param>
    /// <returns>Absolute paths to projects in solution.</returns>
    public static IEnumerable<string> EnumerateProjects(string solutionFullPath) {
        var info = new ProcessStartInfo {
            FileName = Dotnet,
            Arguments = $"sln {solutionFullPath} list",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        var process = Process.Start(info)!;
        process.WaitForExit();

        process.StandardOutput.ReadLine(); // Project(s)
        process.StandardOutput.ReadLine(); // ----------

        var solutionDir = Path.GetDirectoryName(solutionFullPath)!;

        while (!process.StandardOutput.EndOfStream) {
            var project = process.StandardOutput.ReadLine()!;
            yield return Path.Combine(solutionDir, project);
        }
    }

    /// <summary>Uses 'dotnet list Project.csproj reference' to resolve project-to-project references.</summary>
    /// <param name="projectFullPath">Full path to project.</param>
    /// <returns>Absolute paths to references projects in solution.</returns>
    public static IEnumerable<string> EnumerateReferences(string projectFullPath) {
        var info = new ProcessStartInfo {
            FileName = Dotnet,
            Arguments = $"list {projectFullPath} reference",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        var process = Process.Start(info)!;
        process.WaitForExit();

        process.StandardOutput.ReadLine(); // Project reference(s)
        process.StandardOutput.ReadLine(); // --------------------

        var projectDir = Path.GetDirectoryName(projectFullPath)!;

        while (!process.StandardOutput.EndOfStream) {
            var reference = process.StandardOutput.ReadLine()!;
            var referencePath = Path.Combine(projectDir, reference);
            yield return Path.GetFullPath(referencePath);
        }
    }

    /// <summary>Runs 'dotnet stryker' command.</summary>
    /// <param name="solutionFullPath">Full path to your solution file.</param>
    /// <param name="projectName">Used to find the project to test in the project references of the test project.</param>
    /// <param name="testProject">Specify the test projects.</param>
    /// <returns></returns>
    public static bool RunStryker(string solutionFullPath, string projectName, string testProject, string[] extraArgs) {
        var arguments = new string[] {
            "stryker",
            "-s", solutionFullPath,
            "-p", projectName,
            "-tp", testProject,
            "-r", "json",
            "-O", "StrykerOutput"
        }.Concat(extraArgs);

        var info = new ProcessStartInfo {
            FileName = Dotnet,
            Arguments = string.Join(" ", arguments),
            UseShellExecute = false,
        };

        var process = Process.Start(info)!;
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}

