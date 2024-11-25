using OpenQA.Selenium;
using TAF.Core.Util;

namespace AF.Utils.Wrappers;

public class Title(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly UiElement _uiElement = new(driver, locator);

    public bool IsDisplayed => _uiElement.Displayed;
    public new string Text => _uiElement.Text;
}