using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util;

namespace Hgm.Vanilla.Scenes;

public sealed class MainMenuScene : Scene
{
	private readonly Register<object> _assetsRegister;

	public MainMenuScene(Register<object> assetsRegister)
	{
		_assetsRegister = assetsRegister;
		Skin = Skin.FromJson(new FileInfo("main_menu_skin.json").ReadString(Encoding.UTF8), assetsRegister);

		Name = "hgm:main_menu_scene".ToNamespaced();

		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320, 180));
	}

	protected override void OnInitialize()
	{
		var assetsRegister = HedgemenVanilla.Instance.Registers.Assets;

		Root.Add(new Background
		{
			Image = assetsRegister.MakeReference<Texture2D>("hgm:ui/splash_texture")
		});

		Root.Add(new Text
		{
			Font = Skin.Font.LargeFont,
			Bounds = new Rectangle(16, 16, 32, 12),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 0.375f
		});

		var startButton = Root.Add(new Button(Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(16, -25, 48, 16)
		});

		startButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = Skin.Font.MediumFont,
			Message = "Singleplayer",
			Outline = new Vector2(2.0f, 2.0f),
			Scale = 0.15f
		});

		var exitButton = Root.Add(new Button(Skin)
		{
			Anchor = Anchor.CenterLeft,
			Bounds = new Rectangle(16, 0, 48, 16)
		});

		exitButton.OnMousePressed += (sender, args) =>
		{
			Game.Exit();
		};

		exitButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = Skin.Font.MediumFont,
			Message = "Exit",
			Outline = new Vector2(2.0f, 2.0f),
			Scale = 0.15f
		});
	}
}
