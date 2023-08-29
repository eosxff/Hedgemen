using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Graphics;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Vanilla.Scenes;

public static class SplashSceneFactory
{
	public static Scene NewScene(Hedgemen hedgemen, Stream backgroundStream)
	{
		var scene = new Scene(new Stage(), new Skin())
		{
			Name = "hgm:splash_scene",
			BackgroundColor = Color.Black,
			ViewportAdapter = new BoxingViewportAdapter(
				hedgemen.GraphicsDevice,
				hedgemen.Window,
				new Vector2Int(640, 360))
		};

		scene.Root.Add(new Image
		{
			Texture = hedgemen.Assets.LoadAsset<Texture2D>(backgroundStream),
			Anchor = Anchor.TopRight,
			Color = Color.White,
			Bounds = scene.ViewportAdapter.VirtualResolution.ToRectangleSize()
		});

		return scene;
	}
}
