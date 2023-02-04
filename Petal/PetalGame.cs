using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Assets;
using Petal.Input;
using Petal.Scenery;
using Petal.Util;
using Petal.Windowing;

namespace Petal;

public class PetalGame : Game
{
	public static PetalGame Instance
	{
		get;
		private set;
	}
	
	private WindowMode _windowMode;
	private Scene? _scene;
	
	public GraphicsDeviceManager Graphics
	{
		get;
	}
	
	public AssetLoader Assets
	{
		get;
		private set;
	}

	public InputProvider Input
	{
		get;
	}

	public Scene? Scene
	{
		get => _scene;
		set
		{
			_scene?.Exit();
			_scene = value;
			_scene?.Initialize();
		}
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
			}
			
			Graphics.ApplyChanges();
		}
	}

	public PetalGame()
	{
		Graphics = new GraphicsDeviceManager(this);
		Input = new InputProvider();
		Instance = this;
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
		
		Graphics.ApplyChanges();
	}

	public GameSettings QueryGameSettings()
	{
		var settings = new GameSettings
		{
			WindowWidth = Graphics.PreferredBackBufferWidth,
			WindowHeight = Graphics.PreferredBackBufferHeight,
			WindowMode = WindowMode.Windowed,
			PreferredFramerate = (int)Math.Round(1000d / TargetElapsedTime.Milliseconds),
			Vsync = IsFixedTimeStep,
			IsMouseVisible = IsMouseVisible
		};

		return settings;
	}

	protected override void Initialize()
	{
		base.Initialize();
		Content = new StubContentManager(Services);
		Assets = new AssetLoader(GraphicsDevice);
		ApplyGameSettings(GetInitialGameSettings());
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

	protected override void Draw(GameTime gameTime)
	{
		Scene?.Draw();
		base.Draw(gameTime);
	}

	protected override void Update(GameTime gameTime)
	{
		Input.Update(gameTime, Matrix.Identity);
		Scene?.Update();
		base.Update(gameTime);
	}
}