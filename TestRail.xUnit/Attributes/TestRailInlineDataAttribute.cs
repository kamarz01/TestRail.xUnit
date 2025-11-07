using System.Reflection;
using Xunit.Sdk;

namespace Zaghloul.QA.TestRail.xUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestRailInlineDataAttribute : DataAttribute
    {
        public string[] CaseIds { get; }

        public object[] Data { get; }

        public TestRailInlineDataAttribute(string[] caseIds, params object[] data)
        {
            CaseIds = caseIds;
            Data = data;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return new[] { Data };
        }
    }
}