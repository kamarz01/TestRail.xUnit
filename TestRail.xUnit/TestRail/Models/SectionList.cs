using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class SectionList
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int Size { get; set; }
    public Links _Links { get; set; }
    public List<Section> Sections { get; set; }
}

public class Links
{
    public string Next { get; set; }
    public string Prev { get; set; }
}

public class Section
{
    public int Depth { get; set; }

    public int DisplayOrder { get; set; }

    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonProperty("suite_id")]
    public ulong SuiteId { get; set; }
}