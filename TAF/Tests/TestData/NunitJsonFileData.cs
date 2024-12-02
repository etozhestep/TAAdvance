using System.Reflection;
using NUnit.Framework;
using TAF.Business.Models;
using TAF.Core.Util;

namespace TAF.Tests.TestData;

public static class NunitJsonFileData
{
    public static IEnumerable<TestCaseData> GetTestData()
    {
        const string fileName = "launchesTestData.json";
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Tests", "TestData",
            fileName);

        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}");

        var fileData = File.ReadAllText(path);
        var testData = JsonUtil.DeserializeObject<List<LaunchesTestDataModel>>(fileData);

        if (testData == null)
            throw new InvalidOperationException("Unable to deserialize JSON data.");

        foreach (var data in testData) yield return new TestCaseData(data.RunName, data.IsRunNameExist);
    }
}