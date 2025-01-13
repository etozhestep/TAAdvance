using System.Net;
using RestSharp;
using TAF.Core.Configuration;

namespace TAF.Core;

public static class ApiRestSharpHelper
{
    public static RestClient GetClient()
    {
        var options = new RestClientOptions(Configurator.ReadConfiguration().Url)
        {
            Proxy = new WebProxy(Configurator.ReadConfiguration().FiddlerAddress)
        };

        var restClient = new RestClient(options);

        return restClient;
    }

    public static RestRequest CreatePostRequest(string endpoint)
    {
        var restRequest = new RestRequest(endpoint, Method.Post);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }

    public static RestRequest CreateGetRequest(string endpoint)
    {
        var restRequest = new RestRequest(endpoint);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }

    public static RestRequest CreateDeleteRequest(string endpoint)
    {
        var restRequest = new RestRequest(endpoint, Method.Delete);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }

    public static RestRequest CreatePutRequest(string endpoint)
    {
        var restRequest = new RestRequest(endpoint, Method.Put);
        restRequest.AddHeader("Accept", "application/json");
        return restRequest;
    }
}