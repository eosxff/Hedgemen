using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Petal.Framework.IO;

namespace Petal.Framework.Modding;

[Serializable]
public sealed class PetalModList
{
	public static JsonTypeInfo<PetalModList> JsonTypeInfo
		=> PetalModListJsonTypeInfo.Default.PetalModList;

	public static PetalModList FromFile(FileInfo file)
	{
		var petalModList = JsonSerializer.Deserialize(file.ReadString(Encoding.UTF8), JsonTypeInfo);
		return petalModList;
	}

	public static PetalModList FromParams(params string[] modIDs)
	{
		return new PetalModList
		{
			Mods = new List<string>(modIDs)
		};
	}

	[JsonPropertyName("name"), JsonInclude]
	public string Name
	{
		get;
		set;
	} = "Unnamed";

	[JsonPropertyName("description"), JsonInclude]
	public string Description
	{
		get;
		set;
	} = string.Empty;

	[JsonPropertyName("mods"), JsonInclude]
	public List<string> Mods
	{
		get;
		set;
	} = [];
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(PetalModList))]
internal partial class PetalModListJsonTypeInfo : JsonSerializerContext
{

}
