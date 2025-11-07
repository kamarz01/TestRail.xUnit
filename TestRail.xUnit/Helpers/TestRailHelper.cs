using System.Collections.Concurrent;
using System.Diagnostics;
using Zaghloul.QA.TestRail.xUnit.Config;
using Zaghloul.QA.TestRail.xUnit.Models;
using Zaghloul.QA.TestRail.xUnit.TestRail.Client;
using Zaghloul.QA.TestRail.xUnit.TestRail.Helpers;
using Zaghloul.QA.TestRail.xUnit.TestRail.Models;

namespace Zaghloul.QA.TestRail.xUnit.Helpers
{
    public class TestRailHelper
    {
        public TestRailClient TestRailClient;
        private ConcurrentDictionary<string, ulong?> _sectionCache = new ConcurrentDictionary<string, ulong?>();
        private ConcurrentDictionary<ulong, Task<List<Case>>> _casesCache = new ConcurrentDictionary<ulong, Task<List<Case>>>();
        private TestRailConfiguration _config;
        private ulong? _cachedMilestoneId;

        public TestRailHelper()
        {
            TestRailClient = new();
            _config = AppConfigHelper.GetAppConfigurations();
        }

        public async Task PublishResults(ConcurrentDictionary<string, TestInfo> _results)
        {
            Debug.WriteLine($"[xUnit-TestRail-Plugin] Result Publish Started.");

            ulong projectId = ulong.Parse(_config.ProjectId);
            var testsBySuite = _results.Values.GroupBy(x => x.SuiteName);

            var project = await TestRailClient.GetProject(projectId);

            if (project == null)
                return;

            var suiteTasks = testsBySuite.Select(async currentSuite =>
            {
                string suiteName = currentSuite.Key;

                // Ensure Suite Exists
                ulong suiteId = await EnsureSuiteExists(projectId, suiteName);

                // Ensure Sections (collect all section names for this suite)
                var hasValidDefault = !string.IsNullOrWhiteSpace(_config.DefaultSectionName);
                var sectionNames = currentSuite
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.SectionName) ||
                        string.IsNullOrWhiteSpace(x.SectionName) && hasValidDefault)
                    .Select(x => string.IsNullOrWhiteSpace(x.SectionName)
                        ? _config.DefaultSectionName
                        : x.SectionName)
                    .Distinct()
                    .ToList();

                // Ensure sections and store section IDs
                var sectionIdMap = new Dictionary<string, ulong>();
                foreach (var sectionName in sectionNames)
                {
                    ulong sectionId = await EnsureSectionExists(projectId, suiteId, sectionName);
                    sectionIdMap[sectionName] = sectionId;
                }

                // Ensure Cases (if needed)
                var testsWithNoCaseIds = currentSuite.Where(x => x.CaseIds == null || x.CaseIds.Count == 0).ToList();
                if (testsWithNoCaseIds.Any())
                {
                    await HandleCasesAndSections(projectId, suiteId, testsWithNoCaseIds, sectionIdMap, _results);
                }

                // Collect Results
                var results = new ConcurrentDictionary<string, TestInfo>(
                    _results.Where(kv => kv.Value.SuiteName == suiteName)
                );

                Debug.WriteLine($"[xUnit-TestRail-Plugin] Results Collected.");

                // If a milestone name is configured, get its ID (create it if it doesn't exist)
                var milestoneId = await GetCachedMilestoneId(_config.MilestoneName, projectId);

                // Create Run (and Milestone if exists)
                // Check if there's configured serviceName to be added to Run Name then use it while creating the test run.
                // Check if we want to include only the executed tests in the run

                var runId = _config.AddOnlyExecutedCasesToRun
                    ? await CreateRun(project, suiteId, milestoneId, _config.ServiceName, ExtractRunSpecificCaseIds(results), false)
                    : await CreateRun(project, suiteId, milestoneId, _config.ServiceName);

                // Push Results
                await PushTestResults(runId, results);

                // Close Run If needed
                if (_config.CloseRun)
                    await TestRailClient.CloseRun(runId);
            });

            await Task.WhenAll(suiteTasks);

