using NLog;
using OpenQA.Selenium;
using TAF.Core.Configuration;
using TAF.Core.NLogger;

namespace TAF.Core.Driver;

public class Browser
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Browser()
    {
        NLogConfig.Config();

        // Read configuration
        var browserType = Configurator.ReadConfiguration().Ui.BrowserType;
        var seleniumGridUrl = Configurator.ReadConfiguration().Ui.SeleniumGridUrl;

        // Initialize WebDriver
        Driver = GetWebDriver(browserType, seleniumGridUrl);

        // Set timeouts
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

        _logger.Info(
            $"Browser '{browserType}' started successfully in '{Configurator.ReadConfiguration().RunType}' mode!");
    }

    public IWebDriver Driver { get; }

    private IWebDriver GetWebDriver(string browserType, string seleniumGridUrl)
    {
        return Configurator.ReadConfiguration().RunType == RunType.Remote
            ?
            // Remote execution via Selenium Grid
            GetRemoteWebDriver(browserType, seleniumGridUrl)
            : GetLocalWebDriver(browserType);
    }

    private IWebDriver GetLocalWebDriver(string browserType)
    {
        var driverFactory = new DriverFactory();
        return browserType switch
        {
            "chrome" => driverFactory.GetLocalChromeDriver(),
            "firefox" => driverFactory.GetLocalFirefoxDriver(),

            _ => throw new Exception($"This browser type '{browserType}' is not supported for local execution")
        };
    }

    private IWebDriver GetRemoteWebDriver(string browserType, string seleniumGridUrl)
    {
        var driverFactory = new DriverFactory();

        return browserType switch
        {
            "chrome" => driverFactory.GetRemoteChromeDriver(seleniumGridUrl),
            "firefox" => driverFactory.GetRemoteFirefoxDriver(seleniumGridUrl),

            _ => throw new Exception($"This browser type '{browserType}' is not supported for remote execution")
        };
    }
}