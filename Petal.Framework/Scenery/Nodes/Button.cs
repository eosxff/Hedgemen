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

	protected override void OnSceneSet()
	{
		if (Scene is null)
			return;

		Scene.OnSkinChanged += SceneOnSkinChanged;
	}

	protected override void Destroy()
	{
		if (Scene is not null)
			Scene.OnSkinChanged -= SceneOnSkinChanged;
		base.Destroy();
	}

	private void SceneOnSkinChanged(object? sender, Scene.SkinChangedEventArgs args)
	{
		Skin = args.NewSkin;
		Skin.Button.HoverTexture.ReloadItem(Skin.ContentRegistry);
		Skin.Button.InputTexture.ReloadItem(Skin.ContentRegistry);
		Skin.Button.NormalTexture.ReloadItem(Skin.ContentRegistry);
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null || Skin is null)
			return;

		var textureRef = GetButtonTextureFromState();

		if (!textureRef.HasItem)
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
		if (Skin is null)
			return null;

		switch (State)
		{
			case NodeState.Normal:
				return Skin.Button.NormalTexture;
			case NodeState.Input:
				return Skin.Button.InputTexture;
			case NodeState.Hover:
				return Skin.Button.HoverTexture;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}