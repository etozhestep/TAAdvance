using NLog;
using OpenQA.Selenium;
using TAF.Core.Configuration;
using TAF.Core.Logger;

namespace TAF.Core.Driver;

public class Browser
{
    private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();

    public Browser()
    {
        NLogConfig.Config();
        var browserType = Configurator.ReadConfiguration().Ui.BrowserType?.ToLower();

        Driver = browserType?.ToLower() switch
        {
            "chrome" => DriverFactory.GetChromeDriver(),
            _ => throw new Exception("This browser type didn't find")
        };

        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
        _logger.Info($"Browser {browserType} started successfully!");
    }

    public IWebDriver Driver { get; }
}