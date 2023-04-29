using System;
using System.Text.Json;

namespace Petal.Framework.Util;

public static class PetalUtilities
{
    public static T ReadFromJson<T>(string json, JsonSerializerOptions options)
    {
        var obj = JsonSerializer.Deserialize<T?>(json, options);
        if (obj is null)
        {
            throw new ArgumentException($"{typeof(T)} can not be created from {nameof(json)}.");
        }
        return obj;
    }
}