using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics.Adapters;

public class ScalingViewportAdapter : ViewportAdapter
{
	public ScalingViewportAdapter(GraphicsDevice graphicsDevice, Vector2Int virtualResolution)
		: base(graphicsDevice)
	{
		SetVirtualResolution(virtualResolution);
	}

	public override Vector2 GetScale()
	{
		return new Vector2(
			(float)ViewportResolution.X / VirtualResolution.X,
			(float)ViewportResolution.Y / VirtualResolution.Y);
	}

	public override Matrix GetScaleMatrix()
	{
		var scale = GetScale();
		return Matrix.CreateScale(scale.X, scale.Y, 1.0f);
	}
}