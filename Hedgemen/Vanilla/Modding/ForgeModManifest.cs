using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework;

namespace Hgm.Vanilla.Modding;

[Serializable]
public sealed class ForgeModManifest
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
		set;
	} = 1;

	[JsonPropertyName("namespaced_id")]
	public string NamespacedID
	{
		get;
		set;
	} = NamespacedString.Default.FullName;

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
	public HedgemenModManifestContactInfo Contact
	{
		get;
		set;
	} = new();

	[JsonPropertyName("depends")]
	public HedgemenModManifestDependenciesInfo Dependencies
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
	} = false;
}

[Serializable]
public sealed class HedgemenModManifestContactInfo
{
	[JsonPropertyName("homepage")]
	public string Homepage
	{
		get;
		set;
	} = "https://github.com/eosxff/Hedgemen";

	[JsonPropertyName("source")]
	public string Source
	{
		get;
		set;
	} = "https://github.com/eosxff/Hedgemen";
}

[Serializable]
public sealed class HedgemenModManifestDependenciesInfo
{
	[JsonPropertyName("mods")]
	public IReadOnlyList<string> Mods
	{
		get;
		set;
	} = new List<string>();

	[JsonPropertyName("incompatible_mods")]
	public IReadOnlyList<string> IncompatibleMods
	{
		get;
		set;
	} = new List<string>();

	[JsonPropertyName("referenced_dlls")]
	public IReadOnlyList<string> Dlls
	{
		get;
		set;
	} = new List<string>();

	[JsonPropertyName("hedgemen")]
	public string Hedgemen
	{
		get;
		set;
	} = Hgm.Hedgemen.Version.ToString();
}