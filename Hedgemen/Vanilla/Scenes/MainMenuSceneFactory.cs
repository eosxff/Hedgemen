using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Petal.Framework;
using Petal.Framework.Graphics.Adapters;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;

namespace Hgm.Vanilla.Scenes;

public static class MainMenuSceneFactory
{
	public static Scene NewScene(Hedgemen hedgemen, ContentRegistry contentRegistry)
	{
		var skin = Skin.FromJson(new FileInfo("skin.json").ReadString(Encoding.UTF8), contentRegistry);
		
		var scene = new Scene(
			new Stage(), skin)
		{
			BackgroundColor = new Color(232, 190, 198, 255),
			ViewportAdapter = new BoxingViewportAdapter(
				hedgemen.GraphicsDevice,
				hedgemen.Window,
				new Vector2Int(640, 360))
		};
		
		scene.Root.Add(new Text
		{
			Font = scene.Skin.Font.LargeFont,
			Bounds = new Rectangle(32, 32, 64, 24),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 0.75f
		});

		hedgemen.Logger.Critical($"{scene.Skin.Button.HoverTexture.ContentID}");
		var startButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(32, -56, 128, 40)
		});

		startButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Singleplayer",
			Scale = 0.5f
		});
		
		var exitButton = scene.Root.Add(new Button(scene.Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(32, 8, 128*2, 40*2)
		});

		exitButton.OnMousePressed += (sender, args) =>
		{
			hedgemen.Logger.Debug($"Exiting Hedgemen.");
			hedgemen.Exit();
		};
		
		exitButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = scene.Skin.Font.MediumFont,
			Message = "Exit",
			Scale = 0.5f
		});

		return scene;
	}
}