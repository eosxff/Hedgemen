using System;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Petal.Framework.IO;

namespace Petal.Framework.Assets;

[Serializable]
public struct AssetManifestEntry
{
	[JsonPropertyName("name"), JsonConverter(typeof(NamespacedString.JsonConverter)), JsonInclude]
	public NamespacedString Name
	{
		get;
		set;
	}

	[JsonPropertyName("path"), JsonInclude]
	public string Path
	{
		get;
		set;
	}

	[JsonPropertyName("type"), JsonInclude]
	public AssetType Type
	{
		get;
		set;
	}

	public AssetManifestEntry()
	{

	}

	[JsonConstructor]
	private AssetManifestEntry(NamespacedString name, string path, AssetType type)
	{
		Name = name;
		Path = path;
		Type = type;
	}

	public object LoadAsset(AssetLoader loader)
	{
		switch (Type)
		{
			case AssetType.Texture:
				return loader.LoadAsset<Texture2D>(new FileInfo(Path).Open(FileMode.Open));
			case AssetType.Font:
				return loader.LoadAsset<SpriteFont>(Path);
			case AssetType.Effect:
				return new Effect(loader.GraphicsDevice, new FileInfo(Path).ReadBytes());
			case AssetType.SoundEffect:
				return SoundEffect.FromStream(new FileInfo(Path).Open(FileMode.Open));
			case AssetType.Song:
				return Song.FromUri(Path, new Uri($"file://{Path}"));
			case AssetType.None:
				throw new InvalidOperationException($"Can't load asset type {Type}.");
			default:
				throw new ArgumentOutOfRangeException(Type.ToString());
		}
	}
}
