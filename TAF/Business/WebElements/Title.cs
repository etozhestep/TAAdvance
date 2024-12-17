using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Title(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly UiElement _uiElement = new(driver, locator);

    public bool IsDisplayed => _uiElement.Displayed;
    public new string Text => _uiElement.Text;
}