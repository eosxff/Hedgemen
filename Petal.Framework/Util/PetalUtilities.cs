using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Petal.Framework.Util;

public static class PetalUtilities
{
	public static T ReadFromJson<T>(string json, JsonSerializerOptions options)
	{
		var obj = JsonSerializer.Deserialize<T?>(json, options);

		if (obj is null)
			throw new ArgumentException($"{typeof(T)} can not be created from {nameof(json)}.");

		return obj;
	}

	public static T ReadFromJson<T>(JsonElement json, JsonSerializerOptions options)
	{
		var obj = json.Deserialize<T?>(options);

		if (obj is null)
			throw new ArgumentException($"{typeof(T)} can not be created from {nameof(json)}.");

		return obj;
	}

	public static T ReadFromJson<T>(string json, JsonTypeInfo<T> typeInfo)
	{
		var obj = JsonSerializer.Deserialize(json, typeInfo);

		if (obj is null)
			throw new ArgumentException($"{typeof(T)} can not be created from {nameof(json)}.");

		return obj;
	}

	public static T ReadFromJson<T>(JsonElement json, JsonTypeInfo<T> typeInfo)
	{
		var obj = json.Deserialize(typeInfo);

		if (obj is null)
			throw new ArgumentException($"{typeof(T)} can not be created from {nameof(json)}.");

		return obj;
	}
}
