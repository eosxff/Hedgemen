﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Image : Node
{
	public Color Color
	{
		get;
		set;
	} = Color.White;
	
	public Texture2D Texture
	{
		get;
		set;
	}

	protected override Rectangle GetDefaultBounds()
	{
		return new Rectangle(0, 0, 125, 35);
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene == null || Texture == null)
			return;
		
		Scene.Renderer.Begin();
		
		Scene.Renderer.Draw(new RenderData
		{
			Texture = Texture,
			DstRect = AbsoluteBounds,
			Color = Color
		});
		
		Scene.Renderer.End();
	}
}