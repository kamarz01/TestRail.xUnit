using System.Diagnostics;
using Zaghloul.QA.TestRail.xUnit.TestRail.Enums;
using Zaghloul.QA.TestRail.xUnit.TestRail.Models;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Client
{
    public class TestRailClient : BaseClient
    {
        public async Task<Project> GetProject(ulong projectId)
        {
            var uri = CreateUri(RequestType.Get, RequestTarget.Project, projectId);
            return await Get<Project>(uri);
        }

        public async Task<List<Case>> GetCasesBySuiteId(ulong projectId, ulong? suiteId, ulong? sectionId)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> GetCasesBySuiteId");

            // Fix: Make Get Cases API Woking with Pagination
            var allCases = new List<Case>();
            int offset = 0;
            int limit = 250; // Default TestRail API limit (max 250 per docs)


            while (true)
            {
                var uri = $"?/api/v2/get_cases/{projectId}&suite_id={suiteId}&section_id={sectionId}&limit={limit}&offset={offset}";
                var response = await Get<CaseResponse>(uri);

                if (response?.Cases == null || response.Cases.Count == 0)
                    break;

                allCases.AddRange(response.Cases);

                // Stop if there’s no next page
                if (response.Links == null || string.IsNullOrEmpty(response.Links.Next))
                    break;

                // Otherwise, continue to next offset
                offset += limit;

                Debug.WriteLine($"[xUnit-TestRail-Plugin] Http Call -> GetCasesBySuiteId - Total: {response.Size} Offset: {offset} Limit: {limit}");
            }

            return allCases;
        }

        public async Task<ulong> GetSuiteIdFromSuiteName(ulong projectId, string suiteName)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> GetSuiteIdFromSuiteName");

            var uri = CreateUri(RequestType.Get, RequestTarget.Suites, projectId);
            var getSuiteApiResponse = await Get<SuiteList>(uri);
            var suite = getSuiteApiResponse?.Suites.FirstOrDefault(x => x.Name.Equals(suiteName, StringComparison.InvariantCultureIgnoreCase));
            return suite?.Id ?? throw new InvalidDataException($"Suite with name '{suiteName}' not found.");
        }

        public async Task<IList<Result>> AddResultsForCases(ulong runId, BulkResults results)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> AddResultsForCases");

            var uri = CreateUri(RequestType.Add, RequestTarget.ResultForCases, runId);
            return await Post<IList<Result>>(uri, results);
        }

        public async Task<Run> CreateRun(ulong projectId, ulong? suiteId, string name, string description, ulong? milestoneId, ulong? assignedToId = null, HashSet<ulong> caseIds = null, bool includeAll = true)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> CreateRun");

            var uri = CreateUri(RequestType.Add, RequestTarget.Run, projectId);
            var run = new Run
            {
                SuiteId = suiteId,
                Name = name,
                Description = description,
                MilestoneId = milestoneId,
                AssignedTo = assignedToId,
                IncludeAll = includeAll,
                CaseIds = caseIds
            };
            return await Post<Run>(uri, run);
        }

        public async Task CloseRun(ulong runId)
        {
            var uri = CreateUri(RequestType.Close, RequestTarget.Run, runId);
            await Post<Run>(uri);
            Debug.WriteLine($"[xUnit-TestRail-Plugin] Run Closed.");
        }

        public async Task<CreateTestCaseResponse> AddCase(ulong sectionId, CreateTestCaseRequest testCaseRequest)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> AddCase");

            var uri = $"?/api/v2/add_case/{sectionId}";
            return await Post<CreateTestCaseResponse>(uri, testCaseRequest);
        }

        public async Task<SectionList> GetSections(ulong projectId, ulong? suiteId = null, int? limit = null, int? offset = null)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> GetSections");

            var uri = $"?/api/v2/get_sections/{projectId}";
            if (suiteId.HasValue)
            {
                uri += $"&suite_id={suiteId.Value}";
            }
            if (limit.HasValue)
            {
                uri += $"&limit={limit.Value}";
            }
            if (offset.HasValue)
            {
                uri += $"&offset={offset.Value}";
            }
            return await Get<SectionList>(uri);
        }

        public async Task<Suite> AddSuite(ulong projectId, Suite suiteRequest)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> AddSuite");

            var uri = $"?/api/v2/add_suite/{projectId}";
            return await Post<Suite>(uri, suiteRequest);
        }

        public async Task<Section> AddSection(ulong projectId, Section sectionRequest)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> AddSection");

            var uri = $"?/api/v2/add_section/{projectId}";
            return await Post<Section>(uri, sectionRequest);
        }

        public async Task<ulong> GetMilestoneIdOrCreate(ulong projectId, string milestoneName, int isCompleted = 0)
        {
            var uri = $"?/api/v2/get_milestones/{projectId}&is_completed={isCompleted}";
            var milestoneList = await Get<MilestoneList>(uri);
            var milestone = milestoneList.Milestones.FirstOrDefault(x => x.Name.Equals(milestoneName, StringComparison.InvariantCultureIgnoreCase));

            return milestone != null
                ? milestone.Id
                : await CreateMilestone(projectId, milestoneName);
        }

        public async Task<ulong> CreateMilestone(ulong projectId, string milestoneName, string description = null, long? dueOn = null)
        {
            Debug.WriteLine("[xUnit-TestRail-Plugin] Http Call -> AddMilestone");

            var uri = $"?/api/v2/add_milestone/{projectId}";
            var newMilestone = new
            {
                name = milestoneName,
                description,
                due_on = dueOn
            };

            var createdMilestone = await Post<Milestone>(uri, newMilestone);
            return createdMilestone.Id;
        }
    }
}