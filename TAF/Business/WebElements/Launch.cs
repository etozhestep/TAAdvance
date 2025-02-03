using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Launch(IWebDriver driver, IWebElement element) : UiElement(driver, element)
{
    private readonly By _failedTestsLocator = By.XPath(".//div[contains(@class,'launchSuiteGrid__failed')]//a");

    private readonly By _launchNameXpath = By.XPath(".//*[contains(@class,'launchSuiteGrid__name-col')]//a");
    private readonly By _passedTestsLocator = By.XPath(".//div[contains(@class,'launchSuiteGrid__passed')]//a");

    private readonly By _selectCheckboxXpath = By.XPath(".//div[contains(@class,'checkIcon__square')]");
    private readonly By _skippedTestsLocator = By.XPath(".//div[contains(@class,'launchSuiteGrid__skipped')]//a");

    // Locators for test count data within a launch element
    private readonly By _totalTestsLocator = By.XPath(".//div[contains(@class,'launchSuiteGrid__total')]//a");
    private readonly UiElement _uiElement = new(driver, element);

    public Button LaunchNameButton => new(Driver, _launchNameXpath);

    // Methods to get test counts
    public int GetTotalTestsCount()
    {
        return GetTestCount(_totalTestsLocator);
    }

    public int GetPassedTestsCount()
    {
        return GetTestCount(_passedTestsLocator);
    }

    public int GetFailedTestsCount()
    {
        return GetTestCount(_failedTestsLocator);
    }

    public int GetSkippedTestsCount()
    {
        return GetTestCount(_skippedTestsLocator);
    }

    private int GetTestCount(By locator)
    {
        try
        {
            var countElement = _uiElement.FindElement(locator);
            var countText = countElement.Text;
            if (int.TryParse(countText, out var count)) return count;

            Logger.Warn($"Unable to parse test count '{countText}' for locator '{locator}'.");
            return -1;
        }
        catch (NoSuchElementException)
        {
            Logger.Warn($"Test count element not found for locator '{locator}'.");
            return -1;
        }
    }

    public void SelectLaunch()
    {
        var checkbox = _uiElement.FindElement(_selectCheckboxXpath);
        if (!checkbox.GetAttribute("class")!.Contains("checked"))
            checkbox.Click();
    }
}