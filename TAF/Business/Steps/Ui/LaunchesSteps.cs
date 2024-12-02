using OpenQA.Selenium;

namespace TAF.Business.Steps.Ui;

public class LaunchesSteps(IWebDriver driver) : BaseStep(driver)
{
    public void FilterLaunchesByRunName(string runName)
    {
        Waits.WaitForClickable(LaunchesPage.AddFilterButton);
        LaunchesPage.AddFilterButton.Click();
        LaunchesPage.FilterValueField.SendText(runName);
        Waits.WaitForElementsInvisible(LaunchesPage.LoadingSpinnerXpath);
    }

    public bool IsLaunchDisplayed()
    {
        var allLaunches = Waits.WaitForElements(LaunchesPage.RowLaunchesXpath);
        return allLaunches.Count > 0;
    }
}