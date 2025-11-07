using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class CreateTestCaseResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("section_id")]
    public int SectionId { get; set; }

    [JsonProperty("template_id")]
    public int? TemplateId { get; set; }

    [JsonProperty("type_id")]
    public int? TypeId { get; set; }

    [JsonProperty("priority_id")]
    public int? PriorityId { get; set; }

    [JsonProperty("milestone_id")]
    public int? MilestoneId { get; set; }

    [JsonProperty("refs")]
    public string Refs { get; set; }

    [JsonProperty("created_by")]
    public int CreatedBy { get; set; }

    [JsonProperty("created_on")]
    public long CreatedOn { get; set; }

    [JsonProperty("estimate")]
    public string Estimate { get; set; }

    [JsonProperty("estimate_forecast")]
    public string EstimateForecast { get; set; }

    [JsonProperty("updated_by")]
    public int UpdatedBy { get; set; }

    [JsonProperty("updated_on")]
    public long UpdatedOn { get; set; }
}