using NLog;
using OpenQA.Selenium;
using TAF.Core.Driver;
using TAF.Core.Util;
using TechTalk.SpecFlow;

namespace TAF.Core.Hooks;

[Binding]
public class Hooks
{
    public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static ThreadLocal<IWebDriver?> Driver = new();
    public static ThreadLocal<Waits?> Waits = new();

    [BeforeScenario]
    public static void BeforeScenario()
    {
        Logger.Info("Initialising WebDriver for scenario...");
        Driver.Value = new Browser().Driver!;
        Waits.Value = new Waits(Driver.Value);
    }

    [AfterScenario]
    public static void AfterScenario()
    {
        try
        {
            Logger.Info("Tearing down WebDriver for scenario...");
            Driver.Value?.Quit();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during WebDriver tear down.");
        }
        finally
        {
            Driver = null!;
            Waits = null!;
        }
    }
}