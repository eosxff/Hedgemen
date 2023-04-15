using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public class DefaultViewportAdapter : ViewportAdapter
{
    public GameWindow Window
    {
        get;
    }
    
    public DefaultViewportAdapter(GraphicsDevice graphicsDevice, GameWindow window) : base(graphicsDevice)
    {
        Window = window;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    public override Vector2Int VirtualResolution 
        => new (Window.ClientBounds.Width, Window.ClientBounds.Height);

    ~DefaultViewportAdapter()
    {
        Dispose();
    }
    
    public override void Dispose()
    {
        base.Dispose();
        Window.ClientSizeChanged -= OnClientSizeChanged;
    }

    private void OnClientSizeChanged(object sender, EventArgs eventArgs)
    {
        var width = Window.ClientBounds.Width;
        var height = Window.ClientBounds.Height;
        
        GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
    }
}