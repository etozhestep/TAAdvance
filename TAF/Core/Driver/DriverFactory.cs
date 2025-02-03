using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using TAF.Core.Configuration;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace TAF.Core.Driver;

public class DriverFactory
{
    public IWebDriver GetLocalChromeDriver()
    {
        var chromeOptions = CreateChromeOptions();
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        return new ChromeDriver(chromeOptions);
    }

    public IWebDriver GetLocalFirefoxDriver()
    {
        var firefoxOptions = CreateFirefoxOptions();
        new DriverManager().SetUpDriver(new FirefoxConfig(), VersionResolveStrategy.MatchingBrowser);
        return new FirefoxDriver(firefoxOptions);
    }

    public IWebDriver GetRemoteChromeDriver(string seleniumGridUrl)
    {
        var chromeOptions = CreateChromeOptions();
        var remoteAddress = new Uri(seleniumGridUrl);
        return new RemoteWebDriver(remoteAddress, chromeOptions.ToCapabilities(), TimeSpan.FromSeconds(120));
    }

    public IWebDriver GetRemoteFirefoxDriver(string seleniumGridUrl)
    {
        var firefoxOptions = CreateFirefoxOptions();
        var remoteAddress = new Uri(seleniumGridUrl);
        return new RemoteWebDriver(remoteAddress, firefoxOptions.ToCapabilities(), TimeSpan.FromSeconds(120));
    }

    private static ChromeOptions CreateChromeOptions()
    {
        var chromeOptions = new ChromeOptions();
        if (Configurator.ReadConfiguration().RunType == RunType.Remote)
            chromeOptions.AddArgument("--headless");
        chromeOptions.AddArgument("--incognito");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddAdditionalOption("useAutomationExtension", false);
        chromeOptions.AddArgument("--start-maximized");
        chromeOptions.AddArgument("--disable-notifications");
        chromeOptions.AddArgument("--disable-popup-blocking");
        chromeOptions.AddArgument("--disable-infobars");
        chromeOptions.AddArgument("--disable-extensions");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArgument("--no-default-browser-check");
        chromeOptions.AddArgument("--no-first-run");
        chromeOptions.AddArgument("--ignore-certificate-errors");
        return chromeOptions;
    }

    private static FirefoxOptions CreateFirefoxOptions()
    {
        var firefoxOptions = new FirefoxOptions();
        if (Configurator.ReadConfiguration().RunType == RunType.Remote)
            firefoxOptions.AddArgument("--headless");
        firefoxOptions.AddArgument("-private");
        return firefoxOptions;
    }
}