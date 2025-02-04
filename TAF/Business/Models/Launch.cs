using Newtonsoft.Json;

namespace TAF.Business.Models;

public class Launch
{
    [JsonProperty("startTime")] public string? StartTime { get; set; }
    [JsonProperty("endTime")] public string? EndTime { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("uuid")] public string? Uuid { get; set; }
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("description")] public string? Description { get; set; }
    [JsonProperty("mode")] public string Mode { get; set; } = "DEFAULT";
    [JsonProperty("rerun")] public bool Rerun { get; set; } = true;
    [JsonProperty("rerunOf")] public string? RerunOf { get; set; }
    [JsonProperty("status")] public string? Status { get; set; }
    [JsonProperty("attributes")] public Attribute[]? Attributes { get; set; }
}

public class Attribute
{
    [JsonProperty("key")] public string? Key { get; set; }
    [JsonProperty("value")] public string? Value { get; set; }
    [JsonProperty("system")] public bool System { get; set; }
}

public class RootLaunches(List<Launch> launches)
{
    [JsonProperty("Content")] public List<Launch> Launches { get; set; } = launches;
}