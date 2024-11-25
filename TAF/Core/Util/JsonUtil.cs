using Newtonsoft.Json;

namespace TAF.Core.Util;

public static class JsonUtil
{
    /// <summary>
    ///     Serialize an object to JSON string.
    /// </summary>
    public static string SerializeObject<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }

    /// <summary>
    ///     Deserialize JSON string to object.
    /// </summary>
    public static T? DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    ///     Format JSON string.
    /// </summary>
    public static string FormatJson(string json)
    {
        var parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }

    /// <summary>
    ///     Check if JSON string is valid.
    /// </summary>
    public static bool IsValidJson<T>(string json)
    {
        try
        {
            JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidJson(object model)
    {
        try
        {
            JsonConvert.SerializeObject(model);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Get value by key from JSON string.
    /// </summary>
    public static T? GetValueByKey<T>(string json, string key)
    {
        var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        if (jsonObject != null && jsonObject.TryGetValue(key, out var value))
            return JsonConvert.DeserializeObject<T>(value.ToString() ?? string.Empty);
        throw new KeyNotFoundException($"Key '{key}' not found in JSON.");
    }
}