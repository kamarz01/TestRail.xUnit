using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;
using Zaghloul.QA.TestRail.xUnit.Helpers;

namespace Zaghloul.QA.TestRail.xUnit.Framework
{
    public class TestRailxUnitFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public TestRailxUnitFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink) { }

        protected override void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            RunTestCasesAsync(testCases, executionMessageSink, executionOptions).GetAwaiter().GetResult();
        }

        private async Task RunTestCasesAsync(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            var initResult = await Init.Check();

            if (!initResult.IsValid)
            {
                base.RunTestCases(testCases, executionMessageSink, executionOptions);
                return;
            }

            var metadata = new xUnitFrameworkHelper().ExtractMetadata(testCases);
            var wrappedSink = new MessageSinkWrapper(executionMessageSink, metadata, initResult.Configuration);

            base.RunTestCases(testCases, wrappedSink, executionOptions);

            if (wrappedSink is MessageSinkWrapper sinkWrapper)
                await sinkWrapper.WaitForPublishingFinishedAsync();
        }
    }
}