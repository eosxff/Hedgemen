using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework.Assets;
using Petal.Framework.Scenery;
using Petal.Framework.Windowing;
using Petal.Framework.Util;

namespace Petal.Framework;

public class PetalGame : Game
{
	public static PetalGame Petal
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

	public Scene? Scene
	{
		get => _scene;
	}

	public void ChangeScenes(Scene scene)
	{
		_scene?.Exit();
		_scene = scene;
		_scene?.Initialize();
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
		
		Graphics.ApplyChanges();
	}

	public GameSettings QueryGameSettings()
	{
		var settings = new GameSettings
		{
			WindowWidth = Graphics.PreferredBackBufferWidth,
			WindowHeight = Graphics.PreferredBackBufferHeight,
			WindowMode = WindowMode.Windowed,
			// we don't use TargetElapsedTime.Milliseconds due to precision loss
			PreferredFramerate = (int)(1000d / (TargetElapsedTime.Ticks / 10000d)),
			Vsync = IsFixedTimeStep,
			IsMouseVisible = IsMouseVisible
		};

		return settings;
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
		Scene?.Draw(gameTime);
		base.Draw(gameTime);
	}

	protected override void Update(GameTime gameTime)
	{
		Scene?.Update(gameTime);
		base.Update(gameTime);
	}
}