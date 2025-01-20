using OpenQA.Selenium;
using TAF.Business.WebElements;

namespace TAF.Business.Pages;

public class LaunchesPage(IWebDriver driver, bool evaluateStatus = false, bool openPageByUrl = false)
    : BasePage(driver, evaluateStatus, openPageByUrl)
{
    private const string Endpoint = "default_personal/launches/all";

    private readonly By _actionsDropDownXpath =
        By.XPath("//*[contains(text(),'Actions')]/ancestor::div[contains(@class,'ghostMenuButton')]");

    private readonly By _launchesGridXpath = By.XPath("//div[contains(@class,'grid')]");
    private readonly By _pageTitleXpath = By.XPath("//div[text()='All launches']");
    private readonly By _popup = By.XPath("//div[contains(@class,'modalLayout__modal-window')]");

    private readonly By _refreshButtonXpath = By.XPath("//*[contains(text(),'Refresh')]/ancestor::button");
    public readonly By SelectedItemsName = By.XPath("//span[contains(@class,'selectedItem__name')]");
    public Grid LaunchesGrid => new(Driver, _launchesGridXpath);

    public Button RefreshButton => new(Driver, _refreshButtonXpath);
    public Popup Popup => new(Driver, _popup);
    public Title PageTitle => new(Driver, _pageTitleXpath);

    public Dropdown ActionsDropdown => new(Driver, _actionsDropDownXpath);

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