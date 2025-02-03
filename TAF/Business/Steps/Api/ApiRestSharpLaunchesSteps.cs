using RestSharp;
using TAF.Business.Models;
using TAF.Core;
using TAF.Core.Configuration;
using TAF.Core.Util;

namespace TAF.Business.Steps.Api;

public class ApiRestSharpLaunchesSteps
{
    private const string ProjectName = "default_personal";
    private static readonly string ApiToken = "Bearer " + Configurator.ReadConfiguration().ApiKey;


    public static RootLaunches? GetAllLaunches()
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreateGetRequest($"v1/{ProjectName}/launch");
        request.AddHeader("Authorization", ApiToken);
        var response = client.Execute(request);

        return response.Content is not null && JsonUtil.IsValidJson<RootLaunches>(response.Content)
            ? JsonUtil.DeserializeObject<RootLaunches>(response.Content)
            : throw new Exception("Content is null or unable to deserialize");
    }

    public static RestResponse GetAllLaunchesUnauthorized()
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreateGetRequest($"v1/{ProjectName}/launch");
        var response = client.Execute(request);

        return response;
    }

    public static RestResponse PostStartFirstLaunch()
    {
        var firstLaunch = GetAllLaunches()!.Launches.First();
        var newLaunchBody = new Launch
        {
            StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            Name = firstLaunch.Name
        };
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreatePostRequest($"v1/{ProjectName}/launch");
        request.AddHeader("Authorization", ApiToken);
        request.AddJsonBody(newLaunchBody);
        return client.Execute(request);
    }

    public static RestResponse PostStartFirstLaunchUnauthorized()
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreatePostRequest($"v1/{ProjectName}/launch");
        var firstLaunch = GetAllLaunches()!.Launches.First();
        firstLaunch.StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        request.AddJsonBody(firstLaunch);
        return client.Execute(request);
    }

    public static RestResponse PostStartFirstLaunchWithoutProjectName()
    {
        var client = ApiRestSharpHelper.GetClient();

        var request = ApiRestSharpHelper.CreatePostRequest("v1/launch");
        var firstLaunch = GetAllLaunches()!.Launches.First();
        firstLaunch.StartTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        request.AddHeader("Authorization", ApiToken);

        request.AddJsonBody(firstLaunch);
        return client.Execute(request);
    }

    public static RestResponse DeleteLaunchById(int launchId)
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreateDeleteRequest($"v1/{ProjectName}/launch/{launchId}");
        request.AddHeader("Authorization", ApiToken);

        return client.Execute(request);
    }

    public static RestResponse DeleteLaunchByIdWithoutParameter()
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreateDeleteRequest($"v1/{ProjectName}/launch");
        request.AddHeader("Authorization", ApiToken);

        return client.Execute(request);
    }

    public static RestResponse DeleteLaunchByIdUnauthorized(string launchId)
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreatePostRequest($"v1/{ProjectName}/launch");
        request.AddParameter("ids", launchId);

        return client.Execute(request);
    }

    public static RestResponse PutUpdateLaunchById(Launch launch, string status)
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreatePutRequest($"v1/{ProjectName}/launch/{launch.Id}/update");

        launch.Status = status;
        request.AddHeader("Authorization", ApiToken);
        request.AddBody(launch);

        return client.Execute(request);
    }

    public static RestResponse PutUpdateLaunchByIdWithoutBody(Launch launch)
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreatePutRequest($"v1/{ProjectName}/launch/{launch.Id}/update");
        request.AddHeader("Authorization", ApiToken);

        return client.Execute(request);
    }

    public static Launch? GetLaunchByUuid(string uuid)
    {
        var client = ApiRestSharpHelper.GetClient();
        var request = ApiRestSharpHelper.CreateGetRequest($"v1/{ProjectName}/launch/uuid/{uuid}");

        request.AddHeader("Authorization", ApiToken);
        var response = client.Execute(request);

        return response.Content is not null && JsonUtil.IsValidJson<Launch>(response.Content)
            ? JsonUtil.DeserializeObject<Launch>(response.Content)
            : throw new Exception("Content is null or unable to deserialize");
    }
}