using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TAF.Core.Configuration;

namespace TAF.Core;

public class ApiHttpClientHelper
{
    private static readonly HttpClient Client;

    static ApiHttpClientHelper()
    {
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = new WebProxy(Configurator.ReadConfiguration().FiddlerAddress),
            UseProxy = true,
        };
        
        Client = new HttpClient(httpClientHandler);
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, string? token = null,
        object? body = null)
    {
        var baseUrl = Configurator.ReadConfiguration().Url;
        using var request = new HttpRequestMessage(method, baseUrl + endpoint);

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body == null) return await Client.SendAsync(request);
        var json = JsonConvert.SerializeObject(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return await Client.SendAsync(request);
    }
}