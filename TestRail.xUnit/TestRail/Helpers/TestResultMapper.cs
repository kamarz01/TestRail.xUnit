using Reqnroll;
using Zaghloul.QA.TestRail.xUnit.TestRail.Enums;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Helpers
{
    public class TestResultMapper
    {
        public static TestResultStatus GetTestStatus(string outcome)
        {
            return outcome switch
            {
                var value when value.Equals("Passed", StringComparison.InvariantCultureIgnoreCase) => TestResultStatus.Passed,
                var value when value.Equals("Failed", StringComparison.InvariantCultureIgnoreCase) => TestResultStatus.Failed,
                _ => TestResultStatus.Untested
            };
        }

        public static TestResultStatus GetTestStatus(ScenarioExecutionStatus status)
        {
            return status switch
            {
                ScenarioExecutionStatus.OK => TestResultStatus.Passed,
                ScenarioExecutionStatus.TestError or ScenarioExecutionStatus.BindingError or ScenarioExecutionStatus.StepDefinitionPending => TestResultStatus.Failed,
                ScenarioExecutionStatus.Skipped => TestResultStatus.Untested,
                _ => TestResultStatus.Untested
            };
        }
    }
}