using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models
{
    public class Link
    {
        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("prev")]
        public string Prev { get; set; }
    }
}
