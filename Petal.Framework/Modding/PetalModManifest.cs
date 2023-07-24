using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Petal.Framework.Modding;

[Serializable]
public sealed class PetalModManifest
{
	public static PetalModManifest? FromJson(string json)
	{
		var manifest = JsonSerializer.Deserialize<PetalModManifest>(json, JsonDeserializeOptions);
		return manifest;
	}

	public static JsonSerializerOptions JsonDeserializeOptions
		=> new()
		{
			IgnoreReadOnlyProperties = true,
			IgnoreReadOnlyFields = true,
			WriteIndented = true,
			Converters = { }
		};

	[JsonPropertyName("schema_version")]
	public int SchemaVersion
	{
		get;
		init;
	} = 1;

	[JsonPropertyName("mod_id")]
	[JsonConverter(typeof(NamespacedString.JsonConverter))]
	public NamespacedString ModID
	{
		get;
		init;
	} = NamespacedString.Default;

	[JsonPropertyName("name")]
	public string Name
	{
		get;
		init;
	} = "Unnamed";

	[JsonPropertyName("version")]
	public string Version
	{
		get;
		init;
	} = "0.0.1";

	[JsonPropertyName("description")]
	public string Description
	{
		get;
		init;
	} = "I haven't made a description yet!";

	[JsonPropertyName("authors")]
	public IReadOnlyList<string> Authors
	{
		get;
		init;
	} = new List<string>();

	[JsonPropertyName("contact")]
	public PetalModManifestContactInfo Contact
	{
		get;
		init;
	} = new();

	[JsonPropertyName("depends")]
	public PetalModManifestDependenciesInfo Dependencies
	{
		get;
		init;
	} = new();

	[JsonPropertyName("mod_file_dll")]
	public string ModFileDll
	{
		get;
		init;
	} = string.Empty;

	[JsonPropertyName("mod_main")]
	public string ModMain
	{
		get;
		init;
	} = string.Empty;

	[JsonPropertyName("is_overhaul")]
	public bool IsOverhaul
	{
		get;
		init;
	} = false;

	public override string ToString()
	{
		return JsonSerializer.Serialize(this, JsonDeserializeOptions);
	}
}

[Serializable]
public sealed class PetalModManifestContactInfo
{
	[JsonPropertyName("homepage")]
	public string Homepage
	{
		get;
		init;
	} = PetalGame.PetalRepositoryLink;

	[JsonPropertyName("source")]
	public string Source
	{
		get;
		init;
	} = PetalGame.PetalRepositoryLink;
}

[Serializable]
public sealed class PetalModManifestDependenciesInfo
{
	[JsonPropertyName("mods")]
	public IReadOnlyList<string> Mods
	{
		get;
		init;
	} = new List<string>();

	[JsonPropertyName("incompatible_mods"), JsonInclude]
	[JsonConverter(typeof(NamespacedString.ImmutableListJsonConverter))]
	public IReadOnlyList<NamespacedString> IncompatibleMods
	{
		get;
		set;
	} = new List<NamespacedString>();

	[JsonPropertyName("referenced_dlls")]
	public IReadOnlyList<string> ReferencedDlls
	{
		get;
		init;
	} = new List<string>();
}

[JsonSourceGenerationOptions(IncludeFields = true)]
[JsonSerializable(typeof(PetalModManifest))]
public partial class PetalModManifestJsc : JsonSerializerContext
{

}
