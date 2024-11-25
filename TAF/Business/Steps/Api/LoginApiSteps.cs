using NLog;
using RestSharp;
using TAF.Business.Models;
using TAF.Core;

namespace TAF.Business.Steps.Api;

public class LoginApiSteps
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public RestResponse LoginWithInvalidData(UserModel model)
    {
        _logger.Info("Login using API with invalid credentials");
        var client = ApiHelper.GetClient();
        var request = ApiHelper.CreatePostRequest("user/login");
        request.AddJsonBody(new
        {
            login = model.Email,
            password = model.Pass
        });

        return client.Execute(request);
    }
}