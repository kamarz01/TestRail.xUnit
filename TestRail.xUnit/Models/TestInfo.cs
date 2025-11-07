using Xunit.Sdk;
using Zaghloul.QA.TestRail.xUnit.TestRail.Enums;

namespace Zaghloul.QA.TestRail.xUnit.Models
{
    public class TestInfo
    {
        public string UniqueId { get; set; }

        public string DisplayName { get; set; }

        public string SuiteName { get; set; }

        public string SectionName { get; set; }

        public List<string> CaseIds { get; set; } = new();

        public TestResultStatus Status { get; set; }

        public IXunitTestCase RawTestCase { get; set; }
    }
}