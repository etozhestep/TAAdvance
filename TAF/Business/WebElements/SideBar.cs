using OpenQA.Selenium;
using TAF.Core.Util;

namespace TAF.Business.WebElements;

public class SideBar(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly By _locator = locator;
    private readonly UiElement _sideBarContent = new(driver, locator);

    public bool IsDisplayed => _sideBarContent.Displayed;

    public IWebElement GetSideBarElementByXpath(string xpath)
    {
        return _sideBarContent.FindElement(By.XPath(xpath));
    }

    private IWebElement GetSideBarElementByName(string buttonName)
    {
        var xpath = By.XPath(_locator.Criteria + $"//*[contains(text(),'{buttonName}')]");
        return _sideBarContent.FindElement(xpath);
    }

    public void OpenPageByName(string buttonName)
    {
        GetSideBarElementByName(buttonName).Click();
    }

    public void OpenUserPopup()
    {
        var xpath = By.XPath(_locator.Criteria + "//img[@alt='avatar']");
        _sideBarContent.FindElement(xpath).Click();
    }
}