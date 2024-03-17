using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Petal.Framework.Persistence;

public interface IReadOnlyJsonManifest
{
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T? ReadType<T>(string name, JsonTypeInfo<T> jsonTypeInfo, T? defaultValue = default);

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T? ReadType<T>(NamespacedString name, JsonTypeInfo<T> jsonTypeInfo, T? defaultValue = default);

	public bool TryReadType<T>(string name, [NotNullWhen(true)] out T? value, JsonTypeInfo<T> jsonTypeInfo);
	public bool TryReadType<T>(NamespacedString name, [NotNullWhen(true)] out T? value, JsonTypeInfo<T> jsonTypeInfo);
}

public interface IWriteOnlyJsonManifest
{
	public void WriteType<T>(string name, T value, JsonTypeInfo<T> jsonTypeInfo);
	public void WriteType<T>(NamespacedString name, T value, JsonTypeInfo<T> jsonTypeInfo);
}

/// <summary>
/// A much simpler and less convenient version of PersistentData with the upside of not using reflection.
/// </summary>
public sealed class JsonManifest : IReadOnlyJsonManifest, IWriteOnlyJsonManifest
{
	public static DefaultJsonTypeInfo SupportedTypes => DefaultJsonTypeInfo.Default;

	/// <summary>
	/// Intended for internal use only.
	/// </summary>
	[JsonExtensionData]
	public Dictionary<string, JsonElement> ExtensionData
	{
		get;
		set;
	} = [];

	public JsonManifest()
	{

	}

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T? ReadType<T>(string name, JsonTypeInfo<T> jsonTypeInfo, T? defaultValue = default)
	{
		if(!ExtensionData.TryGetValue(name, out var json))
			return defaultValue;

		return json.Deserialize(jsonTypeInfo);
	}

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T? ReadType<T>(NamespacedString name, JsonTypeInfo<T> jsonTypeInfo, T? defaultValue = default)
		=> ReadType(name, jsonTypeInfo, defaultValue);

	public bool TryReadType<T>(string name, [NotNullWhen(true)] out T? value, JsonTypeInfo<T> jsonTypeInfo)
	{
		value = default;

		if(!ExtensionData.TryGetValue(name, out var json))
			return false;

		var deserializedValue = json.Deserialize(jsonTypeInfo);

		switch(deserializedValue is not null)
		{
			case true:
				value = deserializedValue;
				return true;
			case false:
				return false;
		}
	}

	public bool TryReadType<T>(NamespacedString name, [NotNullWhen(true)] out T? value, JsonTypeInfo<T> jsonTypeInfo)
		=> TryReadType(name.FullName, out value, jsonTypeInfo);

	public void WriteType<T>(string name, T value, JsonTypeInfo<T> jsonTypeInfo)
	{
		ExtensionData.TryAdd(name, JsonSerializer.SerializeToElement(value, jsonTypeInfo));
	}

	public void WriteType<T>(NamespacedString name, T value, JsonTypeInfo<T> jsonTypeInfo)
		=> WriteType(name.FullName, value, jsonTypeInfo);

	public override string ToString()
		=> JsonSerializer.Serialize(this, SupportedTypes.JsonManifest);
}