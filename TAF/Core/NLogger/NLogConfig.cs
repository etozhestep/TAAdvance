using NLog;
using NLog.Config;
using NLog.Targets;

namespace TAF.Core.NLogger;

public static class NLogConfig
{
    public static void Config()
    {
        var config = new LoggingConfiguration();

        //targets
        var errorLogFile = new FileTarget("errorLogfile")
        {
            FileName = "ErrorLogFile.txt",
            Layout = "${date} * ${level} * ${message}",
            KeepFileOpen = false
        };
        var infoLogFile = new FileTarget("infoLogfile")
        {
            FileName = "InfoLogFile.txt",
            Layout = "${longdate} * ${level} * ${message}",
            KeepFileOpen = false
        };

        var console = new ConsoleTarget("logconsole")
        {
            Layout = "${date} * ${level} * ${message}"
        };

        //rules
        config.AddRule(LogLevel.Error, LogLevel.Error, errorLogFile);
        config.AddRule(LogLevel.Trace, LogLevel.Info, infoLogFile);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, console);

        //Apply config
        LogManager.Configuration = config;
    }
}