using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Button : Node
{
	public Color Color
	{
		get;
		set;
	} = Color.White;
	
	public Skin Skin
	{
		get;
		set;
	}
	
	public Button(Skin skin)
	{
		Skin = skin;
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene == null || Skin == null)
			return;

		var textureRef = GetButtonTextureFromState();

		if (!textureRef.IsValid)
			return;
		
		Scene.Renderer.Begin();
		
		// todo ninepatching
		Scene.Renderer.Draw(new RenderData
		{
			Color = Color,
			DstRect = AbsoluteBounds,
			Texture = textureRef.Item
		});
		
		Scene.Renderer.End();
	}

	private ContentReference<Texture2D> GetButtonTextureFromState()
	{
		if (Skin == null)
			return null;

		switch (State)
		{
			case NodeState.Default:
				return Skin.Button.RegularTexture;
			case NodeState.Input:
				return Skin.Button.InputTexture;
			case NodeState.Hover:
				return Skin.Button.HoverTexture;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}