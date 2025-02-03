using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Dropdown(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly UiElement _dropDown = new(driver, locator);

    private readonly By _optionXpath = By.XPath("//div[contains(@class,'ghostMenuButton__menu-item')]");
    public bool IsDisplayed => _dropDown.Displayed;


    public void SelectByValue(string value)
    {
        OpenDropDown();
        var options = GetOptions();
        var valueElement = options
            .FirstOrDefault(element =>
                element.Text.Equals(value, StringComparison.OrdinalIgnoreCase)
                || element.Text.StartsWith(value, StringComparison.OrdinalIgnoreCase)
                || element.Text.Contains(value, StringComparison.OrdinalIgnoreCase));

        if (valueElement is null)
            throw new NotFoundException($"Option with value '{value}' not found");

        Actions
            .MoveToElement(valueElement)
            .Click()
            .Perform();
    }

    private IEnumerable<IWebElement> GetOptions()
    {
        return _dropDown.FindElements(_optionXpath).ToList();
    }

    private void OpenDropDown()
    {
        _dropDown.Click();
    }
}