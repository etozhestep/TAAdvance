using RestSharp;
using TAF.Core.Configuration;

namespace TAF.Core;

public static class ApiHelper
{
    public static RestClient GetClient()
    {
        var restClient = new RestClient(Configurator.ReadConfiguration().Url);
        return restClient;
    }

    public static RestRequest CreatePostRequest(string endpoint)
    {
        var url = Path.Combine(Configurator.ReadConfiguration().Url, endpoint);
        var restRequest = new RestRequest(url, Method.Post);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }
}