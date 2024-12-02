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
        if (_driver.Value != null) new LoginSteps(_driver.Value).LoginWithValidCredentials();
    }

    [TearDown]
    public void Clean()
    {
        if (_driver.Value == null) return;
        _driver.Value.Quit();
        _driver.Value = null;
    }

    private readonly ThreadLocal<IWebDriver?> _driver = new(() => new Browser().Driver);

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