using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Petal.Framework.Content;
using Petal.Framework.IO;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Petal.Framework.Assets;

public class AssetManifest : IBankManifest
{
	private readonly List<AssetManifestEntry> _entries = new();

	public IReadOnlyList<AssetManifestEntry> Entries
		=> _entries;

	public AssetManifest()
	{

	}

	public AssetManifest(PersistentData storage)
	{
		var entries = storage.ReadField<List<PersistentData>>("entries");

		foreach (var entryStorage in entries)
		{
			var entry = new AssetManifestEntry
			{
				Name = entryStorage.ReadField<string>("name").ToNamespaced(),
				Path = entryStorage.ReadField<string>("path"),
				Type = entryStorage.ReadField<AssetType>("type"),
			};

			_entries.Add(entry);
		}
	}

	public void ForwardToRegister(IRegister register, AssetLoader assetLoader)
	{
		foreach (var assetEntry in Entries)
		{
			bool success = register.AddKey(assetEntry.Name, assetEntry.LoadAsset(assetLoader));

			if (success)
				continue;

			string message = $"Could not forward {assetEntry.Name} to {register.RegistryName}";
			register.Registry.Logger.Error(message);
		}
	}
	public void ForwardToRegister(IRegister register)
		=> ForwardToRegister(register, PetalGame.Petal.Assets);
}

public readonly struct AssetManifestEntry
{
	public required NamespacedString Name
	{
		get;
		init;
	}

	public required string Path
	{
		get;
		init;
	}

	public required AssetType Type
	{
		get;
		init;
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
