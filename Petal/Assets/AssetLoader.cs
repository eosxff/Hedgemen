using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Petal.Assets;

public sealed class AssetLoader
{
	public delegate void OnAssetLoaded(object asset);
	public delegate void OnAssetUnknownType(Type type);

	public event OnAssetLoaded AssetLoaded = _ => { };
	public event OnAssetUnknownType AssetUnknownType = _ => { };

	private GraphicsDevice _graphicsDevice;

	public AssetLoader(GraphicsDevice graphicsDevice)
	{
		_graphicsDevice = graphicsDevice;
	}

	public T LoadAsset<T>(Stream stream)
	{
		var assetType = typeof(T);

		if (assetType == typeof(Texture2D))
		{
			object asset = Texture2D.FromStream(_graphicsDevice, stream);
			AssetLoaded(asset);
			stream.Close();
			return (T)asset;
		}

		if (assetType == typeof(SoundEffect))
		{
			object asset = SoundEffect.FromStream(stream);
			AssetLoaded(asset);
			stream.Close();
			return (T) asset;
		}
		
		AssetUnknownType(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with '{nameof(stream)}'.");
	}

	public T LoadAsset<T>(string name, Uri uri)
	{
		var assetType = typeof(T);

		if (assetType == typeof(Song))
		{
			object asset = Song.FromUri(name, uri);
			AssetLoaded(asset);
			return (T)asset;
		}
		
		AssetUnknownType(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with " +
		                            $"'{nameof(name)}, {uri}'.");
	}
}