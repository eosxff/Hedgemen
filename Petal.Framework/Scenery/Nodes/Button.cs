﻿using System;
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

	protected override void OnSceneSet()
	{
		if (Scene is null)
			return;

		Scene.OnSkinChanged += SceneOnSkinChanged;
	}

	protected internal override void Destroy()
	{
		if (Scene is not null)
			Scene.OnSkinChanged -= SceneOnSkinChanged;
		base.Destroy();
	}

	private void SceneOnSkinChanged(object? sender, EventArgs args)
	{
		if (args is not Scene.SkinChangedEventArgs e)
			return;
				
		Skin.Button.HoverTexture.ReloadItem(Skin.Registry);
		Skin.Button.InputTexture.ReloadItem(Skin.Registry);
		Skin.Button.RegularTexture.ReloadItem(Skin.Registry);
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