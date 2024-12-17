using System.Collections.ObjectModel;
using System.Drawing;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using TAF.Core.Configuration;
using TAF.Core.Util;
using Exception = System.Exception;

namespace TAF.Business.WebElements;

/// <summary>
///     Wrapper for IWebElement interface with additional methods for working with elements.
/// </summary>
public class UiElement : IWebElement
{
    protected readonly Actions Actions;
    protected readonly IWebDriver? Driver;
    protected readonly By? Locator;
    protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
    protected readonly Waits Waits;
    private IWebElement _element = null!;

    private UiElement(IWebDriver driver)
    {
        Driver = driver;
        Actions = new Actions(driver);
        Waits = new Waits(driver);
    }

    public UiElement(IWebDriver driver, IWebElement element) : this(driver)
    {
        _element = element;
    }

    public UiElement(IWebDriver driver, By locator) : this(driver)
    {
        _element = Waits.WaitForExist(locator);
        Locator = locator;
    }

    public UiElement(IWebDriver driver, string xpath) : this(driver)
    {
        _element = Waits.WaitForExist(By.XPath(xpath));
    }

    public IWebElement FindElement(By by)
    {
        Waits.WaitForVisibility(by);
        return _element.FindElement(by);
    }

    public ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        Driver!.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(Configurator.ReadConfiguration().Ui.TimeOut);
        var collection = _element.FindElements(by);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
        return collection;
    }

    public void Clear()
    {
        try
        {
            _element.Clear();
        }
        catch (Exception)
        {
            try
            {
                _element.SendKeys("");
            }
            catch (Exception)
            {
                ClearAll();
            }
        }
    }

    /// <summary>
    ///     Before sending text to the element, it moves the cursor to the element and then check if the text was sent.
    /// </summary>
    /// <param name="text"></param>
    public void SendKeys(string? text)
    {
        try
        {
            Clear();
            _element.SendKeys(text);
        }
        catch (Exception)
        {
            Clear();
            Actions
                .MoveToElement(_element)
                .Click()
                .SendKeys(text)
                .Build()
                .Perform();
        }
    }

    /// <summary>
    ///     Submits the form element if it is a form element; otherwise, it sends the Enter key to the element.
    /// </summary>
    public void Submit()
    {
        try
        {
            _element.Submit();
        }
        catch (ElementNotInteractableException)
        {
            _element.SendKeys(Keys.Enter);
        }
    }

    /// <summary>
    ///     Clicks the element if it is clickable. If not, it moves the cursor to the element and then clicks it.
    /// </summary>
    public void Click()
    {
        try
        {
            Hover();
            _element.Click();
        }
        catch (ElementNotInteractableException)
        {
            try
            {
                Actions
                    .MoveToElement(_element)
                    .Click()
                    .Build()
                    .Perform();
            }
            catch (ElementNotInteractableException)
            {
                MoveToElement();
                ((IJavaScriptExecutor)Driver!).ExecuteScript("arguments[0].Click();", _element);
            }
        }
        catch (StaleElementReferenceException)
        {
            if (Locator is null)
                throw;
            _element = new UiElement(Driver!, Locator);
            _element.Click();
        }
    }

    public string? GetAttribute(string attributeName)
    {
        return _element.GetAttribute(attributeName);
    }

    public string GetDomAttribute(string attributeName)
    {
        return _element.GetDomAttribute(attributeName);
    }

    public string GetDomProperty(string propertyName)
    {
        return _element.GetDomProperty(propertyName);
    }

    public string GetCssValue(string propertyName)
    {
        return _element.GetCssValue(propertyName);
    }

    public ISearchContext GetShadowRoot()
    {
        return _element.GetShadowRoot();
    }


    public string TagName => _element.TagName;

    /// <summary>
    ///     Gets the text of the element. If the element is an input, it gets the value of the element.
    /// </summary>
    public string Text => (_element.Text ?? GetAttribute(GetAttribute("value") is not null
        ? "innerText"
        : "value")) ?? string.Empty;

    public bool Enabled => _element.Enabled;
    public bool Selected => _element.Selected;
    public Point Location => _element.Location;
    public Size Size => _element.Size;

    public bool Displayed
    {
        get
        {
            try
            {
                if (Locator is null)
                    return _element.Displayed;
                return Locator != null && Waits.WaitForExist(Locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }

    private void ClearAll()
    {
        _element.SendKeys(Keys.Control + "a");
        _element.SendKeys(Keys.Backspace);
    }

    /// <summary>
    ///     Moves the cursor to the element and then hovers over the element.
    /// </summary>
    private void Hover()
    {
        Actions
            .MoveToElement(_element)
            .Build()
            .Perform();
    }

    /// <summary>
    ///     Moves the cursor to the element.
    /// </summary>
    private void MoveToElement()
    {
        try
        {
            Hover();
        }
        catch (Exception)
        {
            ((IJavaScriptExecutor)Driver!).ExecuteScript("arguments[0].scrollIntoView(true);", _element);
        }
    }
}