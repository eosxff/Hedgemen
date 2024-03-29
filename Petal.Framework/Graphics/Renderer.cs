﻿using System;
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

public struct RenderData
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
	} = null;

	public required Rectangle DstRect
	{
		get;
		init;
	}

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

public struct RenderStringData
{
	public string Text
	{
		get;
		set;
	} = string.Empty;

	public SpriteFont Font
	{
		get;
		set;
	} = null;

	public Vector2 Position
	{
		get;
		set;
	} = Vector2.Zero;

	public Color Color
	{
		get;
		set;
	} = Color.White;

	public float Rotation
	{
		get;
		set;
	} = 0.0f;

	public Vector2 Origin
	{
		get;
		set;
	} = new(0.0f, 0.0f);

	public float Scale
	{
		get;
		set;
	} = 1.0f;

	public SpriteEffects SpriteEffects
	{
		get;
		set;
	} = SpriteEffects.None;

	public float LayerDepth
	{
		get;
		set;
	} = 0.0f;

	public RenderStringData()
	{

	}
}
