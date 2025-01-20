using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Button : UiElement
{
    private readonly UiElement _uiElement;

    public Button(IWebDriver driver, IWebElement element) : base(driver, element)
    {
        _uiElement = new UiElement(Driver, element);
    }

    public Button(IWebDriver driver, By locator) : base(driver, locator)
    {
        _uiElement = new UiElement(Driver, locator);
    }

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