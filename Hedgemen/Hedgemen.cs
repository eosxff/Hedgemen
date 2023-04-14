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

	public ContentRegistry ContentRegistry
	{
		get;
	} = new();

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
		};

		/*var button = scene.Root.Add(new Button(skin)
		{
			//Bounds = new Rectangle(50, 50, 150, 50),
			Bounds = new Rectangle(0, 0, 640, 360),
			Color = Color.White
		});*/

		var image = scene.Root.Add(new Image
		{
			Bounds = new Rectangle(0, 0, 640, 360),
			Texture = ContentRegistry.Get<Texture2D>("hedgemen:ui/button_down_texture").Item
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