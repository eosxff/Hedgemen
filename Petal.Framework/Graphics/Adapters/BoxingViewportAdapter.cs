using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics.Adapters;

public enum BoxingMode
{
	PillarBox,
	LetterBox,
	None
}

public class BoxingViewportAdapter : ScalingViewportAdapter
{
	public GameWindow Window
	{
		get;
	}

	public Vector2Int Bleed
	{
		get;
	}

	public BoxingMode BoxingMode
	{
		get;
		private set;
	}

	public BoxingViewportAdapter(
		GraphicsDevice graphicsDevice,
		GameWindow window,
		Vector2Int virtualResolution,
		Vector2Int? bleed = null)
		: base(graphicsDevice, virtualResolution)
	{
		bleed ??= Vector2Int.Zero;

		SetVirtualResolution(virtualResolution);
		Bleed = bleed.Value;
		Window = window;
		Window.ClientSizeChanged += OnClientSizeChanged;

		OnClientSizeChanged(this, EventArgs.Empty);
	}

	~BoxingViewportAdapter()
	{
		Dispose();
	}

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		Window.ClientSizeChanged -= OnClientSizeChanged;
	}

	public override void Reset()
	{
		base.Reset();
		OnClientSizeChanged(this, EventArgs.Empty);
	}

	public override Point PointToScreen(int x, int y)
	{
		var viewport = GraphicsDevice.Viewport;
		return base.PointToScreen(x - viewport.X, y - viewport.Y);
	}

	private void OnClientSizeChanged(object sender, EventArgs eventArgs)
	{
		var clientBounds = Window.ClientBounds;

		var worldScale = new Vector2(
			(float)clientBounds.Width / VirtualResolution.X,
			(float)clientBounds.Height / VirtualResolution.Y);

		var safeScale = new Vector2(
			(float)clientBounds.Width / (VirtualResolution.X - Bleed.X),
			(float)clientBounds.Height / (VirtualResolution.Y - Bleed.Y));

		float worldScale1d = MathHelper.Max(worldScale.X, worldScale.Y);
		float safeScale1d = MathHelper.Min(safeScale.X, safeScale.Y);
		float scale = MathHelper.Min(worldScale1d, safeScale1d);

		var dimensions = new Vector2Int(
			(int)(scale * VirtualResolution.X + 0.5f),
			(int)(scale * VirtualResolution.Y + 0.5f));

		if (dimensions.Y >= clientBounds.Height && dimensions.X < clientBounds.Width)
		{
			BoxingMode = BoxingMode.PillarBox;
		}
		else
		{
			if (dimensions.X >= clientBounds.Height && dimensions.Y <= clientBounds.Height)
				BoxingMode = BoxingMode.LetterBox;
			else
				BoxingMode = BoxingMode.None;
		}

		int x = clientBounds.Width / 2 - dimensions.X / 2;
		int y = clientBounds.Height / 2 - dimensions.Y / 2;

		GraphicsDevice.Viewport = new Viewport(x, y, dimensions.X, dimensions.Y);
	}
}