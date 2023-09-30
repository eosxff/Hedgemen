using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Assets;
using Petal.Framework.Scenery;
using Petal.Framework.Windowing;
using Petal.Framework.Util;
using Petal.Framework.Util.Coroutines;
using Petal.Framework.Util.Logging;

namespace Petal.Framework;

/// <summary>
/// Entry point for games using the petal framework. Sets up a window with a graphics device that runs a game loop
/// calling <see cref="Update"/> and <see cref="Draw"/>
/// </summary>
public abstract class PetalGame : Game
{
	internal static string PetalRepositoryLink => "https://github.com/eosxff/Hedgemen";

	public static readonly Version PetalVersion = typeof(PetalGame).Assembly.GetName().Version!;

	private static PetalGame _instance;

	public static PetalGame Petal
	{
		get
		{
			PetalExceptions.ThrowIfNull(_instance);
			return _instance;
		}

		private set => _instance = value;
	}

	public static bool IsPetalDebug
	{
		get
		{
			PetalExceptions.ThrowIfNull(_instance);
			return _instance.IsDebug;
		}
	}

	public struct DebugChangedArgs
	{
		public required bool IsDebug
		{
			get;
			init;
		}
	}

	public struct SceneChangedArgs
	{
		public required Scene NewScene
		{
			get;
			init;
		}
	}

	public event EventHandler<DebugChangedArgs>? OnDebugChanged;
	public event EventHandler<SceneChangedArgs>? OnSceneChanged;

	private WindowMode _windowMode;

	public ILogger Logger
	{
		get;
		private set;
	}

	public GraphicsDeviceManager Graphics
	{
		get;
	}

	public AssetLoader Assets
	{
		get;
		private set;
	}

	public bool IsDebug
	{
		get;
		private set;
	} = false;

	public Scene? Scene
	{
		get;
		private set;
	}

	private EnqueuedScene? _enqueuedScene = null;

	public CoroutineManager Coroutines
	{
		get;
	} = new();

	/// <summary>
	/// Changes the current scene. This will properly exit out and initialize the scenes accordingly.
	/// </summary>
	/// <param name="scene">new scene to change to.</param>
	public void ChangeScenes(Scene scene)
	{
		PetalExceptions.ThrowIfNull(scene);

		Scene?.Exit();

		if(!scene.IsFinishedLoading)
			scene.Load();

		Scene = scene;

		OnSceneChanged?.Invoke(this, new SceneChangedArgs
		{
			NewScene = scene
		});
	}

	public async void ChangeScenesAsync(Func<Scene> sceneSupplier)
	{
		var scene = await Task.Run(() =>
		{
			var scene = sceneSupplier();
			scene.Load();
			return scene;
		});

		_enqueuedScene = new EnqueuedScene
		{
			Scene = scene
		};
	}

	private async Task<Scene> SupplySceneAsync(Func<Scene> sceneSupplier)
	{
		return await Task.Run(() =>
		{
			var scene = sceneSupplier();
			scene.Load();
			return scene;
		});
	}

	/// <summary>
	/// The current window mode used by the game. Will only apply changes if the new value is not the same
	/// as the old value.
	/// </summary>
	public WindowMode WindowMode
	{
		get => _windowMode;
		set
		{
			if (_windowMode == value)
				return;

			_windowMode = value;

			switch (_windowMode)
			{
				case WindowMode.Windowed:
					Graphics.IsFullScreen = false;
					Window.SetBorderless(false);
					Graphics.ApplyChanges();
					break;
				case WindowMode.BorderlessWindowed:
					Graphics.IsFullScreen = false;
					Window.SetBorderless(true);
					Graphics.ApplyChanges();
					break;
				case WindowMode.BorderlessFullscreen:
					Graphics.IsFullScreen = false;
					Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
					Window.SetBorderless(true);
					Graphics.ApplyChanges();
					break;
				case WindowMode.Fullscreen:
					// wonky hack for monogame works with fna as well apparently
					// https://community.monogame.net/t/fullscreen-issues/15852/6
					Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
					Window.SetBorderless(false);
					Graphics.ApplyChanges();
					Graphics.ToggleFullScreen();
					break;
				default: // should never trip
					throw new ArgumentOutOfRangeException(_windowMode.ToString());
			}
		}
	}

