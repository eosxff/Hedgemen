using System;
using System.IO;
using System.Text;
using System.Text.Json;
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

		Window.AllowUserResizing = true;

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

		var scene = new Scene(new Stage
		{
			
		}, _skin)
		{
			BackgroundColor = Color.CornflowerBlue,
			ViewportAdapter = new BoxingViewportAdapter(
				GraphicsDevice,
				Petal.Window,
				new Vector2Int(640, 360))
		};

		var button = scene.Root.Add(new Button(_skin)
		{
			Bounds = new Rectangle(0, 0, 150, 50),
			Color = Color.White,
		});

		button.OnMousePressed += (sender, args) =>
		{
			var file = new FileInfo("test_file.json");
			var record = new Skin.DataRecord();
			record.Read(Scene.Skin);
			
			file.WriteString(JsonSerializer.Serialize(
				record,
				Skin.DataRecord.JsonDeserializeOptions), Encoding.UTF8, FileMode.OpenOrCreate);
		};
		
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