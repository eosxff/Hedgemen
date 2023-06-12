using System;
using System.IO;
using System.Reflection;
using System.Text;
using Hgm.ComponentsNew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Framework;
<<<<<<< Updated upstream
using Petal.Framework.EntityComponent.Persistence;
=======
using Petal.Framework.EC;
>>>>>>> Stashed changes
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

	private Dictionary<string, Assembly> _registeredAssemblies = new(4);

	protected override void Initialize()
	{
		base.Initialize();

		SerializedRecord.DefaultRegisteredAssemblies = () => _registeredAssemblies;

		RegisterAssemblies(
			typeof(object),
			typeof(Hedgemen),
			typeof(PetalGame),
			typeof(Game));

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

		for (var i = 0; i < anchors.Length; ++i)
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
			else if(scene.Input.IsKeyPressed(Keys.Space))
				Console.Clear();
		};

		ChangeScenes(scene);
		Test();
	}

<<<<<<< Updated upstream
	public bool RegisterAssembly(Type type)
	{
		var assembly = type.Assembly;
		var assemblyFullName = assembly.FullName;

		if (assemblyFullName is null)
		{
			Console.WriteLine($"Assembly.FullName for type {type} is null. Can not register assembly.");
			return false;
		}

		if (_registeredAssemblies.ContainsKey(assemblyFullName))
		{
			Console.WriteLine($"{assemblyFullName} is already registered.");
			return false;
		}

		_registeredAssemblies.Add(assemblyFullName, assembly);
		return true;
	}

	public bool RegisterAssembly<T>()
	{
		return RegisterAssembly(typeof(T));
	}

	public void RegisterAssemblies(params Type[] types)
	{
		for (var i = 0; i < types.Length; ++i)
		{
			var type = types[i];
			RegisterAssembly(type);
		}
	}

	private void Test()
	{
		var sheet = new CharacterSheet();
		sheet.RegisterEvents();
		
		sheet.PropagateEvent(new ChangeStatEvent
		{
			Sender = null,
			ChangeAmount = 10,
			StatName = "strength"
		});
=======
	private void Test()
	{
		var entity = new Entity();
		entity.AddComponent(new CharacterSheet());

		Console.WriteLine($"Event count: {entity._componentEvents.Count}");

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

		var data = entity.WriteObjectState();//new SerializedData(entity);
		//data.AddSerializedObject("any:entity", entity);

		var entityClone = data.GetSerializedObject<Entity>();
		Console.WriteLine($"Sheet: {entityClone?.GetComponent<CharacterSheet>()}");

		entity.RemoveComponent<CharacterSheet>();
		Console.WriteLine($"Event count: {entity._componentEvents.Count}");
>>>>>>> Stashed changes
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