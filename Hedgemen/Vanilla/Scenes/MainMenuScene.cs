using System;
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

		SingleplayerButton = Root.Add(new Button(Skin)
		{
			Anchor = Anchor.Center,
			Bounds = new Rectangle(0, -16, 96, 16)
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

		QuitButton = Root.Add(new Button(Skin)
		{
			Anchor = Anchor.Center,
			Bounds = new Rectangle(0, 1, 96, 16)
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

		if (!hedgemen.Registry.GetRegister("hgm:campaigns", out Register<Supplier<Campaign>> campaignRegister))
		{
			Game.Logger.Error($"Could not find hgm:campaigns register.");
			return;
		}

		var campaignRO = campaignRegister.MakeReference("hgm:hedgemen_campaign");

		var campaign = (HedgemenCampaign)campaignRO.Supply<Campaign>();
		campaign.StartCampaign();
	}

	private void QuitButtonOnMousePressed(object? sender, EventArgs args)
	{
		Game.Exit();
	}
}
