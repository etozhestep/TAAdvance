using NLog;
using OpenQA.Selenium;
using TAF.Business.Pages;
using TAF.Core.Hooks;
using TAF.Core.Util;
using TechTalk.SpecFlow;

namespace TAF.Tests.StepDefinitions;

[Binding]
[TestFixture]
public class FilterLaunchesStepDefinitions
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private static IWebDriver Driver => Hooks.Driver.Value!;
    private static Waits? Waits => Hooks.Waits.Value;


    [Given(@"open login page")]
    public void GivenOpenLoginPage()
    {
        _ = new LoginPage(Driver, true, true);
    }

    [When(@"login with valid credentials")]
    public void WhenLoginWithValidCredentials()
    {
        _logger.Info("Logging with valid credentials...");
        // var username = Configurator.ReadConfiguration().UserEmail;
        // var password = Configurator.ReadConfiguration().UserPassword;
        var loginPage = new LoginPage(Driver, true, true);
        // loginPage.EmailField.SendText(username);
        // loginPage.PasswordField.SendText(password);
        loginPage.SignInButton.Click();
        _ = new LaunchesPage(Driver, true);
    }

    [Given(@"I have opened the launches page")]
    public void GivenIHaveOpenedTheLaunchesPage()
    {
        _ = new LaunchesPage(Driver, true, true);
    }

    [When(@"I filter launches by run name '([^']*)'")]
    public void WhenIFilterLaunchesByRunName(string runName)
    {
        try
        {
            var launchesPage = new LaunchesPage(Driver);
            Waits!.WaitForClickable(launchesPage.AddFilterButton);
            launchesPage.AddFilterButton.Click();
            launchesPage.FilterValueField.SendText(runName);
            Waits.WaitForElementsInvisible(launchesPage.LoadingSpinnerXpath);
        }
        catch (Exception e)
        {
            Hooks.Logger.Error(e);
            throw new NoSuchShadowRootException();
        }
    }

    [Then(@"the launch should be '([^']*)'")]
    public void ThenTheLaunchShouldBe(string isShouldExist)
    {
        try
        {
            var launchesPage = new LaunchesPage(Driver);

            var allLaunches = Waits?
                .WaitForElements(launchesPage.RowLaunchesXpath);
            var expected = bool.Parse(isShouldExist);
            Assert.That(allLaunches?.Count > 0, Is.EqualTo(expected),
                $"Expected: {expected}, Actual: {allLaunches?.Count > 0}");
        }
        catch (Exception e)
        {
            Hooks.Logger.Error(e);
            throw;
        }
    }
}