using TAF.Business.Pages;
using TAF.Core.Configuration;
using NLog;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using TAF.Core.Driver;
using TAF.Core.Util;
using NUnit.Framework;

namespace TAF.Business.StepDefinitions
{
    [Binding]
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class FilterLaunchesStepDefinitions
    {
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private IWebDriver _driver;
        private LaunchesPage _launchesPage;
        private Waits _waits;

        [BeforeScenario]
        public void BeforeScenario()
        {
            _driver = new Browser().Driver;
            _launchesPage = new LaunchesPage(_driver);
            _waits = new Waits(_driver);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver?.Quit();  // Завершаем драйвер после каждого сценария
        }

        [Given(@"open login page")]
        public void GivenOpenLoginPage()
        {
            _ = new LoginPage(_driver, true, true);
        }

        [When(@"login with valid credentials")]
        public void WhenLoginWithValidCredentials()
        {
            Logger.Info("Logging with valid credentials...");
            var username = Configurator.ReadConfiguration().UserEmail;
            var password = Configurator.ReadConfiguration().UserPassword;
            new LoginStep(_driver).Login(username, password);
            _ = new LaunchesPage(_driver, true);
        }

        [Given(@"I have opened the launches page")]
        public void GivenIHaveOpenedTheLaunchesPage()
        {
            _ = new LaunchesPage(_driver, true, true);
        }

        [When(@"I filter launches by run name '([^']*)'")]
        public void WhenIFilterLaunchesByRunName(string runName)
        {
            _waits.WaitForClickable(_launchesPage.AddFilterButton);
            _launchesPage.AddFilterButton.Click();
            _launchesPage.FilterValueField.SendText(runName);
            _waits.WaitForElementsInvisible(_launchesPage.LoadingSpinnerXpath);
        }

        [Then(@"the launch should be '([^']*)'")]
        public void ThenTheLaunchShouldBe(string isShouldExist)
        {
            var allLaunches = _waits.WaitForElements(_launchesPage.RowLaunchesXpath);
            bool expected = bool.Parse(isShouldExist);
            Assert.That(allLaunches.Count > 0, Is.EqualTo(expected));
        }
    }
}