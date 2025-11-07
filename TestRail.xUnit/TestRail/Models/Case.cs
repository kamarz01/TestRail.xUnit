using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class CaseResponse
{
    [JsonProperty("offset")]
    public int Offset { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("_links")]
    public Link Links { get; set; }

    [JsonProperty("cases")]
    public List<Case> Cases { get; set; }
}

public class Case
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
}