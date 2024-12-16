using TAF.Business.Pages;
using TAF.Core.Util;
using NLog;
using OpenQA.Selenium;

namespace TAF.Business.Steps.Ui
{
    /// <summary>
    /// Base class for all steps. Contains common methods and properties.
    /// </summary>
    public abstract class BaseStep
    {
        protected IWebDriver _driver;
        protected readonly LaunchesPage LaunchesPage;
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly LoginPage LoginPage;
        protected readonly Waits Waits;

        protected BaseStep(IWebDriver driver)
        {
            _driver = driver;
            LaunchesPage = new LaunchesPage(_driver);
            LoginPage = new LoginPage(_driver);
            Waits = new Waits(_driver);
        }
    }
}