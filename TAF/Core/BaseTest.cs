using System.Reflection;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NLog;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using TAF.Core.Driver;
using TAF.Core.Util;

namespace TAF.Core;

[Parallelizable(ParallelScope.All)]
[Author("ASciapaniuk")]
public class BaseTest
{
    protected readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    protected IWebDriver? Driver;
    protected Waits Waits;

    [SetUp]
    [AllureBefore("Start the browser")]
    public void Setup()
    {
        Logger.Info($"Executing {TestContext.CurrentContext.Test.Name}");

        // Create a new instance of the browser and waits class
        Driver = new Browser().Driver;
        Waits = new Waits(Driver);
    }

    /// <summary>
    ///     Quite and dispose the browser after each test class.
    ///     If the test failed, make a screenshot and attach it to the report with logs.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status is TestStatus.Passed)
            Logger.Info("Test passed!");

        try
        {
            // Get the path to the log files
            var infoLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "InfoLogFile.txt");
            var errorLogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "ErrorLogFile.txt");

            // Upload logs
            if (File.Exists(infoLogFile))
                AllureApi.AddAttachment("InfoLogFile", "text/plain", infoLogFile);
            if (File.Exists(errorLogFile))
                AllureApi.AddAttachment("ErrorLogFile", "text/plain", errorLogFile);

            // Verify if the test failed
            if (TestContext.CurrentContext.Result.Outcome.Status is not TestStatus.Failed) return;
            Logger.Info("Test failed.");
            if (Driver is null) return;
            Logger.Info("Making screenshot...");
            var screenshotByte = MakeScreenshot();
            AllureApi.AddAttachment("screenshot", "image/png", screenshotByte);
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