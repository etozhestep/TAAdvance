using OpenQA.Selenium;
using TAF.Business.Pages;
using TAF.Business.Steps.Ui;
using NLog;

namespace TAF.Business
{
    public class LoginStep : BaseStep
    {
        public LoginStep(IWebDriver driver) : base(driver) { }

        public void Login(string? email, string? password)
        {
            var loginPage = new LoginPage(_driver, true, true);
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
    }
}