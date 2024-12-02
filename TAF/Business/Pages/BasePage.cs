using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TAF.Business.WebElements;
using TAF.Core.Configuration;

namespace TAF.Business.Pages;

/// <summary>
///     Base class for all pages. Contains common methods and properties. Use LoadableComponent for page loading.
/// </summary>
public abstract class BasePage : LoadableComponent<BasePage>
{
    private const string SideBarContentLocator = "//div[contains(@class,'sidebar-container')]";
    private static readonly By SideBarContentXpath = By.XPath(SideBarContentLocator);

    /// <summary>
    ///     Constructor contains driver instance. If ${openPageByUrl} true open base by base url + endpoint.
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="evaluateStatus"></param>
    /// <param name="openPageByUrl"></param>
    protected BasePage(IWebDriver driver, bool evaluateStatus = false, bool openPageByUrl = false)
    {
        Driver = driver;
        if (openPageByUrl)
            OpenByUrl();
        if (evaluateStatus)
            EvaluateLoadedStatus();
    }

    //public UserPopup UserPopup => new(Driver);
    public SideBar SideBar => new(Driver, SideBarContentXpath);
    protected IWebDriver Driver { get; }

    /// <summary>
    ///     Gets the endpoint of the page.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetEndpoint();

    /// <summary>
    ///     Evaluates the loaded status of the page.
    /// </summary>
    protected override void ExecuteLoad()
    {
        Driver.Navigate().GoToUrl(Configurator.ReadConfiguration().Url + GetEndpoint());
    }

    /// <summary>
    ///     Opens the page by URL.
    /// </summary>
    private void OpenByUrl()
    {
        TryLoad();
    }
}