using NUnit.Framework;
using TAF.Business.Steps.Ui;
using TAF.Core.Driver;
using TAF.Tests.TestData;

namespace TAF.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NunitDdtLaunchesTests
{
    [Test]
    [TestCaseSource(typeof(NunitJsonFileData), nameof(NunitJsonFileData.GetTestData))]
    public void Nunit_FilterPageWithRunNameTest(string runName, bool isShouldExist)
    {
        var driver = new Browser().Driver;
        new LoginSteps(driver).LoginWithValidCredentials();
        var launchesSteps = new LaunchesSteps(driver);

        launchesSteps.FilterLaunchesByRunName(runName);
        var actualIsDisplayed = launchesSteps.IsLaunchDisplayed();

        Assert.That(isShouldExist, Is.EqualTo(actualIsDisplayed));
    }
}