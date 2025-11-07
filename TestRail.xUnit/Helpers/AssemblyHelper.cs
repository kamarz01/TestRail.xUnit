using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Zaghloul.QA.TestRail.xUnit.Helpers
{
    public class AssemblyHelper
    {
        public static string GetAssemblyVersion()
        {
            var infoVersion = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            return infoVersion?.Split('+')[0];
        }

        public static List<ITestCase> GetTestsFromAssembly(string assembly)
        {
            var discoverySink = new TestDiscoverySink();
            var frontController = new XunitFrontController(AppDomainSupport.IfAvailable, assembly);
            var discoveryOptions = TestFrameworkOptions.ForDiscovery();
            frontController.Find(false, discoverySink, discoveryOptions);
            discoverySink.Finished.WaitOne();
            return discoverySink.TestCases.Any() ? discoverySink.TestCases.Where(test => test.SkipReason == null).ToList() : new();
        }
    }
}