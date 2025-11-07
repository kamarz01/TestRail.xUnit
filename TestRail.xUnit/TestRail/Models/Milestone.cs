using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models
{
    public class Milestone
    {
        [JsonProperty("completed_on")]
        public DateTime? CompletedOn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("due_on")]
        public DateTime? DueOn { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("is_completed")]
        public bool? IsCompleted { get; set; }

        [JsonProperty("is_started")]
        public bool? isStarted { get; set; }

        [JsonProperty("milestones")]
        public List<object> Milestones { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parent_id")]
        public ulong? ParentId { get; set; }

        [JsonProperty("project_id")]
        public ulong ProjectId { get; set; }

        [JsonProperty("refs")]
        public string Refs { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("start_on")]
        public DateTime? StartOn { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("started_on")]
        public DateTime? StartedOn { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}