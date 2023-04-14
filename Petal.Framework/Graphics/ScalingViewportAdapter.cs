using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public class ScalingViewportAdapter : ViewportAdapter
{
    public ScalingViewportAdapter(GraphicsDevice graphicsDevice, Vector2Int virtualResolution)
        : base(graphicsDevice)
    {
        VirtualResolution = virtualResolution;
    }

    public override Matrix GetScaleMatrix()
    {
        var scale = new Vector2(
            (float)ViewportResolution.X / VirtualResolution.X,
            (float)ViewportResolution.Y / VirtualResolution.Y);
        
        return Matrix.CreateScale(scale.X, scale.Y, 1.0f);
    }
}