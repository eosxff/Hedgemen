using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
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

	private Skin _skin;

	protected override void Initialize()
	{
		base.Initialize();

		ContentRegistry.Register(
			"hedgemen:ui/button_hover_texture",
			Assets.LoadAsset<Texture2D>("button_hover.png"));
		
		ContentRegistry.Register(
			"hedgemen:ui/button_input_texture",
			Assets.LoadAsset<Texture2D>("button_input.png"));

		ContentRegistry.Register(
			"hedgemen:ui/button_normal_texture",
			Assets.LoadAsset<Texture2D>("button_normal.png"));

		_whiteSquare = Assets.LoadAsset<Texture2D>("white_square.png");
		_peach = Assets.LoadAsset<Texture2D>("peach.png");
		
		ContentRegistry.Register(
			"hedgemen:white_square",
			_whiteSquare);
		
		_skin = Skin.FromJson(new FileInfo("skin.json").ReadString(Encoding.UTF8), ContentRegistry);
		Console.WriteLine(_skin.Button.NormalTexture.ContentIdentifier);
		Console.WriteLine(_skin.Button.HoverTexture.ContentIdentifier);
		Console.WriteLine(_skin.Button.InputTexture.ContentIdentifier);

		_skin.Button.NormalTexture.ReloadItem(ContentRegistry);

		try
		{
			ContentRegistry.Get<Skin>("hedgemen:ui/button_normal_texture");
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}

		Console.WriteLine(ContentRegistry.Get("hedgemen:ui/button_input_texture"));

		var scene = new Scene(new Stage(), _skin)
		{
			BackgroundColor = Color.CornflowerBlue,
			ViewportAdapter = new BoxingViewportAdapter(
				GraphicsDevice,
				Petal.Window,
				new Vector2Int(640, 360))
		};
		
		scene.Root.OnChildAdded += (sender, args) =>
		{
			Console.WriteLine($"Added: {args.Child.Name}");
		};

		var buttonSize = new Vector2Int(48, 96);

		var button = scene.Root.Add(new Button(_skin)
		{
			Bounds = new Rectangle(16, -16, buttonSize.X, buttonSize.Y),
			Color = Color.White,
			Anchor = Anchor.BottomLeft,
			Name = "hedgemen:button"
		});
		
		var button2 = scene.Root.Add(new Button(_skin)
		{
			Bounds = new Rectangle(button.Bounds.X + button.Bounds.Width + 8, -16, buttonSize.X, buttonSize.Y),
			Color = Color.White,
			Anchor = Anchor.BottomLeft,
			Name = "hedgemen:button"
		});
		
		var button3 = scene.Root.Add(new Button(_skin)
		{
			Bounds = new Rectangle(button2.Bounds.X + button2.Bounds.Width + 8, -16, buttonSize.X, buttonSize.Y),
			Color = Color.White,
			Anchor = Anchor.BottomLeft,
			Name = "hedgemen:button"
		});
		
		var button4 = scene.Root.Add(new Button(_skin)
		{
			Bounds = new Rectangle(button3.Bounds.X + button3.Bounds.Width + 8, -16, buttonSize.X, buttonSize.Y),
			Color = Color.White,
			Anchor = Anchor.BottomLeft,
			Name = "hedgemen:button"
		});

		button4.Name = "hedgemen:cool_button";
		
		Console.WriteLine($"Button4: {scene.Root.Find(button4.Name)?.Name}");
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
			IsMouseVisible = true,
			IsWindowUserResizable = true
		};
	}
}