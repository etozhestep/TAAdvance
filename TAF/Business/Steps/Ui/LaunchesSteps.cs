using OpenQA.Selenium;
using TAF.Business.Pages;
using TechTalk.SpecFlow;

namespace TAF.Business.Steps.Ui;

[Binding]
public class LaunchesSteps(IWebDriver driver) : BaseStep(driver)
{
    [Given(@"I have opened the launches page")]
    public void OpenLaunchesPage()
    {
        _ = new LaunchesPage(Driver, true, true);
    }

    [When(@"I filter launches by run name ""<run_name>""")]
    public void FilterLaunchesByRunName(string runName)
    {
        Waits.WaitForClickable(LaunchesPage.AddFilterButton);
        LaunchesPage.AddFilterButton.Click();
        LaunchesPage.FilterValueField.SendText(runName);
        Waits.WaitForElementsInvisible(LaunchesPage.LoadingSpinnerXpath);
    }

    [Then(@"the launch should be ""<is_exist>""")]
    public void IsLaunchDisplayed()
    {
        var allLaunches = Waits.WaitForElements(LaunchesPage.RowLaunchesXpath);
        Assert.That(allLaunches, Is.Not.Empty);
    }
}