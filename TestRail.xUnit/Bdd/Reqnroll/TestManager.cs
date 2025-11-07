using Reqnroll;
using System.Collections.Concurrent;
using System.Diagnostics;
using Xunit.Sdk;
using Zaghloul.QA.TestRail.xUnit.Bdd.Helpers;
using Zaghloul.QA.TestRail.xUnit.Config;
using Zaghloul.QA.TestRail.xUnit.Helpers;
using Zaghloul.QA.TestRail.xUnit.Models;

namespace Zaghloul.QA.TestRail.xUnit.Bdd.Reqnroll
{
    [Binding]
    public static class TestManager
    {
        public static bool ReportingEnabled = true;

        public static ConcurrentDictionary<string, TestInfo> TestCases;

        public static ConcurrentDictionary<string, TestInfo> TestResults = new();

        public static TestRailConfiguration Configuration => AppConfigHelper.GetAppConfigurations();

        [BeforeTestRun]
        public static async Task InitAndCheck(ITestRunnerManager testRunnerManager)
        {
            var testRunner = testRunnerManager.GetTestRunner();

            var initResult = await Init.Check();
            ReportingEnabled = initResult.IsValid;

            if (ReportingEnabled)
            {
                var testAssembly = testRunnerManager.TestAssembly;
                var testCases = AssemblyHelper.GetTestsFromAssembly(testAssembly.Location);
                var xunitTestCases = testCases.OfType<IXunitTestCase>();
                TestCases = new BddFrameworkHelper().ExtractMetadata(xunitTestCases);
            }
        }

        [AfterTestRun]
        public async static Task Finalize(ITestRunnerManager testRunnerManager)
        {
            if (ReportingEnabled)
            {
                try
                {
                    await new TestRailHelper().PublishResults(TestResults);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[xUnit-TestRail-Plugin] Result Publish Failed: {ex.Message}");
                }
            }
        }
    }
}