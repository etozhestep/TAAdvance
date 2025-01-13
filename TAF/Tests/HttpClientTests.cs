using System.Net;
using TAF.Business.Steps.Api;

namespace TAF.Tests;

[TestFixture]
[Category("API")]
public class HttpClientTests
{
    [Test]
    public async Task HttpLaunches_GetAllLaunches_ReturnedListOfLaunches()
    {
        var listOfAllLaunches = await ApiHttpClientLaunchesSteps.GetAllLaunches();
        Assert.That(listOfAllLaunches, Is.Not.Null);
    }

    [Test]
    public async Task HttpLaunches_GetAllLaunchesUnauthorized_ReturnedUnauthorized()
    {
        var response = await ApiHttpClientLaunchesSteps.GetAllLaunchesUnauthorized();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task HttpLaunches_CreateLaunch_ReturnedCreated()
    {
        var response = await ApiHttpClientLaunchesSteps.PostStartFirstLaunch();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task HttpLaunches_CreateLaunchUnauthorized_ReturnedUnauthorized()
    {
        var response = await ApiHttpClientLaunchesSteps.PostStartFirstLaunchUnauthorized();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task HttpLaunches_CreateLaunchWithoutProjectName_ReturnedBadRequest()
    {
        var response = await ApiHttpClientLaunchesSteps.PostStartFirstLaunchWithoutProjectName();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task HttpLaunches_DeleteLaunchById_LaunchDeleted()
    {
        var firstLaunchUuid = (await ApiHttpClientLaunchesSteps.GetAllLaunches())!.Launches.First().Uuid;
        var firstLaunchId  = (await ApiHttpClientLaunchesSteps.GetLaunchByUuid(firstLaunchUuid!))!.Id;
        var response = await ApiHttpClientLaunchesSteps.DeleteLaunchById(firstLaunchId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task HttpLaunches_DeleteLaunchByIdWithoutParameter_ReturnedBadRequest()
    {
        var response = await ApiHttpClientLaunchesSteps.DeleteLaunchByIdWithoutParameter();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task HttpLaunches_DeleteLaunchByIdUnauthorized_ReturnedUnauthorized()
    {
        var firstLaunchId = (await ApiHttpClientLaunchesSteps.GetAllLaunches())!.Launches.First().Uuid;
        var response = await ApiHttpClientLaunchesSteps.DeleteLaunchByIdUnauthorized(firstLaunchId!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task HttpLaunches_PutUpdateLaunchById_LaunchUpdated()
    {
        const string newStatus = "PASSED";

        var firstLaunchUuid = (await ApiHttpClientLaunchesSteps.GetAllLaunches())!.Launches.First().Uuid;
        var firstLaunch = await ApiHttpClientLaunchesSteps.GetLaunchByUuid(firstLaunchUuid!);
        var response = await ApiHttpClientLaunchesSteps.PutUpdateLaunchById(firstLaunch!, newStatus);
        var content = await response.Content.ReadAsStringAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content,
                Does.Contain($"Launch with ID = '{firstLaunch!.Id}' successfully updated."));
        });
    }

    [Test]
    public async Task HttpLaunches_PutUpdateLaunchWithoutBody_ReturnedBadRequest()
    {
        var firstLaunchUuid = (await ApiHttpClientLaunchesSteps.GetAllLaunches())!.Launches.First().Uuid;
        var firstLaunch = await ApiHttpClientLaunchesSteps.GetLaunchByUuid(firstLaunchUuid!);
        var response = await ApiHttpClientLaunchesSteps.PutUpdateLaunchByIdWithoutBody(firstLaunch!);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}