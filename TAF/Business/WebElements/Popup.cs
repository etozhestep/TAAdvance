using OpenQA.Selenium;

namespace TAF.Business.WebElements;

public class Popup(IWebDriver driver, By locator) : UiElement(driver, locator)
{
    private readonly By _deleteButtonXpath = By.XPath("//button[contains(text(),'Delete')]");
    private readonly By _popupTitleXpath = By.XPath("//span[contains(@class,'modalHeader__modal-title')]");
    private readonly UiElement _uiElement = new(driver, locator);

    public Title PopupTitle => new(Driver, _uiElement.FindElement(_popupTitleXpath));
    public Button DeleteButton => new(Driver, _uiElement.FindElement(_deleteButtonXpath));
}