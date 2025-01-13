using Newtonsoft.Json;
using TAF.Business.Models;
using TAF.Core;
using TAF.Core.Configuration;
using TAF.Core.Util;

namespace TAF.Business.Steps.Api;

public static class ApiHttpClientLaunchesSteps
{
    private const string ProjectName = "default_personal";
    private static readonly string ApiToken = Configurator.ReadConfiguration().ApiKey;

    public static async Task<RootLaunches?> GetAllLaunches()
    {
        var response = await ApiHttpClientHelper.SendAsync(HttpMethod.Get, $"v1/{ProjectName}/launch", ApiToken);
        var content = await response.Content.ReadAsStringAsync();

        return JsonUtil.IsValidJson<RootLaunches>(content)
            ? JsonConvert.DeserializeObject<RootLaunches>(content)
            : throw new Exception("Content is null or unable to deserialize");
    }

    public static async Task<HttpResponseMessage> DeleteLaunchById(int launchId)
    {
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Delete, $"/v1/{ProjectName}/launch/{launchId}", ApiToken);
    }

    public static async Task<HttpResponseMessage> PostStartFirstLaunch()
    {
        var firstLaunch = (await GetAllLaunches())!.Launches.First();
        var newLaunchBody = new Launch
        {
            StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            Name = firstLaunch.Name
        };

        return await ApiHttpClientHelper.SendAsync(HttpMethod.Post, $"/v1/{ProjectName}/launch", ApiToken,
            newLaunchBody);
    }

    public static async Task<HttpResponseMessage> PutUpdateLaunchById(Launch launch, string status)
    {
        launch.Status = status;
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Put, $"/v1/{ProjectName}/launch/{launch.Id}/update",
            ApiToken, launch);
    }

    public static async Task<HttpResponseMessage> PutUpdateLaunchByIdWithoutBody(Launch launch)
    {
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Put, $"/v1/{ProjectName}/launch/{launch.Id}/update",
            ApiToken);
    }

    public static async Task<Launch?> GetLaunchByUuid(string uuid)
    {
        var response =
            await ApiHttpClientHelper.SendAsync(HttpMethod.Get, $"/v1/{ProjectName}/launch/uuid/{uuid}", ApiToken);
        var content = await response.Content.ReadAsStringAsync();

        return JsonUtil.IsValidJson<Launch>(content)
            ? JsonConvert.DeserializeObject<Launch>(content)
            : throw new Exception("Content is null or unable to deserialize");
    }

    public static async Task<HttpResponseMessage> GetAllLaunchesUnauthorized()
    {
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Get, $"/v1/{ProjectName}/launch");
    }

    public static async Task<HttpResponseMessage> PostStartFirstLaunchUnauthorized()
    {
        var firstLaunch = (await GetAllLaunches())!.Launches.First();
        firstLaunch.StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        return await ApiHttpClientHelper.SendAsync(HttpMethod.Post, $"/v1/{ProjectName}/launch", body: firstLaunch);
    }

    public static async Task<HttpResponseMessage> PostStartFirstLaunchWithoutProjectName()
    {
        var firstLaunch = (await GetAllLaunches())!.Launches.First();
        firstLaunch.StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        return await ApiHttpClientHelper.SendAsync(HttpMethod.Post, "/v1/launch", ApiToken, firstLaunch);
    }

    public static async Task<HttpResponseMessage> DeleteLaunchByIdWithoutParameter()
    {
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Delete, $"/v1/{ProjectName}/launch", ApiToken);
    }

    public static async Task<HttpResponseMessage> DeleteLaunchByIdUnauthorized(string launchId)
    {
        return await ApiHttpClientHelper.SendAsync(HttpMethod.Delete, $"/v1/{ProjectName}/launch/{launchId}");
    }
}