	/// <summary>
	/// Creates a new instance of <see cref="PetalGame"/>. Note that this instance will not be properly initialized
	/// until <see cref="PetalGame.Run"/> is called.
	/// </summary>
	protected PetalGame()
	{
		Graphics = new GraphicsDeviceManager(this);
		Petal = this;
	}

	/// <summary>
	/// Applies <see cref="GameSettings"/>. If debug mode changes, <see cref="PetalGame.OnDebugChanged"/> will
	/// be called.
	/// </summary>
	/// <param name="settings">the settings to apply.</param>
	public void ApplyGameSettings(GameSettings settings)
	{
		Graphics.PreferredBackBufferWidth = settings.WindowWidth;
		Graphics.PreferredBackBufferHeight = settings.WindowHeight;
		Graphics.SynchronizeWithVerticalRetrace = settings.Vsync;

		IsMouseVisible = settings.IsMouseVisible;
		IsFixedTimeStep = !settings.Vsync;
		TargetElapsedTime = TimeSpan.FromMilliseconds(1000d / settings.PreferredFramerate);
		WindowMode = settings.WindowMode;
		Window.AllowUserResizing = settings.IsWindowUserResizable;

		if (settings.IsDebug != IsDebug)
		{
			IsDebug = settings.IsDebug;

			OnDebugChanged?.Invoke(this, new DebugChangedArgs
			{
				IsDebug = settings.IsDebug,
			});
		}

		Graphics.ApplyChanges();
	}

	/// <summary>
	/// Retrieves the current <see cref="GameSettings"/>.
	/// </summary>
	/// <returns>game settings currently used.</returns>
	public GameSettings QueryGameSettings()
	{
		var settings = new GameSettings
		{
			WindowWidth = Graphics.PreferredBackBufferWidth,
			WindowHeight = Graphics.PreferredBackBufferHeight,
			WindowMode = WindowMode,
			// don't use milliseconds (precision loss)
			PreferredFramerate = (int)Math.Round(1000d / (TargetElapsedTime.Ticks / 10000d)),
			Vsync = IsFixedTimeStep,
			IsMouseVisible = IsMouseVisible,
			IsWindowUserResizable = Window.AllowUserResizing,
			IsDebug = IsDebug
		};

		return settings;
	}

	/// <summary>
	/// Post engine initialization game setup.
	/// </summary>
	protected virtual void Setup()
	{

	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
		Assets.Dispose();
		Petal = null;
	}

	/// <summary>
	/// Engine initialization. If overriding, try not to make assumptions about object state, regardless of
	/// whether or not the particular object is not annotated with '?'.
	/// </summary>
	protected override void Initialize()
	{
		base.Initialize();
		Logger = GetInitialLogger();
		Content = new StubContentManager(Services);
		Assets = new AssetLoader(GraphicsDevice, Logger);
		Window.Title = GetType().Name;
		Window.ClientSizeChanged += WindowOnClientSizeChanged;

		ApplyGameSettings(GetInitialGameSettings());
	}

	private void WindowOnClientSizeChanged(object sender, EventArgs e)
	{
		Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
		Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
		Graphics.ApplyChanges();
	}

	/// <summary>
	/// Used to apply settings on engine initialization.
	/// </summary>
	protected virtual GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			WindowWidth = 960,
			WindowHeight = 540,
			PreferredFramerate = 60,
			Vsync = true,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}

	/// <summary>
	/// Retrieves the default logger when initializing the game.
	/// </summary>
	/// <returns></returns>
	protected virtual ILogger GetInitialLogger()
		=> new PetalLogger
		{
			LogLevel = LogLevel.Off
		};

	/// <summary>
	/// Called when the game is to perpetuate the draw loop.
	/// </summary>
	/// <param name="gameTime">the elapsed time since the previous draw call.</param>
	protected override void Draw(GameTime gameTime)
	{
		Scene?.Draw(gameTime);
		base.Draw(gameTime);
	}

	/// <summary>
	/// Called when the game is to perpetuate the update loop.
	/// </summary>
	/// <param name="gameTime">the elapsed time since the previous update call.</param>
	protected override void Update(GameTime gameTime)
	{
		if (_enqueuedScene is not null)
		{
			ChangeScenes(_enqueuedScene.Scene);
			_enqueuedScene = null;
		}

		Coroutines.Update(gameTime);
		Scene?.Update(gameTime);
		base.Update(gameTime);
	}

	private class EnqueuedScene
	{
		public required Scene Scene
		{
			get;
			init;
		}
	}
}
