using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Assets;
using Petal.Framework.Scenery;
using Petal.Framework.Windowing;
using Petal.Framework.Util;
using Petal.Framework.Util.Logging;

namespace Petal.Framework;

public abstract class PetalGame : Game
{
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

	public void ChangeScenes(Scene scene)
	{
		if (scene is null)
		{
			Logger.Error("Can not change to null scene.");
			return;
		}

		if (Scene is not null)
		{
			lock (Scene)
			{
				Scene.Exit();
				Scene = scene;
				Scene.Initialize();
			}
		}

		else
		{
			Scene = scene;
			Scene.Initialize();
		}

		OnSceneChanged?.Invoke(this, new SceneChangedArgs
		{
			NewScene = scene
		});
	}

	public WindowMode WindowMode
	{
		get => _windowMode;
		set
		{
			_windowMode = value;

			switch (_windowMode)
			{
				case WindowMode.Windowed:
					Graphics.IsFullScreen = false;
					Window.SetBorderless(false);
					break;

				case WindowMode.BorderlessWindowed:
					Graphics.IsFullScreen = false;
					Window.SetBorderless(true);
					break;

				case WindowMode.BorderlessFullscreen:
					Graphics.IsFullScreen = false;
					Window.SetBorderless(true);
					Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
					Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
					break;

				case WindowMode.Fullscreen:
					Window.SetBorderless(false);
					Graphics.ToggleFullScreen();
					break;

				default:
					throw new ArgumentOutOfRangeException(_windowMode.ToString());
			}

			Graphics.ApplyChanges();
		}
	}

	protected PetalGame()
	{
		Graphics = new GraphicsDeviceManager(this);
		Petal = this;
	}

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

	public GameSettings QueryGameSettings()
	{
		var settings = new GameSettings
		{
			WindowWidth = Graphics.PreferredBackBufferWidth,
			WindowHeight = Graphics.PreferredBackBufferHeight,
			WindowMode = WindowMode.Windowed,
			// don't use milliseconds (precision loss)
			PreferredFramerate = (int)Math.Round(1000d / (TargetElapsedTime.Ticks / 10000d)),
			Vsync = IsFixedTimeStep,
			IsMouseVisible = IsMouseVisible,
			IsWindowUserResizable = Window.AllowUserResizing,
			IsDebug = IsDebug
		};

		return settings;
	}

	protected virtual void Setup()
	{

	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		base.OnExiting(sender, args);
		Assets.Dispose();
		Petal = null;
	}

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

	protected virtual GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			WindowWidth = 960,
			WindowHeight = 540,
			PreferredFramerate = 60,
			Vsync = false,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}

	protected virtual ILogger GetInitialLogger()
		=> new PetalLogger
		{
			LogLevel = LogLevel.Off
		};

	protected override void Draw(GameTime gameTime)
	{
		Scene?.Draw(gameTime);
		base.Draw(gameTime);
	}

	protected override void Update(GameTime gameTime)
	{
		Scene?.Update(gameTime);
		base.Update(gameTime);
	}
}
