using AF.Utils.Wrappers;
using OpenQA.Selenium;

namespace TAF.Business.Pages;

public class LaunchesPage(IWebDriver driver, bool evaluateStatus = false, bool openPageByUrl = false)
    : BasePage(driver, evaluateStatus, openPageByUrl)
{
    private const string Endpoint = "/ui/#default_personal/launches/all";
    private readonly By _pageTitleXpath = By.XPath("//div[text()='All launches']");

    public Title PageTitle => new(Driver, _pageTitleXpath);

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