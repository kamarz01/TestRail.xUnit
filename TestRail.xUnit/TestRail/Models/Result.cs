using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class Result
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("test_id")]
    public ulong TestId { get; set; }

    [JsonProperty("case_id")]
    public string CaseId { get; set; }

    [JsonProperty("status_id")]
    public ulong? StatusId { get; set; }

    [JsonProperty("created_by")]
    public ulong? CreatedBy { get; set; }

    [JsonProperty("created_on")]
    private int? CreatedOnTimestamp
    {
        set => CreatedOn = value.HasValue ? new DateTime(1970, 1, 1).AddSeconds(value.Value) : null;
    }

    public DateTime? CreatedOn { get; private set; }

    [JsonProperty("assignedto_id")]
    public ulong? AssignedToId { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("elapsed")]
    public string Elapsed { get; set; }

    [JsonProperty("defects")]
    public string Defects { get; set; }
}