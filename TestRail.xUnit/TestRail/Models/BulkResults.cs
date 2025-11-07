using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class BulkResults
{
    [JsonProperty("results")]
    public IList<Result> Results { get; set; }
}