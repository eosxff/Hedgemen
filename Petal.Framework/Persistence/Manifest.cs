using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.Util;

namespace Petal.Framework.Persistence;

public sealed class Manifest
{
	public static JsonTypeInfo<Manifest> JsonTypeInfo
		=> ManifestJsonTypeInfo.Default.Manifest;

	public static Manifest FromJson(JsonElement element)
	{
		var manifest = element.Deserialize(JsonTypeInfo);
		PetalExceptions.ThrowIfNull(manifest);
		return manifest;
	}

	public static Manifest FromJson(string json)
	{
		PetalExceptions.ThrowIfNullOrEmpty(json, nameof(json));
		var manifest = JsonSerializer.Deserialize(json, JsonTypeInfo);
		PetalExceptions.ThrowIfNull(manifest);
		return manifest;
	}

	public static Manifest FromStream(Stream stream)
	{
		var reader = new StreamReader(stream);
		return FromJson(reader.ReadToEnd());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TField? ReadField<TField>(NamespacedString name, TField? defaultValue = default)
	{
		ReadField(name.FullName, out var field, defaultValue);
		return field;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TField? ReadField<TField>(string name, TField? defaultValue = default)
	{
		ReadField(name, out var field, defaultValue);
		return field;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadField<TField>(
		NamespacedString name,
		[NotNullWhen(true), NotNullIfNotNull("defaultValue")]
		out TField? field,
		TField defaultValue = default)
		=> ReadField(name.FullName, out field);

	public bool ReadField<TField>(
		string name,
		[NotNullWhen(true), NotNullIfNotNull(nameof(defaultValue))]
		out TField? field,
		TField? defaultValue = default)
	{
		field = defaultValue;

		if (!ExtensionData.TryGetValue(name, out var element))
			return false;

		var deserializedField = element.Deserialize<TField>(DefaultJsonOptions);

		if (deserializedField is null)
			return false;

		field = deserializedField;
		return true;
	}

	/// <summary>
	/// This is only public for serialization purposes. Highly recommended to not mess with it directly.
	/// </summary>
	[JsonExtensionData, JsonInclude]
	public Dictionary<string, JsonElement> ExtensionData
	{
		get;
		set;
	} = new();

	public T NewInstance<T>() where T : IManifestReadable, new()
	{
		var obj = new T();
		obj.ReadManifest(this);
		return obj;
	}

	private static JsonSerializerOptions DefaultJsonOptions
		=> new()
		{
			WriteIndented = true,
			IncludeFields = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Manifest))]
internal partial class ManifestJsonTypeInfo : JsonSerializerContext
{

}
