using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Zaghloul.QA.TestRail.xUnit.Framework
{
    public class TestRailxUnitFramework : XunitTestFramework
    {
        public TestRailxUnitFramework(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
        }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestRailxUnitFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
        }
    }
}