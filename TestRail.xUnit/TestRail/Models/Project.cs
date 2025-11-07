using Newtonsoft.Json;

namespace Zaghloul.QA.TestRail.xUnit.TestRail.Models;

public class Project
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("announcement")]
    public string Announcement { get; set; }

    [JsonProperty("show_announcement")]
    public bool? ShowAnnouncement { get; set; }

    [JsonProperty("is_completed")]
    public bool? IsCompleted { get; set; }

    [JsonProperty("completed_on")]
    private int? CompletedOnTimestamp
    {
        set => CompletedOn = value.HasValue ? new DateTime(1970, 1, 1).AddSeconds(value.Value) : null;
    }

    public DateTime? CompletedOn { get; private set; }

    [JsonProperty("default_role_id")]
    public object DefaultRoleId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("users")]
    public List<object> Users { get; set; }

    [JsonProperty("groups")]
    public List<object> Groups { get; set; }
}