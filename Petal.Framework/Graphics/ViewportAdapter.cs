using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Util;

namespace Petal.Framework.Graphics;

public abstract class ViewportAdapter : IDisposable
{
	public GraphicsDevice GraphicsDevice
	{
		get;
	}

	private Vector2Int _virtualResolution;

	public virtual Vector2Int VirtualResolution
		=> _virtualResolution;

	private Vector2Int _viewportResolution;

	public Vector2Int ViewportResolution
	{
		get => _viewportResolution;
		protected set
		{
			_viewportResolution = value;
		}
	}

	public Rectangle BoundingRectangle
		=> new(0, 0, VirtualResolution.X, VirtualResolution.Y);

	public Point Center
		=> BoundingRectangle.Center;

	public virtual Vector2 GetScale()
		=> new(ViewportResolution.X, ViewportResolution.Y);

	protected ViewportAdapter(GraphicsDevice graphicsDevice)
	{
		GraphicsDevice = graphicsDevice;

		var graphicsDeviceViewportSize = new Vector2Int(
			graphicsDevice.Viewport.Width,
			graphicsDevice.Viewport.Height);

		ViewportResolution = graphicsDeviceViewportSize;
		SetVirtualResolution(graphicsDeviceViewportSize);
	}

	/// <summary>
	/// Used in situations where virtual resolution is dynamic or being set for the first time.
	/// </summary>
	protected void SetVirtualResolution(Vector2Int resolution)
	{
		_virtualResolution = resolution;
	}

	public virtual Matrix GetScaleMatrix()
	{
		return Matrix.Identity;
	}

	public Vector2 PointToScreen(Point point)
	{
		return PointToScreen(point.X, point.Y);
	}

	public virtual Vector2 PointToScreen(float x, float y)
	{
		var scaleMatrix = GetScaleMatrix();
		var inverseMatrix = Matrix.Invert(scaleMatrix);
		return Vector2.Transform(new Vector2(x, y), inverseMatrix);
	}

	public virtual void Reset()
	{
		ViewportResolution = new Vector2Int(
			GraphicsDevice.Viewport.Width,
			GraphicsDevice.Viewport.Height);
	}

	~ViewportAdapter()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
