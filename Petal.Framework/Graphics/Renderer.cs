using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Petal.Framework.Graphics;

public abstract class Renderer : IDisposable
{
	public RendererState RenderState
	{
		get;
		protected set;
	} = new();

	public abstract void Begin();
	public abstract void End();
	public abstract void Draw(RenderData data);
	public abstract void DrawNinePatch(RenderData data, NinePatch sourcePatch, NinePatch destinationPatch);
	public abstract void Draw(RenderStringData data);
	public abstract void Dispose();
}

public readonly struct RenderData
{
	public required Texture2D Texture
	{
		get;
		init;
	}

	public Rectangle? SrcRect
	{
		get;
		init;
	}

	public required Rectangle DstRect
	{
		get;
		init;
	}

	public required Color Color
	{
		get;
		init;
	}

	public float Rotation
	{
		get;
		init;
	}

	public Vector2? Origin
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

	}

	[SetsRequiredMembers]
	public RenderData(RenderData data)
	{
		Texture = data.Texture;
		SrcRect = data.SrcRect;
		DstRect = data.DstRect;
		Color = data.Color;
		Rotation = data.Rotation;
		Origin = data.Origin;
		SpriteEffects = data.SpriteEffects;
		LayerDepth = data.LayerDepth;
	}
}

public readonly struct RenderStringData
{
	public required string Text
	{
		get;
		init;
	}

	public required SpriteFont Font
	{
		get;
		init;
	}

	public Vector2 Position
	{
		get;
		init;
	}

	public required Color Color
	{
		get;
		init;
	}

	public float Rotation
	{
		get;
		init;
	}

	public Vector2? Origin
	{
		get;
		init;
	}

	public float? Scale
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

	}
}
