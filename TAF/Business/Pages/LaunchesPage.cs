using AF.Utils.Wrappers;
using OpenQA.Selenium;
using TAF.Business.WebElements;

namespace TAF.Business.Pages;

public class LaunchesPage(IWebDriver driver, bool evaluateStatus = false, bool openPageByUrl = false)
    : BasePage(driver, evaluateStatus, openPageByUrl)
{
    private const string Endpoint = "#default_personal/launches/all";
    private readonly By _addFilterButtonXpath = By.XPath("//*[contains(text(),'Add filter')]/parent::button");
    private readonly By _filterFieldXpath = By.XPath("//input[@placeholder='Enter name']");
    private readonly By _pageTitleXpath = By.XPath("//div[text()='All launches']");
    public readonly By RowLaunchesXpath = By.XPath("//td[@rowspan and contains(@class,'name')]");
    private Title PageTitle => new(Driver, _pageTitleXpath);
    public Button AddFilterButton => new(Driver, _addFilterButtonXpath);
    public Field FilterValueField => new(Driver, _filterFieldXpath);

    protected override bool EvaluateLoadedStatus()
    {
        try
        {
            return PageTitle is { IsDisplayed: true, Text: "All launches" } && Driver.Url.Contains(Endpoint);
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    protected override string GetEndpoint()
    {
        return Endpoint;
    }
}