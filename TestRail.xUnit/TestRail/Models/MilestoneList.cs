using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models
{
    public class MilestoneList
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("_links")]
        public Link Links { get; set; }

        [JsonProperty("milestones")]
        public List<Milestone> Milestones { get; set; }
    }
}