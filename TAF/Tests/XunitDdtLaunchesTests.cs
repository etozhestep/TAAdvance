using TAF.Core;
using TAF.Tests.TestData;
using Xunit;

namespace TAF.Tests;

public class XunitDdtLaunchesTests : BaseTest
{
    public XunitDdtLaunchesTests()
    {
        LoginSteps.LoginWithValidCredentials();
    }

    [Theory]
    [XunitJsonFileData("launchesTestData.json")]
    public void Launches_FilterPageWithRunNameTest(string launchName, bool isShouldExist)
    {
        LaunchesSteps.FilterLaunchesByRunName(launchName);
        var actualIsDisplayed = LaunchesSteps.IsLaunchDisplayed();

        Assert.Equal(actualIsDisplayed, isShouldExist);
    }
}