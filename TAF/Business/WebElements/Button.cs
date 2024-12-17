using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Button(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly UiElement _uiElement = new(driver, locator);

    public bool IsDisplayed => _uiElement.Displayed;

    public bool IsEnabled
    {
        get
        {
            var attribute = _uiElement.GetAttribute("aria-disabled");
            return string.IsNullOrEmpty(attribute);
        }
    }

    public string ButtonText => _uiElement.Text;

    public new void Click()
    {
        _uiElement.Click();
    }

    public new void Submit()
    {
        _uiElement.Submit();
    }
}