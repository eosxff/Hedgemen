using System;
using System.IO;
using System.Text;
using Hgm.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Hedgemen Instance { get; private set; }

	public Hedgemen()
	{
		Instance = this;
	}

	public ContentRegistry ContentRegistry { get; } = new();

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

		scene.Root.OnChildAdded += (sender, args) => { Console.WriteLine($"Added: {args.Child.Name}"); };

		var buttonSize = new Vector2Int(32, 32);
		var anchors = Enum.GetValues<Anchor>();

		for (int i = 0; i < anchors.Length; ++i)
		{
			var button = scene.Root.Add(new Button(_skin)
			{
				Bounds = new Rectangle(8, 8, buttonSize.X, buttonSize.Y),
				Color = Color.White,
				Anchor = anchors[i],
				Name = $"hedgemen:button-{i}"
			});
		}

		scene.AfterUpdate += (sender, args) =>
		{
			if (scene.Input.IsKeyPressed(Keys.Escape))
				Exit();
			else if (scene.Input.IsKeyPressed(Keys.Space))
				Console.Clear();
		};

		ChangeScenes(scene);
		Test();
	}

	private void Test()
	{
		var entity = new Entity();
		entity.AddComponent(new CharacterSheet());
		entity.AddComponent(new CharacterRace
		{
			RaceName = "high elf"
		});

		for (int i = 0; i < 1; ++i)
		{
			if (entity.WillRespondToEvent<ChangeStatEvent>())
			{
				entity.PropagateEvent(new ChangeStatEvent
				{
					Sender = entity,
					ChangeAmount = 1015,
					StatName = "strength"
				});
			}

			entity.PropagateEventIfResponsive(new ChangeStatEvent
			{
				Sender = entity,
				ChangeAmount = 10,
				StatName = "constitution"
			});
		}

		var data = entity.WriteObjectState();
		var entityClone = data.GetSerializedObject<Entity>();

		entityClone.RemoveComponent<CharacterSheet>();
		Console.WriteLine($"Responds to event: {entityClone.WillRespondToEvent<ChangeStatEvent>()}");

		entity.RemoveComponent<CharacterSheet>();
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