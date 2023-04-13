using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Hedgemen Instance
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		Instance = this;
	}

	public ContentBank GameContent
	{
		get;
	} = new();

	protected override void Initialize()
	{
		base.Initialize();

		var resource1 = GameContent.Register(
			"hedgemen:ui/button_down_texture",
			Assets.LoadAsset<Texture2D>("button_down.png"));
		GameContent.Register(
			"hedgemen:ui/button_hover_texture",
			Assets.LoadAsset<Texture2D>("button_hover.png"));
		GameContent.Register(
			"hedgemen:ui/button_regular_texture",
			Assets.LoadAsset<Texture2D>("button_regular.png"));

		var skin = new Skin
		{
			Button = new Skin.ButtonData
			{
				ButtonDownTexture = GameContent.Get<Texture2D>("hedgemen:ui/button_down_texture"),
				ButtonHoverTexture = GameContent.Get<Texture2D>("hedgemen:ui/button_hover_texture"),
				ButtonRegularTexture = GameContent.Get<Texture2D>("hedgemen:ui/button_regular_texture"),
			}
		};
		
		
		var scene = new Scene(new Stage
		{
			
		}, skin)
		{
			BackgroundColor = Color.CornflowerBlue,
			ResolutionPolicy = SceneResolutionPolicy.ExactFit
		};

		var button = scene.Root.Add(new Button(skin)
		{
			Bounds = new Rectangle(50, 50, 150, 50),
			Color = Color.White
		});

		ChangeScenes(scene);
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 1200,
			WindowHeight = 700,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}
	
	
}