using Reqnroll;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Sdk;
using Zaghloul.QA.TestRail.xUnit.Bdd.Reqnroll;
using Zaghloul.QA.TestRail.xUnit.Helpers;
using Zaghloul.QA.TestRail.xUnit.Models;

namespace Zaghloul.QA.TestRail.xUnit.Bdd.Helpers
{
    public class BddFrameworkHelper
    {
        public ConcurrentDictionary<string, TestInfo> ExtractMetadata(IEnumerable<IXunitTestCase> testCases)
        {
            var result = new ConcurrentDictionary<string, TestInfo>();

            foreach (var testCase in testCases)
            {
                var testMethod = testCase.TestMethod.Method;
                var testArgs = testCase.TestMethodArguments;
                var testId = testCase.UniqueID;
                var caseIds = new List<string>();

                if (testMethod.GetCustomAttributes(typeof(InlineDataAttribute)).Any())
                {
                    var caseId = TestFrameworkHelper.ExtractCaseIdFromInlineData(testArgs);
                    if (!string.IsNullOrWhiteSpace(caseId))
                        caseIds.Add(caseId);
                }

                else
                {
                    var title = testCase.DisplayName ?? string.Empty;
                    var match = Regex.Match(title, @"C\d+", RegexOptions.IgnoreCase);
                    if (match.Success)
                        caseIds.Add(match.Value);
                }

                result.TryAdd(testId, new TestInfo
                {
                    RawTestCase = testCase,
                    UniqueId = testId,
                    DisplayName = TestFrameworkHelper.GetDisplayName(testCase),
                    CaseIds = caseIds.Distinct().ToList()
                });
            }

            return result;
        }

        public static TestInfo GetMatchingScenario(ScenarioContext scenarioContext)
        {
            var title = scenarioContext.ScenarioInfo.Title;
            var args = scenarioContext.ScenarioInfo.Arguments?.Values.Cast<object>().ToArray() ?? Array.Empty<object>();

            return TestManager.TestCases.Values.FirstOrDefault(t =>
            {
                var rawTestCase = t.RawTestCase;
                var displayName = GetScenarioDisplayName(rawTestCase);

                var factName = displayName?.Equals(title, StringComparison.OrdinalIgnoreCase) == true;
                if (!factName) return false;

                var xunitArgs = NormalizeReqnrollArgs(rawTestCase.TestMethodArguments);

                if (xunitArgs == null || xunitArgs.Length == 0)
                    return true;

                if (args.Length != xunitArgs.Length)
                    return false;

                for (int i = 0; i < args.Length; i++)
                {
                    if (!string.Equals(args[i]?.ToString(), xunitArgs[i]?.ToString(), StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return true;
            });
        }

        private static string GetScenarioDisplayName(IXunitTestCase testCase)
        {
            if (testCase == null)
                return null;

            if (testCase.Traits.TryGetValue("Description", out var values))
            {
                return values?.FirstOrDefault();
            }

            return null;
        }

        private static object[] NormalizeReqnrollArgs(object[] xunitArgs)
        {
            if (xunitArgs == null)
                return Array.Empty<object>();

            return xunitArgs.Length > 2
                ? xunitArgs.Take(xunitArgs.Length - 2).ToArray()
                : xunitArgs;
        }
    }
}