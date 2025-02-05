using System.Net;
using TAF.Business.Steps.Api;

namespace TAF.Tests;

public class RestSharpTests
{
    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_GetAllLaunches_ReturnedListOfLaunches()
    {
        var listOfAllLaunches = ApiRestSharpLaunchesSteps.GetAllLaunches();

        Assert.That(listOfAllLaunches, Is.Not.Null);
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_GetAllLaunchesUnauthorized_ReturnedUnauthorized()
    {
        var response = ApiRestSharpLaunchesSteps.GetAllLaunchesUnauthorized();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_CreateLaunch_ReturnedCreated()
    {
        var response = ApiRestSharpLaunchesSteps.PostStartFirstLaunch();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_CreateLaunchUnauthorized_ReturnedUnauthorized()
    {
        var response = ApiRestSharpLaunchesSteps.PostStartFirstLaunchUnauthorized();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_CreateLaunchWithoutProjectName_ReturnedBadRequest()
    {
        var response = ApiRestSharpLaunchesSteps.PostStartFirstLaunchWithoutProjectName();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_DeleteLaunchById_LaunchDeleted()
    {
        var firstLaunchUuid = ApiRestSharpLaunchesSteps.GetAllLaunches()!.Launches.First().Uuid!;
        var firstLaunchId = ApiRestSharpLaunchesSteps.GetLaunchByUuid(firstLaunchUuid)!.Id;
        var response = ApiRestSharpLaunchesSteps.DeleteLaunchById(firstLaunchId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_DeleteLaunchByIdWithoutParameter_ReturnedBadRequest()
    {
        var response = ApiRestSharpLaunchesSteps.DeleteLaunchByIdWithoutParameter();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_DeleteLaunchByIdUnauthorized_ReturnedUnauthorized()
    {
        var firstLaunchId = ApiRestSharpLaunchesSteps.GetAllLaunches()!.Launches.First().Uuid!;
        var response = ApiRestSharpLaunchesSteps.DeleteLaunchByIdUnauthorized(firstLaunchId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_PutUpdateLaunchById_LaunchUpdated()
    {
        var firstLaunchUuid = ApiRestSharpLaunchesSteps.GetAllLaunches()!.Launches.First().Uuid!;
        var firstLaunch = ApiRestSharpLaunchesSteps.GetLaunchByUuid(firstLaunchUuid)!;

        const string newStatus = "PASSED";
        var response = ApiRestSharpLaunchesSteps.PutUpdateLaunchById(firstLaunch, newStatus);

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Does.Contain($"Launch with ID = '{firstLaunch.Id}' " +
                                                       $"successfully updated."));
        });
    }

    [Test]
    [Ignore("test")]
    [Category("API")]
    public void RestLaunches_PutUpdateLaunchWithoutBody_ReturnedBadRequest()
    {
        var firstLaunchUuid = ApiRestSharpLaunchesSteps.GetAllLaunches()!.Launches.First().Uuid!;
        var firstLaunch = ApiRestSharpLaunchesSteps.GetLaunchByUuid(firstLaunchUuid)!;

        var response = ApiRestSharpLaunchesSteps.PutUpdateLaunchByIdWithoutBody(firstLaunch);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}