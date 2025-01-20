using TAF.Business.Pages;
using TAF.Core;

namespace TAF.Tests;

public class LaunchesTests : BaseTest
{
    [SetUp]
    public void BeforeTest()
    {
        LoginSteps.LoginWithValidCredentials();
    }

    [Test]
    public void Launches_DefaultSorting_IsSortedByMostRecent()
    {
        var launchesPage = new LaunchesPage(Driver);
        const string expectedSortedColumnName = "start time";
        const string expectedSortingDirection = "desc";


        var actualSortedColumnName = launchesPage.LaunchesGrid.SortedByName();
        var actualSortingDirection = launchesPage.LaunchesGrid.SortedByDirection();

        Assert.Multiple(() =>
        {
            Assert.That(actualSortingDirection, Is.EqualTo(expectedSortingDirection));
            Assert.That(actualSortedColumnName, Is.EqualTo(expectedSortedColumnName));
        });
    }

    [Test]
    public void Launches_ChangeSorting_ListIsSortedAccordingly()
    {
        var launchesPage = new LaunchesPage(Driver);
        const string expectedSortedColumnName = "failed";
        const string expectedSortingDirection = "asc";

        launchesPage.LaunchesGrid.SortByColumn(expectedSortedColumnName, expectedSortingDirection);
        var actualSortedColumnName = launchesPage.LaunchesGrid.SortedByName();
        var actualSortingDirection = launchesPage.LaunchesGrid.SortedByDirection();

        Assert.Multiple(() =>
        {
            Assert.That(actualSortingDirection, Is.EqualTo(expectedSortingDirection));
            Assert.That(actualSortedColumnName, Is.EqualTo(expectedSortedColumnName));
        });
    }

    [Test]
    public void Launches_GetTestCountsByName_ContainsCorrectCounts()
    {
        const string launchName = "Demo Api Tests#3";
        const int expectedTotalTests = 20;
        const int expectedPassedTests = 10;
        const int expectedFailedTests = 8;
        const int expectedSkippedTests = 2;

        var launch = LaunchesSteps.GetLaunchByName(launchName);
        var actualTotalTests = launch.GetTotalTestsCount();
        var actualPassedTests = launch.GetPassedTestsCount();
        var actualFailedTests = launch.GetFailedTestsCount();
        var actualSkippedTests = launch.GetSkippedTestsCount();

        Assert.Multiple(() =>
        {
            Assert.That(actualTotalTests, Is.EqualTo(expectedTotalTests),
                "Total tests count does not match expected value.");
            Assert.That(actualPassedTests, Is.EqualTo(expectedPassedTests),
                "Passed tests count does not match expected value.");
            Assert.That(actualFailedTests, Is.EqualTo(expectedFailedTests),
                "Failed tests count does not match expected value.");
            Assert.That(actualSkippedTests, Is.EqualTo(expectedSkippedTests),
                "Skipped tests count does not match expected value.");
        });
    }

    [Test]
    public void Launches_SelectAndCompare_MultipleLaunchesCanBeCompared()
    {
        var launchesPage = new LaunchesPage(Driver);
        const string firstLaunchName = "Demo Api Tests#3";
        const string secondLaunchName = "Demo Api Tests#5";

        LaunchesSteps.SelectLaunchByName(firstLaunchName);
        LaunchesSteps.SelectLaunchByName(secondLaunchName);

        var actualSelectedLaunches = LaunchesSteps.GetSelectedLaunchesName().ToArray();

        LaunchesSteps.CompareSelectedLaunches();
        Assert.Multiple(() =>
        {
            Assert.That(launchesPage.Popup.Displayed);
            Assert.That(actualSelectedLaunches,
                Does.Contain(firstLaunchName.ToLower().Replace(" ", "")));
            Assert.That(actualSelectedLaunches,
                Does.Contain(secondLaunchName.ToLower().Replace(" ", "")));
        });
    }

    [Test]
    public void Launches_RemoveLaunchByName_LaunchIsRemoved()
    {
        const string launchName = "Demo Api Tests#7";

        LaunchesSteps.RemoveLaunchByName(launchName);

        Assert.That(!LaunchesSteps.IsLaunchPresented(launchName));
    }

    [Test]
    public void Launches_ReorderLaunches_OrderIsUpdated()
    {
        const string mainLaunch = "Demo Api Tests#6";
        const string targetLaunch = "Demo Api Tests#9";

        LaunchesSteps.MoveLaunchBeforeAnother(mainLaunch, targetLaunch);
        var isMainLaunchBeforeTargetLaunch =
            LaunchesSteps.IsLaunchBeforeAnother(mainLaunch, targetLaunch);

        Assert.That(isMainLaunchBeforeTargetLaunch);
    }
}