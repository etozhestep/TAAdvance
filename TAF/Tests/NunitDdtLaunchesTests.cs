using NUnit.Framework;
using OpenQA.Selenium;
using TAF.Business.Steps.Ui;
using TAF.Core.Driver;
using TAF.Tests.TestData;

namespace TAF.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class NunitDdtLaunchesTests
{
    [SetUp]
    public void Login()
    {
        if (Driver.Value != null) new LoginSteps(Driver.Value).LoginWithValidCredentials();
    }

    [TearDown]
    public void Clean()
    {
        if (Driver.Value == null) return;
        Driver.Value.Quit();
        Driver.Value.Dispose();
        Driver.Value = null;
    }

    private static readonly ThreadLocal<IWebDriver?> Driver = new(() => new Browser().Driver);

    [Test]
    [TestCaseSource(typeof(NunitJsonFileData), nameof(NunitJsonFileData.GetTestData))]
    public void Nunit_FilterPageWithRunNameTest(string runName, bool isShouldExist)
    {
        var launchesSteps = new LaunchesSteps(Driver.Value!);

        launchesSteps.FilterLaunchesByRunName(runName);
        var actualIsDisplayed = launchesSteps.IsLaunchDisplayed();

        Assert.That(isShouldExist, Is.EqualTo(actualIsDisplayed));
    }
}