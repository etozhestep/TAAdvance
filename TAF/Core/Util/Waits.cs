using System.Reflection;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace TAF.Core.Util;

/// <summary>
///     Wrapper class for Selenium WebDriverWait to use specific waits.
/// </summary>
public class Waits
{
    private readonly IWebDriver _driver;
    private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly WebDriverWait _wait;

    public Waits(IWebDriver driver)
    {
        _driver = driver;
        Timeout = TimeSpan.FromSeconds(Configurator.ReadConfiguration().Ui.TimeOut);
        _wait = new WebDriverWait(driver, Timeout);
    }

    public Waits(IWebDriver driver, TimeSpan timeout)
    {
        _driver = driver;
        Timeout = timeout;
        _wait = new WebDriverWait(driver, Timeout);
    }

    private TimeSpan Timeout { get; }

    public IWebElement WaitForVisibility(By locator)
    {
        try
        {
            return _wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }
        catch (WebDriverTimeoutException exception)
        {
            throw new WebDriverTimeoutException(
                $"Element with locator {locator} is not visible after {Timeout} seconds", exception);
        }
    }

    public IWebElement WaitForVisibility(By locator, TimeSpan timeout)
    {
        try
        {
            return new WebDriverWait(_driver, timeout).Until(ExpectedConditions.ElementIsVisible(locator));
        }
        catch (WebDriverTimeoutException exception)
        {
            throw new WebDriverTimeoutException(
                $"Element with locator {locator} is not visible after {timeout} seconds", exception);
        }
    }

    public bool WaitForInvisibility(UiElement element)
    {
        try
        {
            _wait.Until(d => !element.Displayed);
            return true;
        }
        catch (NoSuchElementException)
        {
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            throw new WebDriverTimeoutException("Element still visible");
        }
    }

    public bool WaitForElementInvisible(By locator)
    {
        try
        {
            return _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }
        catch (WebDriverTimeoutException exception)
        {
            throw new WebDriverTimeoutException(
                $"Element with locator {locator} is still visible after {Timeout} seconds", exception);
        }
    }

    public bool WaitForElementInvisible(By locator, TimeSpan timeout)
    {
        try
        {
            return new WebDriverWait(_driver, timeout)
                .Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }
        catch (WebDriverTimeoutException exception)
        {
            throw new WebDriverTimeoutException(
                $"Element with locator {locator} is still visible after {timeout} seconds", exception);
        }
    }

    public void WaitForElementsInvisible(By locator)
    {
        try
        {
            _driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(Configurator.ReadConfiguration().Ui.TimeOut);
            var elements = _driver.FindElements(locator);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
            foreach (var _ in elements) _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }
        catch (TargetInvocationException timeoutException)
        {
            _logger.Error($"Elements with locator {locator} are still visible after " +
                          $"{Timeout} seconds", timeoutException);
            throw;
        }
        catch (Exception e)
        {
            _logger.Error("Error while waiting for elements invisible " + e);
            throw;
        }
    }

    public void WaitForElementsInvisible(By locator, TimeSpan timeout)
    {
        try
        {
            var elements = WaitForElementsPresence(locator);
            foreach (var _ in elements)
                new WebDriverWait(_driver, timeout)
                    .Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }
        catch (WebDriverTimeoutException timeoutException)
        {
            _logger.Error($"Elements with locator {locator} are still visible after {timeout} seconds",
                timeoutException);
        }
        catch (Exception e)
        {
            _logger.Error("Error while waiting for elements invisible " + e);
            throw;
        }
    }

    public IWebElement WaitForExist(By locator)
    {
        try
        {
            return _wait.Until(ExpectedConditions.ElementExists(locator));
        }
        catch (WebDriverTimeoutException timeoutException)
        {
            _logger.Error("Element with locator " + locator + " is not exist after " + Timeout +
                          " seconds", timeoutException);
            throw;
        }
        catch (Exception e)
        {
            _logger.Error("Error while waiting for element exist " + e);
            throw;
        }
    }

    public void WaitForExist(IWebElement? element)
    {
        try
        {
            _wait.Until(_ =>
            {
                try
                {
                    return element!.Displayed ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });
        }
        catch (Exception e)
        {
            _logger.Error("Error while waiting for element exist " + e);
            throw;
        }
    }

    public IReadOnlyCollection<IWebElement> WaitForElementsPresence(By locator)
    {
        try
        {
            _driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(Configurator.ReadConfiguration().Ui.TimeOut);
            var elements = _driver.FindElements(locator);
            foreach (var element in elements) WaitForExist(element);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
            return elements;
        }
        catch (WebDriverTimeoutException timeoutException)
        {
            _logger.Error("Elements with locator " + locator + " are not present after " + Timeout +
                          " seconds", timeoutException);
            throw;
        }
        catch (Exception e)
        {
            _logger.Error("Error while waiting for elements presence " + e);
            throw;
        }
    }

    public void WaitForFrameAvailable(By locator)
    {
        try
        {
            _wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(locator));
        }
        catch (WebDriverTimeoutException ex)
        {
            _logger.Error("Frame with locator " + locator + " is not available after " + Timeout +
                          " seconds", ex);
            throw;
        }
    }

    public IWebElement WaitForClickable(By locator)
    {
        try
        {
            return _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }
        catch (WebDriverTimeoutException ex)
        {
            _logger.Error("Element with locator " + locator + " is not clickable after " + Timeout +
                          " seconds", ex);
            throw;
        }
    }

    public bool WaitForTaskComplete(TaskCompletionSource taskCompletionSource)
    {
        _wait.Until(_ => taskCompletionSource.Task.IsCompleted);
        return taskCompletionSource.Task.IsCompleted;
    }

    public bool IsElementDisappeared(By locator)
    {
        try
        {
            _wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool IsElementVisible(By locator)
    {
        try
        {
            var collection = WaitForElementsPresence(locator);
            return collection.Count > 0;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public IWebElement WaitForClickable(IWebElement element)
    {
        return _wait.Until(ExpectedConditions.ElementToBeClickable(element));
    }
}