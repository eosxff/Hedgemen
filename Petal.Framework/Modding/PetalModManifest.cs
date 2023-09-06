using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.Persistence;

namespace Petal.Framework.Modding;

/// <summary>
/// Mod details.
/// </summary>
[Serializable]
public sealed class PetalModManifest
{
	public static JsonTypeInfo<PetalModManifest> JsonTypeInfo
		=> PetalModManifestJsonTypeInfo.Default.PetalModManifest;

	public static PetalModManifest? FromJson(string json)
	{
		var manifest = JsonSerializer.Deserialize(json, JsonTypeInfo);
		return manifest;
	}

	public static PetalModManifest? FromJson(JsonElement json)
	{
		var manifest = json.Deserialize(JsonTypeInfo);
		return manifest;
	}

	[JsonPropertyName("schema_version")]
	public int SchemaVersion
	{
		get;
		set;
	} = 1;

	[JsonPropertyName("mod_id")]
	[JsonConverter(typeof(NamespacedString.JsonConverter))]
	public NamespacedString ModID
	{
		get;
		set;
	} = NamespacedString.Default;

	[JsonPropertyName("name")]
	public string Name
	{
		get;
		set;
	} = "Unnamed";

	[JsonPropertyName("version")]
	public string Version
	{
		get;
		set;
	} = "0.0.1";

	[JsonPropertyName("description")]
	public string Description
	{
		get;
		set;
	} = "I haven't made a description yet!";

	[JsonPropertyName("authors")]
	public IReadOnlyList<string> Authors
	{
		get;
		set;
	} = new List<string>();

	[JsonPropertyName("contact")]
	public PetalModManifestContactInfo Contact
	{
		get;
		set;
	} = new();

	[JsonPropertyName("depends")]
	public PetalModManifestDependenciesInfo Dependencies
	{
		get;
		set;
	} = new();

	[JsonPropertyName("mod_file_dll")]
	public string ModFileDll
	{
		get;
		set;
	} = string.Empty;

	[JsonPropertyName("mod_main")]
	public string ModMain
	{
		get;
		set;
	} = string.Empty;

	[JsonPropertyName("is_overhaul")]
	public bool IsOverhaul
	{
		get;
		set;
	}

	public override string ToString()
	{
		return JsonSerializer.Serialize(this, JsonTypeInfo);
	}
}

[Serializable]
public sealed class PetalModManifestContactInfo
{
	[JsonPropertyName("homepage")]
	public string Homepage
	{
		get;
		set;
	} = PetalGame.PetalRepositoryLink;

	[JsonPropertyName("source")]
	public string Source
	{
		get;
		set;
	} = PetalGame.PetalRepositoryLink;
}

[Serializable]
public sealed class PetalModManifestDependenciesInfo
{
	[JsonPropertyName("mods")]
	[JsonConverter(typeof(ImmutableListJsonConverter<NamespacedString, NamespacedString.JsonConverter>))]
	public IReadOnlyList<NamespacedString> Mods
	{
		get;
		set;
	} = new List<NamespacedString>();

	[JsonPropertyName("incompatible_mods"), JsonInclude]
	[JsonConverter(typeof(ImmutableListJsonConverter<NamespacedString, NamespacedString.JsonConverter>))]
	public IReadOnlyList<NamespacedString> IncompatibleMods
	{
		get;
		set;
	} = new List<NamespacedString>();

	[JsonPropertyName("referenced_dlls")]
	public IReadOnlyList<string> ReferencedDlls
	{
		get;
		set;
	} = new List<string>();
}

[JsonSourceGenerationOptions(IncludeFields = true)]
[JsonSerializable(typeof(PetalModManifest))]
internal partial class PetalModManifestJsonTypeInfo : JsonSerializerContext
{

}

[JsonSourceGenerationOptions(IncludeFields = true)]
[JsonSerializable(typeof(PetalModManifestDependenciesInfo))]
internal partial class PetalModManifestDependenciesInfoJsonTypeInfo : JsonSerializerContext
{

}
