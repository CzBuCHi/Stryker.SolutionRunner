namespace Stryker.SolutionRunner;

internal struct TestInfo
{
    /// <summary>Full path to tested project.</summary>
    public required string TestedProjectPath { get; init; }

    /// <summary>Full path to tested project directory.</summary>
    public string TestedProjectDir => Directory.GetParent(TestedProjectPath)!.FullName;

    /// <summary>Name of tested project </summary>
    public string TestedProjectName => Path.GetFileName(TestedProjectPath);

    /// <summary>Full path to test project.</summary>
    public required string TestProjectPath { get; init; }

    /// <summary>Full path to test project directory.</summary>
    public string TestProjectDir => Directory.GetParent(TestProjectPath)!.FullName;

    /// <summary>Full path to report file.</summary>
    public string ReportPath => Path.ChangeExtension(TestProjectPath, ".json");
}
