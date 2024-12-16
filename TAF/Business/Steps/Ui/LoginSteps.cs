using Allure.NUnit.Attributes;
using OpenQA.Selenium;
using TAF.Business.Pages;
using TAF.Core.Configuration;
using TechTalk.SpecFlow;

namespace TAF.Business.Steps.Ui;

[Binding]
public class LoginSteps(IWebDriver driver) : BaseStep(driver)
{
    /// <summary>
    ///     This method handles the login process.
    ///     Firstly open login page by url and then try to enter credentials.
    ///     If the username and password are null, it will log a warning.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    private void Login(string? email, string? password)
    {
        var loginPage = new LoginPage(Driver, true, true);
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            Logger.Warn("Email or password is null");

        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password))
        {
            Logger.Warn("Email and password is null or empty");
            loginPage.SignInButton.Click();
        }
        else
        {
            Logger.Info("Entering credentials...");
            loginPage.EmailField.SendText(email);
            loginPage.PasswordField.SendText(password);
            loginPage.SignInButton.Click();
        }
    }

    [AllureStep("Login with valid credentials")]
    [Given(@"Login with valid credentials")]
    public LaunchesPage LoginWithValidCredentials()
    {
        Logger.Info("Logging with valid credentials...");
        var username = Configurator.ReadConfiguration().UserEmail;
        var password = Configurator.ReadConfiguration().UserPassword;
        Login(username, password);
        return new LaunchesPage(Driver, true);
    }
    [Given(@"Open login page")]
    public void OpenLoginPage()
    {
        _ = new LoginPage(Driver, true, true);
    }

    [When(@"I click login button")]
    public void ClickLoginButton()
    {
        
    }
}