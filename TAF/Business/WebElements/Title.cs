using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Title : UiElement
{
    private readonly UiElement _uiElement;

    public Title(IWebDriver driver, IWebElement element) : base(driver, element)
    {
        _uiElement = new UiElement(Driver, element);
    }

    public Title(IWebDriver driver, By locator) : base(driver, locator)
    {
        _uiElement = new UiElement(Driver, locator);
    }

    public bool IsDisplayed => _uiElement.Displayed;
    public new string Text => _uiElement.Text;
}