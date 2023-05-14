using System.Text;
using System.Text.Json.Nodes;

namespace Stryker.SolutionRunner;

internal static class ReportManager
{
    /// <summary>Adds files from <paramref name="additionalJson" /> into <paramref name="reportJson" />.</summary>
    private static void AddFiles(JsonNode reportJson, JsonNode additionalJson) {
        var reportJsonFiles = reportJson["files"]!.AsObject();
        var additionalContentFiles = additionalJson["files"]!.AsObject();

        foreach (var property in additionalContentFiles.AsObject().ToArray()) {
            property.Value!.Parent!.AsObject().Remove(property.Key);
            reportJsonFiles.Add(property);
        }
    }

    /// <summary>Returns <see cref="FileInfo"/> of file that is newer thant report file.</summary>
    private static FileInfo? GetFileNewerThanReport(TestInfo testInfo) {
        var report = testInfo.ReportPath;
        var reportWriteTime = File.GetLastWriteTime(report);
        return EnumerateFiles(testInfo).FirstOrDefault(file => file.LastWriteTime > reportWriteTime);
    }

    /// <summary>Enumerate all files in test and tested project directories excluding files in 'bin' and 'obj' directories.</summary>
    private static IEnumerable<FileInfo> EnumerateFiles(TestInfo testInfo) {
        foreach (var file in EnumerateProjectFiles(testInfo.TestProjectDir)) {
            yield return file;
        }

        foreach (var file in EnumerateProjectFiles(testInfo.TestedProjectDir)) {
            yield return file;
        }

    }

    /// <summary>Enumerate all files in <paramref name="projectDir"/> excluding files in 'bin' and 'obj' folders.</summary>
    private static IEnumerable<FileInfo> EnumerateProjectFiles(string projectDir) {
        var binPath = Path.Combine(projectDir, "bin") + Path.DirectorySeparatorChar;
        var objPath = Path.Combine(projectDir, "obj") + Path.DirectorySeparatorChar;

        var files = new DirectoryInfo(projectDir).EnumerateFiles("*.*", SearchOption.AllDirectories);
        foreach (FileInfo file in files) {

            if (file.FullName.StartsWith(binPath) ||
                file.FullName.StartsWith(objPath)) {
                continue;
            }

            yield return file;
        }
    }

    /// <summary>Merge all json reports into single json report</summary>
    private static JsonNode MergeReports(TestInfo[] testInfos) {
        var mergedJson = JsonNode.Parse(Resource.mutation_report)!;

        foreach (var testInfo in testInfos) {
            JsonNode reportJson;
            using (var stream = File.OpenRead(testInfo.ReportPath)) {
                reportJson = JsonNode.Parse(stream)!;
            }

            AddFiles(mergedJson, reportJson);
        }

        return mergedJson;
    }

     /// <summary>
     /// Update report json if needed.
     /// </summary>
     /// <param name="solutionFullPath"></param>
     /// <param name="testInfo"></param>
     /// <param name="force">If set report will be updated.</param>
     /// <param name="extraArgs">Additional stryker arguments.</param>
     /// <returns>True if report was updated, otherwise false.</returns>
    public static bool UpdateReport(string solutionFullPath, TestInfo testInfo, bool force, string[] extraArgs) {
        Console.Write("Test project: ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(Path.GetFileName(testInfo.TestProjectPath));
        Console.ResetColor();

        bool upToDate = true;
        if (!File.Exists(testInfo.ReportPath)) {
            Console.Write("Project report file not found.");
            upToDate = false;
        }

        if (upToDate && !force) {
            var file = GetFileNewerThanReport(testInfo);
            if (file != null) {
                Console.Write("File ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(file.FullName);
                Console.ResetColor();
                Console.WriteLine(" is newer that report file.");
                upToDate = false;
            }
        }

        if (upToDate) {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Project report is up to date.");
            Console.ResetColor();
            return false;
        }

        if (!DotnetWrapper.RunStryker(solutionFullPath, testInfo.TestedProjectName, testInfo.TestProjectPath, extraArgs)) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Stryker exited with error.");
            Console.ResetColor();
            return false;
        }

        var report = Path.Combine(Directory.GetCurrentDirectory(), "StrykerOutput", "reports", "mutation-report.json");

        File.Copy(report, testInfo.ReportPath, true);

        return true;
    }

    /// <summary>Write merged report to html file.</summary>
    public static string WriteReport(string solutionFullPath, TestInfo[] testInfos) {
        var solutionName = Path.GetFileNameWithoutExtension(solutionFullPath);

        var reportJson = MergeReports(testInfos);

        var finalReport = Path.ChangeExtension(solutionFullPath, ".html");

        var report = Resource.ReportTemplate;
        report = report.Replace("##REPORT_JSON##", reportJson.ToJsonString());
        report = report.Replace("##REPORT_TITLE##", solutionName + " Stryker report");
        report = report.Replace("##REPORT_JS##", Resource.mutation_test_elements);

        File.WriteAllText(finalReport, report, Encoding.UTF8);
        return finalReport;
    }
}
