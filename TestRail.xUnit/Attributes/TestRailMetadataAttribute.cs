namespace Zaghloul.QA.TestRail.xUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class TestRailMetadataAttribute : Attribute
    {
        public string SuiteName { get; set; }

        public string SectionName { get; set; }

        public string[] CaseIds { get; set; }

        public TestRailMetadataAttribute(string suiteId)
        {
            SuiteName = suiteId;
        }

        public TestRailMetadataAttribute(string suiteName, string[] caseIds)
        {
            SuiteName = suiteName;
            CaseIds = caseIds;
        }

        public TestRailMetadataAttribute(string suiteName, string sectionName)
        {
            SuiteName = suiteName;
            SectionName = sectionName;
        }
    }
}