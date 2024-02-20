using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;
using Petal.Framework.Util;

namespace Petal.Framework.Scenery.Nodes;

public sealed class Canvas : Node, IDisposable
{
	private Map<Color> _colorMap;

	public Map<Color> ColorMap
	{
		get => _colorMap;
		set => _colorMap = value;
	}

	private readonly Texture2D _texture;
	private readonly GraphicsDevice _graphicsDevice;

	public Canvas(Vector2Int dimensions, GraphicsDevice graphicsDevice)
	{
		ColorMap = new Map<Color>(dimensions);
		ColorMap.Populate(() => Color.Black);

		_graphicsDevice = graphicsDevice;
		_texture = new Texture2D(graphicsDevice, dimensions.X, dimensions.Y);
	}

	~Canvas()
	{
		Dispose();
	}

	public void ApplyColorMap()
	{
		_texture.SetData(ColorMap.ToArray());
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_texture?.Dispose();
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null)
			return;

		Scene.Renderer.Begin();

		Scene.Renderer.Draw(new RenderData
		{
			Texture = _texture,
			DstRect = AbsoluteBounds,
			Color = Color.White
		});

		Scene.Renderer.End();
	}
}
