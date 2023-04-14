using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Util;

namespace Petal.Framework.Graphics;

public class ViewportAdapter : IDisposable
{
	public GraphicsDevice GraphicsDevice
	{
		get;
	}

	public Vector2Int VirtualResolution
	{
		get;
		protected set;
	}

	public Vector2Int ViewportResolution
	{
		get;
		protected set;
	}

	public Rectangle BoundingRectangle
		=> new(0, 0, VirtualResolution.X, VirtualResolution.Y);

	public Point Center
		=> BoundingRectangle.Center;

	public ViewportAdapter(GraphicsDevice graphicsDevice)
	{
		GraphicsDevice = graphicsDevice;
		VirtualResolution = new Vector2Int(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
		ViewportResolution = new Vector2Int(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
	}

	~ViewportAdapter()
	{
		Dispose();
	}
	
	public virtual Matrix GetScaleMatrix()
		=> Matrix.Identity;

	public Point PointToScreen(Point point)
		=> PointToScreen(point.X, point.Y);

	public virtual Point PointToScreen(int x, int y)
	{
		var scaleMatrix = GetScaleMatrix();
		var inverseMatrix = Matrix.Invert(scaleMatrix);
		return Vector2.Transform(new Vector2(x, y), inverseMatrix).ToPoint();
	}

	public virtual void Reset()
	{
		ViewportResolution = new Vector2Int(
			GraphicsDevice.Viewport.Width,
			GraphicsDevice.Viewport.Height);
	}

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
		GraphicsDevice?.Dispose();
	}
}