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

    /// <summary>Check if report is up to date.</summary>
    /// <returns>True if report is up to date, false otherwise.</returns>
    private static bool IsReportValid(TestInfo testInfo) {
        var report = testInfo.ReportPath;
        if (!File.Exists(report)) {
            return false;
        }

        var reportWriteTime = File.GetLastWriteTime(report);

        var testBin = Path.Combine(testInfo.TestProjectDir, "bin") + Path.DirectorySeparatorChar;
        var testObj = Path.Combine(testInfo.TestProjectDir, "obj") + Path.DirectorySeparatorChar;

        var testedBin = Path.Combine(testInfo.TestedProjectDir, "bin") + Path.DirectorySeparatorChar;
        var testedObj = Path.Combine(testInfo.TestedProjectDir, "obj") + Path.DirectorySeparatorChar;

        var testFiles = new DirectoryInfo(testInfo.TestProjectDir).EnumerateFiles("*.*", SearchOption.AllDirectories);
        var testedFiles = new DirectoryInfo(testInfo.TestedProjectDir).EnumerateFiles("*.*", SearchOption.AllDirectories);

        var lastWriteTime = testFiles.Concat(testedFiles)
                                     .Where(
                                          o =>
                                              o.FullName != report &&
                                              !o.FullName.StartsWith(testBin) && !o.FullName.StartsWith(testObj) &&
                                              !o.FullName.StartsWith(testedBin) && !o.FullName.StartsWith(testedObj)
                                      )
                                     .Max(o => o.LastWriteTime);

        return reportWriteTime >= lastWriteTime;
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
        Console.Write(Path.GetFileName(testInfo.TestProjectPath));

        if (!force && IsReportValid(testInfo)) {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(" up to date");
            Console.ResetColor();
            return false;
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(" updating ");
        Console.ResetColor();
        Console.WriteLine("...");

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
