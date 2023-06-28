using System;
using System.IO;
using System.Text;
using Hgm.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework.EC;
using Petal.Framework.Graphics;
using Petal.Framework.Input;
using Petal.Framework.IO;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Windowing;

namespace Petal.Framework.Sandbox;

using Framework;

public class SandboxGame : PetalGame
{
	public static SandboxGame Instance
	{
		get;
		private set;
	}

	public ContentRegistry ContentRegistry
	{
		get;
	} = new();

	private Texture2D _whiteSquare;
	private Texture2D _peach;

	private Skin _skin;

	public SandboxGame()
	{
		Instance = this;
		OnDebugChanged += DebugChangedCallback;
	}

	protected override void Initialize()
	{
		base.Initialize();

		ContentRegistry.OnContentRegistered += (sender, args) =>
		{
			Logger.Debug($"Registered '{args.RegisteredContent.ContentIdentifier}' to content registry.");
		};
		
		ContentRegistry.Register(
			"sandbox:ui/button_hover_texture",
			Assets.LoadAsset<Texture2D>("button_hover.png"));

		ContentRegistry.Register(
			"sandbox:ui/button_input_texture",
			Assets.LoadAsset<Texture2D>("button_input.png"));

		ContentRegistry.Register(
			"sandbox:ui/button_normal_texture",
			Assets.LoadAsset<Texture2D>("button_normal.png"));

		_whiteSquare = Assets.LoadAsset<Texture2D>("white_square.png");
		_peach = Assets.LoadAsset<Texture2D>("peach.png");

		ContentRegistry.Register(
			"sandbox:white_square",
			_whiteSquare);

		_skin = Skin.FromJson(new FileInfo("skin.json").ReadString(Encoding.UTF8), ContentRegistry);
		Logger.Debug($"Skin successfully created.");

		_skin.Button.NormalTexture.ReloadItem(ContentRegistry);

		var testContentRegistryGet = new NamespacedString("sandbox:ui/button_normal_texture");
		
		try
		{
			ContentRegistry.Get<Skin>(testContentRegistryGet);
			Logger.Debug($"Content successfully grabbed from registry: {testContentRegistryGet}");
		}
		
		catch (Exception e)
		{
			Logger.Error($"Content unsuccessfully grabbed from registry: {testContentRegistryGet}");
		}

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
			if (sender is Node node)
			{
				Logger.Debug($"Node {args.Child.Name} has been added to {node.Name}");
			}
		};

		var buttonSize = new Vector2Int(32, 32);
		var anchors = Enum.GetValues<Anchor>();

		for (int i = 0; i < anchors.Length; ++i)
		{
			var button = scene.Root.Add(new Button(_skin)
			{
				Bounds = new Rectangle(8, 8, buttonSize.X, buttonSize.Y),
				Color = Color.White,
				Anchor = anchors[i],
				Name = $"sandbox:button-{i}"
			});
		}

		scene.AfterUpdate += SceneAfterUpdate;

		ChangeScenes(scene);
		Test();
	}

	private void SceneAfterUpdate(object? sender, EventArgs args)
	{
		if (sender is not Scene scene)
			return;
		
		if (scene.Input.IsKeyPressed(Keys.Escape))
			Exit();
		else if (scene.Input.IsKeyPressed(Keys.Space))
			Console.Clear();
			
		if(scene.Input.IsMouseButtonClicked(MouseButtons.RightButton))
			Logger.Debug("right button has been clicked!");

		if (scene.Input.IsKeyPressed(Keys.Back))
		{
			string logFile = "log.txt";
			
			Logger.Debug($"Writing log file to {logFile}.");

			var file = new FileInfo(logFile);
			file.WriteString(Logger.ToString(), Encoding.UTF8, FileMode.OpenOrCreate);
		}
	}
	
	private void DebugChangedCallback(object? sender, DebugChangedArgs args)
	{
		if (sender is PetalGame game)
		{
			Logger.Debug($"Logger now set to {game.Logger.LogLevel.ToString()}.");
		}
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

		if (entity.WillRespondToEvent<ChangeStatEvent>())
		{
			var task = entity.PropagateEventAsync(new ChangeStatEvent
			{
				Sender = entity,
				ChangeAmount = 1015,
				StatName = "strength"
			});

			task.ContinueWith(_ =>
			{
				lock (entity)
				{
					Logger.Debug($"New entity strength: {entity.GetComponent<CharacterSheet>().Strength}");
				}
			});
		}

		var data = entity.WriteObjectState();
		var entityClone = data.GetSerializedObject<Entity>();

		Logger.Debug($"Test entity responds to {nameof(ChangeStatEvent)}: " +
		             $"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");
		
		entityClone.RemoveComponent<CharacterSheet>();
		
		Logger.Debug(
			$"Test does entity respond to {nameof(ChangeStatEvent)} after removing all referenced components: " + 
			$"{entityClone.WillRespondToEvent<ChangeStatEvent>()}");
		
		Logger.Error($"Testing if error applies in {(IsDebug ? "Debug" : Logger.LogLevel.ToString())}");
		Logger.Critical($"Testing if critical applies in {(IsDebug ? "Debug" : Logger.LogLevel.ToString())}");

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
			IsWindowUserResizable = true,
			IsDebug = true
		};
	}
}