            Debug.WriteLine($"[xUnit-TestRail-Plugin] Result Publish Finished.");
        }

        private async Task HandleCasesAndSections(ulong projectId, ulong suiteId, List<TestInfo> tests, Dictionary<string, ulong> sectionIdMap, ConcurrentDictionary<string, TestInfo> _results)
        {
            Debug.WriteLine($"[xUnit-TestRail-Plugin] Checking TestCases & Sections.");

            var defaultSectionName = _config.DefaultSectionName;

            foreach (var test in tests)
            {
                try
                {
                    var sectionName = string.IsNullOrWhiteSpace(test.SectionName)
                        ? !string.IsNullOrWhiteSpace(defaultSectionName) ? defaultSectionName : null
                        : test.SectionName;

                    if (string.IsNullOrWhiteSpace(sectionName) || !sectionIdMap.ContainsKey(sectionName))
                        continue;

                    // Get the section ID
                    ulong sectionId = sectionIdMap[sectionName];

                    // Get or Add cached cases for Suite/Section
                    var existingCases = await GetCasesCachedAsync(projectId, suiteId, sectionId);

                    // Handle Cases
                    string caseId = await EnsureTestCaseExists(sectionId, test.DisplayName, existingCases);

                    _results.AddOrUpdate(test.UniqueId, test, (key, existing) =>
                    {
                        existing.CaseIds = new List<string> { caseId };
                        return existing;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[xUnit-TestRail-Plugin] Failed to handle test '{test.DisplayName}': {ex.Message}");
                }
            }
        }

        private async Task<ulong?> GetCachedMilestoneId(string milestoneName, ulong projectId)
        {
            if (string.IsNullOrWhiteSpace(milestoneName))
                return null;

            if (_cachedMilestoneId == null)
            {
                _cachedMilestoneId = await TestRailClient.GetMilestoneIdOrCreate(projectId, milestoneName);
            }

            Debug.WriteLine($"[xUnit-TestRail-Plugin] Milestone with Id: {_cachedMilestoneId} found or created.");

            return _cachedMilestoneId;
        }

        private static HashSet<ulong> ExtractRunSpecificCaseIds(ConcurrentDictionary<string, TestInfo> results)
        {
            return results
                .SelectMany(x => x.Value.CaseIds)
                .Select(id => ulong.TryParse(id, out var parsed) ? (ulong?)parsed : null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToHashSet();
        }

        private async Task<ulong> EnsureSuiteExists(ulong projectId, string suiteName)
        {
            try
            {
                var suiteId = await TestRailClient.GetSuiteIdFromSuiteName(projectId, suiteName);
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Suite with Id: {suiteId} found.");
                return suiteId;
            }
            catch (InvalidDataException)
            {
                Debug.WriteLine("[xUnit-TestRail-Plugin] Suite doesn't exists, creating it.");

                var created = await TestRailClient.AddSuite(projectId, new() { Name = suiteName });
                return created.Id;
            }
        }

        private async Task<ulong> EnsureSectionExists(ulong projectId, ulong suiteId, string fullSectionName)
        {
            ulong? parentId = null;

            try
            {
                var sectionNames = fullSectionName.Split('/');

                foreach (var sectionName in sectionNames.Select(s => s.Trim()))
                {
                    var cacheKey = GetCacheKey(projectId, suiteId, sectionName);

                    if (!_sectionCache.TryGetValue(cacheKey, out ulong? sectionId))
                    {
                        sectionId = await GetSectionIdByNameAsync(projectId, suiteId, sectionName, parentId);

                        if (sectionId == null)
                        {
                            sectionId = await CreateSectionAsync(projectId, suiteId, sectionName, parentId);
                        }

                        _sectionCache[cacheKey] = sectionId.Value;
                    }

                    parentId = sectionId;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Something went wrong while checking Section: {ex}");
            }

            return parentId.Value; // Return the final (deepest) section ID
        }

        private async Task<string> EnsureTestCaseExists(ulong sectionId, string displayName, IEnumerable<Case> existingCases)
        {
            var existing = existingCases.FirstOrDefault(x => x.Title.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
                return existing.Id.ToString();

            var created = await TestRailClient.AddCase(sectionId, new CreateTestCaseRequest { Title = displayName });
            return created.Id.ToString();
        }

        private async Task<ulong> CreateRun(Project project, ulong suiteId, ulong? milestoneId, string serviceName, HashSet<ulong> caseIds = null, bool includeAll = true)
        {
            var runName = RunHelper.GenerateRunName(project.Name, serviceName);
            var runDescription = RunHelper.GenerateRunDescription(project.Name);
            var run = await TestRailClient.CreateRun(project.Id, suiteId, runName, runDescription, milestoneId, caseIds: caseIds, includeAll: includeAll);

            Debug.WriteLine($"[xUnit-TestRail-Plugin] Run with Id: {run?.Id} created.");

            return (ulong)(run?.Id);
        }

        private async Task PushTestResults(ulong runId, ConcurrentDictionary<string, TestInfo> testResults)
        {
            try
            {
                if (runId == 0)
                    return;

                var mappedResults = MapResultsToTestRail(testResults);
                var response = await TestRailClient.AddResultsForCases(runId, mappedResults);

                Debug.WriteLine($"[xUnit-TestRail-Plugin] Results Published.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Failed to publish results: {ex.Message}");
            }
        }

        private async Task<ulong?> GetSectionIdByNameAsync(ulong projectId, ulong suiteId, string name, ulong? parentId = null)
        {
            var sections = await TestRailClient.GetSections(projectId, suiteId);
            return sections.Sections
                .FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && s.ParentId.Equals(parentId))
                ?.Id;
        }

        private async Task<ulong> CreateSectionAsync(ulong projectId, ulong suiteId, string name, ulong? parentId = null)
        {
            var request = new Section
            {
                Name = name,
                SuiteId = suiteId,
                ParentId = parentId
            };

            var createdSection = await TestRailClient.AddSection(projectId, request);
            return createdSection.Id;
        }

        private string GetCacheKey(ulong projectId, ulong suiteId, string sectionName)
        {
            return $"{projectId}-{suiteId}-{sectionName}";
        }

        private BulkResults MapResultsToTestRail(ConcurrentDictionary<string, TestInfo> testResults)
        {
            var bulk = new BulkResults
            {
                Results = testResults.Values
                       .SelectMany(testInfo => testInfo.CaseIds.Select(caseId =>
                           new Result
                           {
                               CaseId = caseId,
                               StatusId = (ulong)testInfo.Status,
                               Elapsed = null,
                               Comment = $"Executed by xUnit TestRail Plugin v{AssemblyHelper.GetAssemblyVersion()}."
                           }))
                       .ToList()
            };

            return bulk;
        }

        private async Task<List<Case>> GetCasesCachedAsync(ulong projectId, ulong suiteId, ulong sectionId)
        {
            return await _casesCache.GetOrAdd(sectionId, async id =>
            {
                Debug.WriteLine($"[xUnit-TestRail-Plugin] Fetching cases for section {id}");
                var response = await TestRailClient.GetCasesBySuiteId(projectId, suiteId, id);
                return response ?? new List<Case>();
            });
        }
    }
}