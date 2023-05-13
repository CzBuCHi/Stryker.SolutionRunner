namespace Stryker.SolutionRunner;

internal static class ProjectResolver
{
    // TODO: assuming test project suffix
    private const string TestProjectNameSuffix = ".Tests";

    /// <summary>Resolve test-tested projects in solution.</summary>
    /// <param name="solutionFullPath">Full path to solution.</param>
    /// <returns>Enumeration of test-tested projects.</returns>
    public static IEnumerable<TestInfo> GetTestInfos(string solutionFullPath) {
        var projects = DotnetWrapper.EnumerateProjects(solutionFullPath).ToHashSet();

        var testProjects = projects.Where(o => o.EndsWith(TestProjectNameSuffix + ".csproj"));

        foreach (var testProject in testProjects) {
            var testInfo = TryGetTestInfo(testProject);
            if (testInfo != null) {
                yield return testInfo.Value;
            }
        }
    }

    /// <summary>Try resolve tested project for given <paramref name="testProject"/>.</summary>
    /// <param name="testProject">Full path to test project.</param>
    /// <returns>New instance od <see cref="TestInfo"/> -or- null if tested project not resolved.</returns>
    private static TestInfo? TryGetTestInfo(string testProject) {
        var references = DotnetWrapper.EnumerateReferences(testProject).ToArray();
        if (references.Length == 0) {
            return null;
        }

        // assuming single project reference is tested project
        if (references.Length == 1) {
            return new TestInfo {
                TestProjectPath = testProject,
                TestedProjectPath = references[0],
            };
        }

        // for multiple references try find by project name
        var projectName = Path.GetFileNameWithoutExtension(testProject);
        foreach (var reference in references) {
            var referenceName = Path.GetFileNameWithoutExtension(reference);

            if (referenceName + TestProjectNameSuffix == projectName) {
                return new TestInfo {
                    TestProjectPath = testProject,
                    TestedProjectPath = reference,
                };
            }
        }

        // not sure what to do ...
        throw new NotImplementedException();
    }
}