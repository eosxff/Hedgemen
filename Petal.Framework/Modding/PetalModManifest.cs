using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Persistence;

namespace Petal.Framework.Modding;

[Serializable]
public sealed class PetalModManifest
{
	public static JsonSerializerOptions JsonDeserializeOptions
		=> new()
		{
			IgnoreReadOnlyProperties = true,
			IgnoreReadOnlyFields = true,
			WriteIndented = true,
			Converters = { }
		};
	
	[JsonInclude, JsonPropertyName("schema_version")]
	public int SchemaVersion
	{
		get;
		init;
	} = 1;

	[JsonPropertyName("namespaced_id")]
	public string NamespacedID
	{
		get;
		init;
	} = NamespacedString.Default.FullName;

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
}

[Serializable]
public sealed class PetalModManifestContactInfo
{
	[JsonPropertyName("homepage")]
	public string Homepage
	{
		get;
		init;
	} = PetalModLoader.PetalRepositoryLink;

	[JsonPropertyName("source")]
	public string Source
	{
		get;
		init;
	} = PetalModLoader.PetalRepositoryLink;
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

	[JsonPropertyName("incompatible_mods")]
	public IReadOnlyList<string> IncompatibleMods
	{
		get;
		init;
	} = new List<string>();

	[JsonPropertyName("referenced_dlls")]
	public IReadOnlyList<string> ReferencedDlls
	{
		get;
		init;
	} = new List<string>();
}

[JsonSerializable(typeof(PetalModManifest))]
internal partial class PetalModManifestJsonSourceGeneration : JsonSerializerContext
{
	
}