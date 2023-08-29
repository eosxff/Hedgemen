using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Content;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;

namespace Hgm.Vanilla.Scenes;

public static class MainMenuSceneFactory
{
	public static Scene NewScene(Hedgemen hedgemen, Register<object> assetsRegister)
	{
		var skin = Skin.FromJson(new FileInfo("skin.json").ReadString(Encoding.UTF8), assetsRegister);

		var scene = new Scene(
			new Stage(), skin)
		{
			Name = "hgm:main_menu_scene",
			BackgroundColor = Color.Black,
			ViewportAdapter = new BoxingViewportAdapter(
				hedgemen.GraphicsDevice,
				hedgemen.Window,
				//new Vector2Int(640, 360))
				new Vector2Int(320, 180))
		};

		scene.Root.Add(new Background
		{
			Image = assetsRegister.MakeReference<Texture2D>("hgm:ui/splash_texture")
		});

		scene.Root.Add(new Text
		{
			Font = scene.Skin.Font.LargeFont,
			Bounds = new Rectangle(16, 16, 32, 12),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 0.375f
		});

		var startButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(16, -25, 64, 21)
		});

		startButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Singleplayer",
			Scale = 0.25f
		});

		var exitButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(16, 0, 64, 21)
		});

		exitButton.OnMousePressed += (sender, args) =>
		{
			hedgemen.Exit();
		};

		exitButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Exit",
			Scale = 0.25f
		});

		return scene;
	}
}
