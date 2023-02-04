using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Graphics;

public abstract class Renderer
{
	public RendererState RenderState
	{
		get;
		protected set;
	}

	protected Renderer()
	{
		RenderState = new RendererState();
	}

	public abstract void Begin();
	public abstract void End();
	public abstract void Draw(RenderData data);
	public abstract void Draw(RenderStringData data);
}

public struct RenderData
{
	public Texture2D Texture
	{
		get;
		init;
	}

	public Rectangle? SrcRect
	{
		get;
		init;
	}

	public Rectangle DstRect
	{
		get;
		init;
	}

	public Color Color
	{
		get;
		init;
	}

	public float Rotation
	{
		get;
		init;
	}

	public Vector2 Origin
	{
		get;
		init;
	}

	public SpriteEffects SpriteEffects
	{
		get;
		init;
	}

	public float LayerDepth
	{
		get;
		init;
	}

	public RenderData()
	{
		Texture = null;
		SrcRect = null;
		DstRect = Rectangle.Empty;
		Color = Color.White;
		Rotation = 0.0f;
		Origin = Vector2.Zero;
		SpriteEffects = SpriteEffects.None;
		LayerDepth = 0;
	}
}

public struct RenderStringData
{
	public string Text
	{
		get;
		init;
	}

	public SpriteFont Font
	{
		get;
		init;
	}

	public Vector2 Position
	{
		get;
		init;
	}

	public Color Color
	{
		get;
		init;
	}
	
	public float Rotation
	{ 
		get;
		init;
	}

	public Vector2 Origin
	{
		get;
		init;
	}

	public float Scale
	{
		get;
		init;
	}

	public SpriteEffects SpriteEffects
	{
		get;
		init;
	}

	public float LayerDepth
	{
		get;
		init;
	}
	
	public RenderStringData()
	{
		Text = string.Empty;
		Font = null;
		Position = Vector2.Zero;
		Color = Color.White;
		Rotation = 0.0f;
		Origin = Vector2.Zero;
		Scale = 1.0f;
		SpriteEffects = SpriteEffects.None;
		LayerDepth = 0;
	}
}