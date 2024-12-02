using Allure.NUnit.Attributes;
using RestSharp;
using TAF.Core.Configuration;

namespace TAF.Core;

public static class ApiHelper
{
    [AllureStep("Set up Url and return client")]
    public static RestClient GetClient()
    {
        var restClient = new RestClient(Configurator.ReadConfiguration().Url);
        return restClient;
    }

    [AllureStep("Create Post Request")]
    public static RestRequest CreatePostRequest(string endpoint)
    {
        var url = Path.Combine(Configurator.ReadConfiguration().Url, endpoint);
        var restRequest = new RestRequest(url, Method.Post);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }
}