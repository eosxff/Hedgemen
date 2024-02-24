using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Assets;

public sealed class AssetLoader(GraphicsDevice graphicsDevice, ILogger logger) : IDisposable
{
	public delegate void OnAssetLoaded(object asset);

	public delegate void OnAssetUnknownType(Type type);

	public event OnAssetLoaded? AssetLoaded;
	public event OnAssetUnknownType? AssetUnknownType;

	private readonly GraphicsDevice _graphicsDevice = graphicsDevice;
	private readonly ContentManager _contentManager = new AssetLoaderInternalContentManager(PetalGame.Petal.Services);
	private readonly ILogger _logger = logger;

	public GraphicsDevice GraphicsDevice
		=> _graphicsDevice;

	public T LoadAsset<T>(Stream stream)
	{
		var assetType = typeof(T);

		if (assetType == typeof(Texture2D))
		{
			object asset = Texture2D.FromStream(_graphicsDevice, stream);
			AssetLoaded?.Invoke(asset);
			stream.Close();
			return (T)asset;
		}

		if (assetType == typeof(SoundEffect))
		{
			object asset = SoundEffect.FromStream(stream);
			AssetLoaded?.Invoke(asset);
			stream.Close();
			return (T)asset;
		}

		AssetUnknownType?.Invoke(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with '{nameof(stream)}'.");
	}

	public T LoadAsset<T>(string name, Uri uri)
	{
		var assetType = typeof(T);

		if (assetType == typeof(Song))
		{
			object asset = Song.FromUri(name, uri);
			AssetLoaded?.Invoke(asset);
			return (T)asset;
		}

		AssetUnknownType?.Invoke(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with " +
		                            $"'{nameof(name)}: {name} | '{nameof(uri)}: {uri}'.");
	}

	public T LoadAsset<T>(byte[] assetBytes)
	{
		var assetType = typeof(T);

		if (assetType == typeof(Effect))
		{
			object asset = new Effect(_graphicsDevice, assetBytes);
			AssetLoaded?.Invoke(asset);
			return (T)asset;
		}

		AssetUnknownType?.Invoke(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with " +
		                            $"'{nameof(assetBytes)}: {assetBytes}'.");
	}

	public T LoadAsset<T>(string path)
	{
		var assetType = typeof(T);

		if (assetType == typeof(SpriteFont))
		{
			object asset = _contentManager.Load<SpriteFont>(path);
			AssetLoaded?.Invoke(asset);
			return (T)asset;
		}

		if (assetType == typeof(Texture2D))
		{
			var file = new FileInfo(path);
			var stream = file.Open(FileMode.Open);

			object asset = Texture2D.FromStream(_graphicsDevice, stream);
			AssetLoaded?.Invoke(asset);
			stream.Close();
			return (T)asset;
		}

		AssetUnknownType?.Invoke(assetType);
		throw new ArgumentException($"Type '{assetType}' can not be loaded with " +
		                            $"'{nameof(path)}: {path}'.");
	}

	public void Unload()
	{
		_contentManager?.Unload();
	}

	public void Dispose()
	{
		_contentManager?.Dispose();
	}

	private class AssetLoaderInternalContentManager : ContentManager
	{
		public AssetLoaderInternalContentManager(IServiceProvider serviceProvider)
			: base(serviceProvider, string.Empty)
		{
		}

		public AssetLoaderInternalContentManager(IServiceProvider serviceProvider, string rootDirectory)
			: base(serviceProvider, rootDirectory)
		{
		}

		protected override void Dispose(bool disposing)
		{
		}

		public override void Unload()
		{
		}
	}
}
