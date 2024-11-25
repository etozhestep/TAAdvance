using Newtonsoft.Json;

namespace TAF.Core;

public class AppSettings(
    Ui ui,
    Api api,
    string url,
    string userEmail,
    string userPassword)
{
    [JsonProperty("UI")] public Ui Ui { get; set; } = ui;
    [JsonProperty("API")] public Api Api { get; set; } = api;
    [JsonProperty("Email")] public string UserEmail { get; set; } = userEmail;
    [JsonProperty("Password")] public string UserPassword { get; set; } = userPassword;
    public string Url { get; set; } = url;
}

public class Ui(string? browserType, double timeOut)
{
    public string? BrowserType { get; set; } = browserType;

    public double TimeOut { get; set; } = timeOut;
}

public class Api(string token)
{
    public string Token { get; set; } = token;
}

public enum RunType
{
    Local,
    Remote
}