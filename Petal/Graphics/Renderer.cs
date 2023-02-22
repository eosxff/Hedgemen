using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public abstract class Renderer : IDisposable
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
	public abstract void Dispose();
}

public struct RenderData
{
	public Texture2D Texture
	{
		get;
		init;
	} = null;

	public Rectangle? SrcRect
	{
		get;
		init;
	} = null;

	public Rectangle DstRect
	{
		get;
		init;
	} = new(0, 0, 1, 1);

	public Color Color
	{
		get;
		init;
	} = Color.White;

	public float Rotation
	{
		get;
		init;
	} = 0.0f;

	public Vector2 Origin
	{
		get;
		init;
	} = Vector2.Zero;

	public SpriteEffects SpriteEffects
	{
		get;
		init;
	} = SpriteEffects.None;

	public float LayerDepth
	{
		get;
		init;
	} = 0.0f;

	public RenderData()
	{
		
	}
}

public struct RenderStringData
{
	public string Text
	{
		get;
		init;
	} = string.Empty;

	public SpriteFont Font
	{
		get;
		init;
	} = null;

	public Vector2 Position
	{
		get;
		init;
	} = Vector2.Zero;

	public Color Color
	{
		get;
		init;
	} = Color.White;

	public float Rotation
	{
		get;
		init;
	} = 1.0f;

	public Vector2 Origin
	{
		get;
		init;
	} = new(0.0f, 0.0f);

	public float Scale
	{
		get;
		init;
	} = 1.0f;

	public SpriteEffects SpriteEffects
	{
		get;
		init;
	} = SpriteEffects.None;

	public float LayerDepth
	{
		get;
		init;
	} = 0.0f;
	
	public RenderStringData()
	{
		
	}
}