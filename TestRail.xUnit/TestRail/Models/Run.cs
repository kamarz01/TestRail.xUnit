using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class Run
{
    [JsonProperty("id")]
    public ulong? Id { get; private set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("suite_id")]
    public ulong? SuiteId { get; set; }

    [JsonProperty("milestone_id")]
    public ulong? MilestoneId { get; set; }

    [JsonProperty("config")]
    public string Config { get; private set; }

    [JsonProperty("is_completed")]
    public bool? IsCompleted { get; private set; }

    [JsonProperty("completed_on")]
    private int? CompletedOnTimestamp
    {
        set => CompletedOn = value.HasValue ? new DateTime(1970, 1, 1).AddSeconds(value.Value) : null;
    }

    public DateTime? CompletedOn { get; private set; }

    [JsonProperty("created_on")]
    private int? CreatedOnTimestamp
    {
        set => CreatedOn = value.HasValue ? new DateTime(1970, 1, 1).AddSeconds(value.Value) : null;
    }

    public DateTime? CreatedOn { get; private set; }

    [JsonProperty("passed_count")]
    public uint? PassedCount { get; private set; }

    [JsonProperty("blocked_count")]
    public uint? BlockedCount { get; private set; }

    [JsonProperty("untested_count")]
    public uint? UntestedCount { get; private set; }

    [JsonProperty("retest_count")]
    public uint? RetestCount { get; private set; }

    [JsonProperty("failed_count")]
    public uint? FailedCount { get; private set; }

    [JsonProperty("project_id")]
    public ulong? ProjectId { get; private set; }

    [JsonProperty("plan_id")]
    public ulong? PlanId { get; private set; }

    [JsonProperty("assignedto_id")]
    public ulong? AssignedTo { get; set; }

    [JsonProperty("include_all")]
    public bool IncludeAll { get; set; }

    [JsonProperty("url")]
    public string Url { get; private set; }

    [JsonProperty("case_ids")]
    public HashSet<ulong> CaseIds { get; set; }

    [JsonProperty("config_ids")]
    public List<ulong> ConfigIds { get; set; }
}