using NLog;
using OpenQA.Selenium;
using TAF.Business.Pages;
using TAF.Core.Util;

namespace TAF.Business.Steps.Ui;

/// <summary>
///     Base class for all steps. Contains common methods and properties.
/// </summary>
/// <param name="driver"></param>
public abstract class BaseStep(IWebDriver driver)
{
    protected readonly LaunchesPage LaunchesPage = new(driver);
    protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

    protected readonly LoginPage LoginPage = new(driver);
    protected readonly Waits Waits = new(driver);

    protected IWebDriver Driver => driver;
}