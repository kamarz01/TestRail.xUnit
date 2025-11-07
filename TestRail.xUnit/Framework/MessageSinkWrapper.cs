using System.Collections.Concurrent;
using System.Diagnostics;
using Xunit.Abstractions;
using Zaghloul.QA.TestRail.xUnit.Helpers;
using Zaghloul.QA.TestRail.xUnit.Models;
using Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;

namespace Zaghloul.QA.TestRail.xUnit.Framework
{
    public class MessageSinkWrapper : IMessageSink
    {
        private readonly IMessageSink _executionMessageSink;
        private readonly ConcurrentDictionary<string, TestInfo> _results = new();
        private readonly ConcurrentDictionary<string, TestInfo> _metadataLookup;
        private TestRailConfiguration _config;
        private readonly TaskCompletionSource<bool> _publishingFinished = new();
        private readonly TestRailHelper _testRailHelper;
        private ulong? _cachedMilestoneId;

        public Task WaitForPublishingFinishedAsync() =>
            Task.WhenAny(_publishingFinished.Task, Task.Delay(TimeSpan.FromMinutes(60)));

        public MessageSinkWrapper(IMessageSink executionMessageSink, ConcurrentDictionary<string, TestInfo> metadataLookup, TestRailConfiguration configs)
        {
            _executionMessageSink = executionMessageSink;
            _metadataLookup = metadataLookup;
            _config = configs;
            _testRailHelper = new();
        }

        public bool OnMessage(IMessageSinkMessage message)
        {
            bool result = _executionMessageSink.OnMessage(message);

            if (_metadataLookup == null)
                return result;

            switch (message)
            {
                case ITestPassed passed:
                    AddResult(passed, "Passed");
                    break;
                case ITestFailed failed:
                    AddResult(failed, "Failed");
                    break;
                case ITestSkipped skipped:
                    AddResult(skipped, "Skipped");
                    break;
                case ITestAssemblyFinished:
                    _ = PublishResultsAsync();
                    break;
            }

            return result;
        }

        private void AddResult(ITestResultMessage message, string outcome)
        {
            if (!_metadataLookup.TryGetValue(message.Test.TestCase.UniqueID, out var meta))
                return;

            var status = TestResultMapper.GetTestStatus(outcome);
            meta.Status = status;
            meta.CaseIds = meta.CaseIds.Select(x => x.Replace("C", "", StringComparison.InvariantCultureIgnoreCase)).ToList();

            _results.AddOrUpdate(meta.UniqueId, meta, (key, existing) => meta);
        }

        private async Task PublishResultsAsync()
        {
            try
            {
                await _testRailHelper.PublishResults(_results);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Failed to publish results: {ex.Message}");
            }
            finally
            {
                _publishingFinished.TrySetResult(true);
            }
        }
    }
}