// See https://aka.ms/new-console-template for more information
using Stryker.SolutionRunner;

if (args.Length == 0) {
    Console.WriteLine("USAGE: Stryker.SolutionRunner Solution.sln [--force] [stryker args]");
    return;
}

var solutionFullPath = args[0];
var force = args.Length > 1 && args[1] == "--force";

var strykerArgs = args.Skip(force ? 2 : 1).ToArray();

var testInfos = ProjectResolver.GetTestInfos(solutionFullPath).ToArray();

bool reportUpdated = false;
foreach (var testInfo in testInfos) {
    reportUpdated |= ReportManager.UpdateReport(solutionFullPath, testInfo, force, strykerArgs);
}

if (reportUpdated) {
    ReportManager.WriteReport(solutionFullPath, testInfos);
}
