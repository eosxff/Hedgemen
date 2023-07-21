using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
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
		Skin.Refresh();
	}

	protected override void OnDraw(GameTime time)
	{
		if (Scene is null || Skin is null)
			return;

		var textureRef = GetButtonTextureFromState();

		if (!textureRef.HasValidKey)
			return;

		Scene.Renderer.Begin();

		// todo testing purposes

		var renderData = new RenderData
		{
			Color = Color,
			DstRect = AbsoluteBounds,
			Texture = textureRef.Get(),
			SrcRect = textureRef.Get().Bounds
		};

		const int xPadding = 4, yPadding = 4;
		const int leftPadding = xPadding, rightPadding = xPadding, topPadding = yPadding, bottomPadding = yPadding;

		var sourcePatch = new NinePatch(
			renderData.SrcRect.Value,
			leftPadding,
			rightPadding,
			topPadding,
			bottomPadding);

		var destinationPatch = new NinePatch(
			renderData.DstRect,
			leftPadding,
			rightPadding,
			topPadding,
			bottomPadding);

		Scene.Renderer.DrawNinePatch(renderData, sourcePatch, destinationPatch);

		Scene.Renderer.End();
	}

	private RegistryObject<Texture2D> GetButtonTextureFromState()
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
