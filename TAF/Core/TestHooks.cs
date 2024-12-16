using OpenQA.Selenium;
using TAF.Core.Driver;
using TechTalk.SpecFlow;

namespace TAF.Core;

[Binding]
public class TestHooks
{
    protected IWebDriver? Driver;

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        Driver = new Browser().Driver;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        Driver?.Quit();
    }
}