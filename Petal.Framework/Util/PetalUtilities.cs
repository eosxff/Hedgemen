using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Petal.Framework.Util;

public static class PetalUtilities
{
	[RequiresDynamicCode(""), RequiresUnreferencedCode("")]
	public static T ReadFromJson<T>(string json, JsonSerializerOptions options)
	{
		var obj = JsonSerializer.Deserialize<T?>(json, options);
		PetalExceptions.ThrowIfNullWithMessage(obj, $"{typeof(T)} can not be created from {nameof(json)}");
		return obj;
	}

	[RequiresDynamicCode(""), RequiresUnreferencedCode("")]
	public static T ReadFromJson<T>(JsonElement json, JsonSerializerOptions options)
	{
		var obj = json.Deserialize<T?>(options);
		PetalExceptions.ThrowIfNullWithMessage(obj, $"{typeof(T)} can not be created from {nameof(json)}");
		return obj;
	}

	public static T ReadFromJson<T>(string json, JsonTypeInfo<T> typeInfo)
	{
		var obj = JsonSerializer.Deserialize(json, typeInfo);
		PetalExceptions.ThrowIfNullWithMessage(obj, $"{typeof(T)} can not be created from {nameof(json)}");
		return obj;
	}

	public static T ReadFromJson<T>(JsonElement json, JsonTypeInfo<T> typeInfo)
	{
		var obj = json.Deserialize(typeInfo);
		PetalExceptions.ThrowIfNullWithMessage(obj, $"{typeof(T)} can not be created from {nameof(json)}");
		return obj;
	}
}
