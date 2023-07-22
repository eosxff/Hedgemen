using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Petal.Framework.Content;

namespace Petal.Framework.Assets;

[Serializable]
public sealed class AssetManifest
{
	public static AssetManifest FromJson(string json)
	{
		return JsonSerializer.Deserialize(json, AssetManifestJsc.Default.AssetManifest);
	}

	public static AssetManifest FromFile(FileInfo file)
	{
		return JsonSerializer.Deserialize(file.Open(FileMode.Open), AssetManifestJsc.Default.AssetManifest);
	}

	public static AssetManifest FromFile(string filePath)
	{
		var file = new FileInfo(filePath);
		return JsonSerializer.Deserialize(file.Open(FileMode.Open), AssetManifestJsc.Default.AssetManifest);
	}

	[JsonPropertyName("entries"), JsonInclude]
	public List<AssetManifestEntry> Assets
	{
		get;
		set;
	}

	public AssetManifest() : this(0)
	{

	}

	public AssetManifest(int initialCapacity)
	{
		Assets = new List<AssetManifestEntry>(initialCapacity);
	}

	public void ForwardToRegister(IRegister assetRegister, AssetLoader assetLoader)
	{
		foreach (var assetEntry in Assets)
		{
			bool success = assetRegister.AddKey(assetEntry.Name, assetEntry.LoadAsset(assetLoader));

			if (!success)
			{
				string message = $"Could not forward " + $"{assetEntry.Name} to {assetRegister.RegistryName}";
				assetRegister.Registry.Logger.Error(message);
			}
		}
	}
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AssetManifest))]
public partial class AssetManifestJsc : JsonSerializerContext
{

}
