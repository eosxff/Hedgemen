using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Image : Node
{
	public static Rectangle DefaultBounds
	{
		get;
		set;
	} = new (0, 0, 32, 32);
	
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
		return DefaultBounds;
	}

	protected override void OnUpdate(GameTime time, NodeSelection selection)
	{
		
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