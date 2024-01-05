using System;
using System.IO;
using System.Text;
using Hgm.Game.Systems;
using Hgm.Game.WorldGeneration;
using Hgm.Vanilla;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util.Extensions;

namespace Hgm.Game.Scenes;

public sealed class MainMenuScene : Scene
{
	public Background BackgroundSplash
	{
		get;
		private set;
	}

	public Text Title
	{
		get;
		private set;
	}

	public Panel ButtonPanel
	{
		get;
		private set;
	}

	public Button SingleplayerButton
	{
		get;
		private set;
	}

	public Text SingleplayerButtonText
	{
		get;
		private set;
	}

	public Button QuitButton
	{
		get;
		private set;
	}

	public Text QuitButtonText
	{
		get;
		private set;
	}

	public MainMenuScene()
	{
		Name = "hgm:main_menu_scene".ToNamespaced();
		ViewportAdapter = new BoxingViewportAdapter(
			Game.GraphicsDevice,
			Game.Window,
			new Vector2Int(320, 180));
	}

	protected override void OnLoad()
	{
		var assetsRegister = HedgemenVanilla.Instance.Registers.Assets;

		Skin = Skin.FromJson(new FileInfo("vanilla_skin.json").ReadString(Encoding.UTF8), assetsRegister);

		BackgroundSplash = Root.Add(new Background
		{
			Image = assetsRegister.MakeReference<Texture2D>("hgm:ui/splash_texture")
		});

		Title = Root.Add(new Text
		{
			Font = Skin.Font.LargeFont,
			Bounds = new Rectangle(0, 16, 32, 12),
			Color = Color.White,
			Message = "Hedgemen!",
			Scale = 0.375f,
			Anchor = Anchor.Top
		});

		ButtonPanel = Root.Add(new Panel(Skin)
		{
			Anchor = Anchor.Center,
			Bounds = new Rectangle(0, 0, 100, 36)
		});

		SingleplayerButton = ButtonPanel.Add(new Button(Skin)
		{
			Anchor = Anchor.Top,
			Bounds = new Rectangle(0, 2, 96, 16)
		});

		SingleplayerButton.OnMousePressed += SingleplayerButtonOnMousePressed;

		SingleplayerButtonText = SingleplayerButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = Skin.Font.MediumFont,
			Message = "Singleplayer",
			Outline = new Vector2(2.0f, 2.0f),
			Scale = 0.15f
		});

		QuitButton = ButtonPanel.Add(new Button(Skin)
		{
			Anchor = Anchor.Top,
			Bounds = new Rectangle(0, 18, 96, 16)
		});

		QuitButton.OnMousePressed += QuitButtonOnMousePressed;

		QuitButtonText = QuitButton.Add(new Text
		{
			Anchor = Anchor.Center,
			Font = Skin.Font.MediumFont,
			Message = "Quit",
			Outline = new Vector2(2.0f, 2.0f),
			Scale = 0.15f
		});
	}

	private void SingleplayerButtonOnMousePressed(object? sender, EventArgs args)
	{
		if (Game is not Hedgemen hedgemen)
			return;

		var cartographers = HedgemenVanilla.Instance.Registers.Cartographers;

		if (!cartographers.GetItem(new NamespacedString("hgm:overworld_cartographer"), out Cartographer cartographer))
			return;

		CampaignSystem.StartCampaign(new CampaignStartArgs
		{
			Hedgemen = hedgemen,
			ModList = PetalModList.FromParams("hgm:mod", "example:mod"),
			SessionDirectory = new DirectoryInfo("debug_save_path"),
			StartingWorldCartographer = cartographer
		});
	}

	private void QuitButtonOnMousePressed(object? sender, EventArgs args)
	{
		Game.Exit();
	}
}
