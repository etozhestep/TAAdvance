using OpenQA.Selenium;
using TAF.Business.Pages;

namespace TAF.Business.Steps.Ui;

public class NavigationSteps(IWebDriver driver) : BaseStep(driver)
{
    public LaunchesPage OpenLaunchesPageByUrl()
    {
        return new LaunchesPage(Driver, true, true);
    }
}