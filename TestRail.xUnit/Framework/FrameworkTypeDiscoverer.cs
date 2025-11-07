using Xunit.Abstractions;
using Xunit.Sdk;

namespace Zaghloul.QA.TestRail.xUnit.Framework
{
    public class FrameworkTypeDiscoverer : ITestFrameworkTypeDiscoverer
    {
        public Type GetTestFrameworkType(IAttributeInfo attribute)
        {
            return typeof(TestRailxUnitFramework);
        }
    }
}