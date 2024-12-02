using System.Reflection;
using TAF.Business.Models;
using TAF.Core.Util;
using Xunit.Sdk;

namespace TAF.Tests.TestData;

public class XunitJsonFileDataAttribute(string fileName) : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        ArgumentNullException.ThrowIfNull(testMethod);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Tests", "TestData",
            fileName);

        if (!File.Exists(path)) throw new ArgumentException($"Could not find file at path: {path}");
        var fileData = File.ReadAllText(path);
        var testData = JsonUtil.DeserializeObject<List<LaunchesTestDataModel>>(fileData);
        if (testData == null)
            throw new InvalidOperationException("Unable to deserialize JSON data.");

        return testData.Select(td => new object[] { td.RunName ?? string.Empty, td.IsRunNameExist });
    }
}