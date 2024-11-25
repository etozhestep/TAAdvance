using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace TAF.Core;

public class DriverFactory
{
    /// <summary>
    ///     Create a new instance of the Chrome driver with specific options.
    /// </summary>
    /// <returns></returns>
    public static IWebDriver GetChromeDriver()
    {
        var chromeOptions = CreateChromeOptions();
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

        return new ChromeDriver(chromeOptions);
    }

    private static ChromeOptions CreateChromeOptions()
    {
        var chromeOptions = new ChromeOptions();

        if (Configurator.GetRunType() == RunType.Remote) chromeOptions.AddArgument("--headless=new");
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
}