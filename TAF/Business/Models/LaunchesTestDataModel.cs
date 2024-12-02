using Newtonsoft.Json;

namespace TAF.Business.Models;

public class LaunchesTestDataModel
{
    [JsonProperty("run_name")] public string? RunName { get; set; }
    [JsonProperty("is_exist")] public bool IsRunNameExist { get; set; }
}