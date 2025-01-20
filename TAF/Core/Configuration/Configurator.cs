using System.Reflection;
using TAF.Core.Util;

namespace TAF.Core.Configuration;

public static class Configurator
{
    private static AppSettings? _appSettings;

    public static AppSettings ReadConfiguration()
    {
        if (_appSettings is not null) return _appSettings;
        _appSettings!.RunType = GetRunType();

        var appSettingPath = GetConfigurationPath();
        var appSettingsText = File.ReadAllText(appSettingPath);

        if (JsonUtil.IsValidJson<AppSettings>(appSettingsText))
            _appSettings = JsonUtil.DeserializeObject<AppSettings>(appSettingsText) ??
                           throw new FileNotFoundException();
        else
            throw new Exception("Invalid JSON format. Unable to deserialize the appsettings.json file.");

        if (!string.IsNullOrEmpty(_appSettings.Url)) return _appSettings;
        _appSettings.Url = Environment.GetEnvironmentVariable("URL")
                           ?? throw new Exception("URL is empty");
        _appSettings.UserEmail = Environment.GetEnvironmentVariable("EMAIL")
                                 ?? throw new Exception("Email is empty");
        _appSettings.UserPassword = Environment.GetEnvironmentVariable("PASSWORD")
                                    ?? throw new Exception("Password is empty");

        return _appSettings;
    }

    private static RunType GetRunType()
    {
        var runType = Environment.GetEnvironmentVariable("RUN_TYPE");
        if (!string.IsNullOrEmpty(runType) && runType.Equals("Remote"))
            return RunType.Remote;
        return RunType.Local;
    }

    private static string GetConfigurationPath()
    {
        return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Core", "Configuration",
            _appSettings?.RunType == RunType.Local ? "appsettings.Development.json" : "appsettings.json");
    }
}