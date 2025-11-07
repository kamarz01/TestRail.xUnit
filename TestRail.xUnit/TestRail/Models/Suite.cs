using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class Suite
{
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    public string Description { get; set; }
}