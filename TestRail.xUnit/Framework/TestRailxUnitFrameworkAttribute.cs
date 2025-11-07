using Xunit.Sdk;

namespace Zaghloul.QA.TestRail.xUnit.Framework
{
    [AttributeUsage(AttributeTargets.Assembly)]
    [TestFrameworkDiscoverer("Zaghloul.QA.TestRail.xUnit.Framework.FrameworkTypeDiscoverer", "Zaghloul.QA.TestRail.xUnit")]
    public class TestRailxUnitFrameworkAttribute : Attribute, ITestFrameworkAttribute
    {
    }
}