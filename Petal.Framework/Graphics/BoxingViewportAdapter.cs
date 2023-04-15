using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

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
        base.Dispose();
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

        var worldScaleX = (float)clientBounds.Width / VirtualResolution.X;
        var worldScaleY = (float)clientBounds.Height / VirtualResolution.Y;

        var safeScaleX = (float)clientBounds.Width / (VirtualResolution.X - Bleed.X);
        var safeScaleY = (float)clientBounds.Height / (VirtualResolution.Y - Bleed.Y);

        var worldScale = MathHelper.Max(worldScaleX, worldScaleY);
        var safeScale = MathHelper.Min(safeScaleX, safeScaleY);
        var scale = MathHelper.Min(worldScale, safeScale);

        var width = (int)(scale * VirtualResolution.X + 0.5f);
        var height = (int)(scale * VirtualResolution.Y + 0.5f);

        if (height >= clientBounds.Height && width < clientBounds.Width)
            BoxingMode = BoxingMode.PillarBox;
        else
        {
            if (width >= clientBounds.Height && height <= clientBounds.Height)
                BoxingMode = BoxingMode.LetterBox;
            else
                BoxingMode = BoxingMode.None;
        }

        var x = clientBounds.Width / 2 - width / 2;
        var y = clientBounds.Height / 2 - height / 2;
        GraphicsDevice.Viewport = new Viewport(x, y, width, height);
    }
}