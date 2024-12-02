using AF.Utils.Wrappers;
using OpenQA.Selenium;
using TAF.Business.WebElements;

namespace TAF.Business.Pages;

public class LoginPage(IWebDriver driver, bool evaluateStatus = false, bool openPageByUrl = false)
    : BasePage(driver, evaluateStatus, openPageByUrl)
{
    private const string EmailFieldErrorXpath = "/ancestor::*[contains(@class,'login-field')]//span";

    private const string PasswordFieldErrorXpath = "/ancestor::*[contains(@class,'login-form')]" +
                                                   "//*[contains(@class,'password-field')]//span";

    private static readonly string Endpoint = string.Empty;
    private readonly By _emailFieldXpath = By.XPath("//input[@placeholder='Login']");
    private readonly By _passwordFieldXpath = By.XPath("//input[@placeholder='Password']");
    private readonly By _signInButtonXpath = By.XPath("//button[@type='submit']");

    public Field EmailField => new(Driver, _emailFieldXpath);
    public Field PasswordField => new(Driver, _passwordFieldXpath);
    public Button SignInButton => new(Driver, _signInButtonXpath);
    public Title EmailFieldError => new(Driver, By.XPath(_emailFieldXpath.Criteria + EmailFieldErrorXpath));
    public Title PasswordFieldError => new(Driver, By.XPath(_emailFieldXpath.Criteria + PasswordFieldErrorXpath));

    protected override bool EvaluateLoadedStatus()
    {
        try
        {
            return EmailField.IsDisplayed && PasswordField.IsDisplayed && SignInButton.IsDisplayed;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    protected override string GetEndpoint()
    {
        return Endpoint;
    }
}