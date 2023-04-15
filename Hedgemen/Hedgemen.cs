using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
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

	public ContentRegistry ContentRegistry
	{
		get;
	} = new();

	private Texture2D _whiteSquare;
	private Texture2D _peach;

	protected override void Initialize()
	{
		base.Initialize();

		Window.AllowUserResizing = true;

		var resource1 = ContentRegistry.Register(
			"hedgemen:ui/button_down_texture",
			Assets.LoadAsset<Texture2D>("button_down.png"));
		
		ContentRegistry.Register(
			"hedgemen:ui/button_hover_texture",
			Assets.LoadAsset<Texture2D>("button_hover.png"));
		
		ContentRegistry.Register(
			"hedgemen:ui/button_regular_texture",
			Assets.LoadAsset<Texture2D>("button_regular.png"));

		_whiteSquare = Assets.LoadAsset<Texture2D>("white_square.png");
		_peach = Assets.LoadAsset<Texture2D>("peach.png");
		
		ContentRegistry.Register(
			"hedgemen:white_square",
			_whiteSquare);

		var skin = new Skin
		{
			Button = new Skin.ButtonData
			{
				InputTexture = ContentRegistry.Get<Texture2D>("hedgemen:ui/button_down_texture"),
				HoverTexture = ContentRegistry.Get<Texture2D>("hedgemen:ui/button_hover_texture"),
				RegularTexture = ContentRegistry.Get<Texture2D>("hedgemen:ui/button_regular_texture"),
			}
		};
		
		skin.Button.RegularTexture.ReloadItem(ContentRegistry);
		
		Console.WriteLine(ContentRegistry.Get("hedgemen:ui/button_down_texture"));

		var scene = new Scene(new Stage
		{
			
		}, skin)
		{
			BackgroundColor = Color.CornflowerBlue,
			//ViewportAdapter = new ScalingViewportAdapter(GraphicsDevice, new Vector2Int(640, 360))
			ViewportAdapter = new BoxingViewportAdapter(
				GraphicsDevice,
				PetalGame.Petal.Window,
				new Vector2Int(640, 360))
		};

		var image = scene.Root.Add(new Button(skin)
		{
			Bounds = new Rectangle(0, 0, 150, 50),
			//Bounds = new Rectangle(0, 0, 640, 360),
			Color = Color.White,
		});

		ChangeScenes(scene);
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 960,
			WindowHeight = 540,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}
	
	
}