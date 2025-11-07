using Reqnroll;
using Zaghloul.QA.TestRail.xUnit.Bdd.Helpers;
using Zaghloul.QA.TestRail.xUnit.Helpers;
using Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;

namespace Zaghloul.QA.TestRail.xUnit.Bdd.Reqnroll
{
    [Binding]
    public class ScenarioManager
    {
        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (TestManager.ReportingEnabled)
            {
                var matchingTest = BddFrameworkHelper.GetMatchingScenario(scenarioContext);
                if (matchingTest != null)
                    AddResult(matchingTest.UniqueId, scenarioContext, featureContext);
            }
        }

        private void AddResult(string testUniqueId, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (!TestManager.TestCases.TryGetValue(testUniqueId, out var meta))
                return;

            var tags = ScenarioTagHelper.GetSuiteAndSectionTags(featureContext);

            meta.DisplayName = TestFrameworkHelper.GetTestRailTitleWithLimit(meta.DisplayName);
            meta.Status = TestResultMapper.GetTestStatus(scenarioContext.ScenarioExecutionStatus);
            meta.CaseIds = meta.CaseIds.Select(x => x.Replace("C", "", StringComparison.InvariantCultureIgnoreCase)).ToList();
            meta.SuiteName = tags.SuiteName;
            meta.SectionName = tags.SectionName;

            TestManager.TestResults.AddOrUpdate(meta.UniqueId, meta, (key, existing) => meta);
        }
    }
}