using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Graphics;

namespace Petal.Framework.Scenery.Nodes;

public class Panel : Node
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

	public Panel(Skin skin)
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

		var textureRef = Skin.Panel.PanelTexture;

		if (!textureRef.IsPresent)
			return;

		Scene.Renderer.Begin();

		var renderData = new RenderData
		{
			Color = Color,
			DstRect = AbsoluteBounds,
			Texture = textureRef.Get(),
			SrcRect = textureRef.Get().Bounds
		};

		int leftPadding = Skin.Panel.BorderPadding;
		int rightPadding = Skin.Panel.BorderPadding;
		int topPadding = Skin.Panel.BorderPadding;
		int bottomPadding = Skin.Panel.BorderPadding;

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
}
