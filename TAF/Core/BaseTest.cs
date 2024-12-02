using System.Reflection;
using NLog;
using OpenQA.Selenium;
using TAF.Business.Steps.Ui;
using TAF.Core.Driver;
using TAF.Core.Util;

namespace TAF.Core;

public class BaseTest : IDisposable
{
    protected readonly LaunchesSteps LaunchesSteps;
    protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
    protected readonly LoginSteps LoginSteps;
    protected IWebDriver? Driver;
    protected Waits Waits;

    protected BaseTest()
    {
        // Create a new instance of the browser and waits class
        Driver = new Browser().Driver;
        Waits = new Waits(Driver);
        LoginSteps = new LoginSteps(Driver);
        LaunchesSteps = new LaunchesSteps(Driver);
    }

    /// <summary>
    ///     Quite and dispose the browser after each test class.
    ///     If the test failed, make a screenshot and attach it to the report with logs.
    /// </summary>
    public void Dispose()
    {
        try
        {
            // Get the path to the log files
            var infoLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "InfoLogFile.txt");
        }
        catch (Exception)
        {
            var errorLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "ErrorLogFile.txt");

            // Verify if the test failed
            Logger.Info("Test failed.");
            if (Driver is null) return;
            Logger.Info("Making screenshot...");
            var screenshotByte = MakeScreenshot();
        }
        finally
        {
            Logger.Info("Quiting the browser...");
            Driver?.Quit();
        }
    }

    private byte[] MakeScreenshot()
    {
        var screenshot = ((ITakesScreenshot)Driver!).GetScreenshot();
        return screenshot.AsByteArray;
    }
}