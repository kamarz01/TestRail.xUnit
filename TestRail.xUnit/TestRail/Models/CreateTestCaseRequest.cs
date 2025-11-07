using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class CreateTestCaseRequest
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("template_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? TemplateId { get; set; }

    [JsonProperty("type_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? TypeId { get; set; }

    [JsonProperty("priority_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? PriorityId { get; set; }

    [JsonProperty("estimate", NullValueHandling = NullValueHandling.Ignore)]
    public string Estimate { get; set; }

    [JsonProperty("milestone_id", NullValueHandling = NullValueHandling.Ignore)]
    public int? MilestoneId { get; set; }

    [JsonProperty("refs", NullValueHandling = NullValueHandling.Ignore)]
    public string Refs { get; set; }

    [JsonProperty("custom_fields", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object> CustomFields { get; set; }
}