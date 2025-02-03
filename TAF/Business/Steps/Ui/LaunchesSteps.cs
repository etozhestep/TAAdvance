using OpenQA.Selenium;
using TAF.Business.Pages;
using TAF.Business.WebElements;

namespace TAF.Business.Steps.Ui;

public class LaunchesSteps(IWebDriver driver) : BaseStep(driver)
{
    public IEnumerable<Launch> GetRecentLaunches(int count)
    {
        return LaunchesPage.LaunchesGrid.GetAllLaunches().Take(count);
    }

    public Launch GetLaunchByName(string launchName)
    {
        var launches = LaunchesPage.LaunchesGrid.GetAllLaunches();
        var launch = launches.FirstOrDefault(l => l.Text.Contains(launchName, StringComparison.OrdinalIgnoreCase));
        if (launch == null) throw new Exception($"Launch with name '{launchName}' not found.");
        return launch;
    }

    public void SelectLaunchByName(string launchName)
    {
        var launches = LaunchesPage.LaunchesGrid.GetAllLaunches();
        var launch = launches.FirstOrDefault(l => l.Text.Contains(launchName, StringComparison.OrdinalIgnoreCase));
        if (launch == null) throw new Exception($"Launch with name '{launchName}' not found.");
        launch.SelectLaunch();
    }

    public IEnumerable<string> GetSelectedLaunchesName()
    {
        return Waits.WaitForElementsPresence(LaunchesPage.SelectedItemsName)
            .Select(element => element.Text.ToLower().Replace(" ", ""))
            .Where(text => text != string.Empty);
    }

    public void CompareSelectedLaunches()
    {
        LaunchesPage.ActionsDropdown.SelectByValue("Compare");
    }

    public void RemoveLaunchByName(string launchName)
    {
        SelectLaunchByName(launchName);
        LaunchesPage.ActionsDropdown.SelectByValue("Delete");
        LaunchesPage.Popup.DeleteButton.Click();
        LaunchesPage.RefreshButton.Click();
        _ = new LaunchesPage(Driver, true);
    }

    public bool IsLaunchPresented(string launchName)
    {
        var launches = LaunchesPage.LaunchesGrid.GetAllLaunches();
        return launches.Any(l => l.Text.Contains(launchName, StringComparison.OrdinalIgnoreCase));
    }

    public void MoveLaunchBeforeAnother(string mainLaunch, string targetLaunch)
    {
        var launches = LaunchesPage.LaunchesGrid.GetAllLaunches().ToArray();
        var launch = launches.FirstOrDefault(l => l.Text.Contains(mainLaunch, StringComparison.OrdinalIgnoreCase));
        if (launch == null) throw new Exception($"Launch with name '{mainLaunch}' not found.");
        var moveToLaunch =
            launches.FirstOrDefault(l => l.Text.Contains(targetLaunch, StringComparison.OrdinalIgnoreCase));
        if (moveToLaunch == null) throw new Exception($"Launch with name '{targetLaunch}' not found.");
        launch.LaunchNameButton.DragAndDrop(moveToLaunch.LaunchNameButton);
    }

    public bool IsLaunchBeforeAnother(string firstLaunchName, string secondLaunchName)
    {
        var launchList = LaunchesPage.LaunchesGrid.GetAllLaunches().ToList();

        var firstLaunchPosition = launchList.FindIndex(launch =>
            launch.Text.Contains(firstLaunchName, StringComparison.OrdinalIgnoreCase));
        var secondLaunchPosition = launchList.FindIndex(launch =>
            launch.Text.Contains(secondLaunchName, StringComparison.OrdinalIgnoreCase));

        if (firstLaunchPosition == -1) throw new Exception($"Launch '{firstLaunchName}' not found in the list.");

        if (secondLaunchPosition == -1) throw new Exception($"Launch '{secondLaunchName}' not found in the list.");

        return firstLaunchPosition < secondLaunchPosition;
    }
}