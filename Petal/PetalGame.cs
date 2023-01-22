using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal.Assets;
using Petal.Input;
using Petal.Util;
using Petal.Windowing;

namespace Petal;

public class PetalGame : Game
{
	private WindowMode _windowMode;
	
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
	}

	public void ApplyGameSettings(GameSettings settings)
	{
		Graphics.PreferredBackBufferWidth = settings.WindowWidth;
		Graphics.PreferredBackBufferHeight = settings.WindowHeight;
		Graphics.SynchronizeWithVerticalRetrace = settings.Vsync;
		
		IsFixedTimeStep = settings.Vsync;
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
			Vsync = IsFixedTimeStep
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
			WindowMode = WindowMode.Windowed
		};
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}

	protected override void Update(GameTime gameTime)
	{
		Input.Update(gameTime, Matrix.Identity);
		
		if(Input.KeyPressed(Keys.Space))
			Exit();
		
		base.Update(gameTime);
	}
}