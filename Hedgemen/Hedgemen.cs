using System;
using System.IO;
using Hgm.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Petal;
using Petal.Graphics;
using Petal.Input;
using Petal.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	private Renderer _renderer;
	private Texture2D _texture;
	
	public Hedgemen()
	{
		
	}

	protected override void Initialize()
	{
		base.Initialize();
		_renderer = new SceneRenderer();
		_texture = Assets.LoadAsset<Texture2D>(new FileInfo("peach.png").Open(FileMode.Open));
		Scene = new MainMenu
		{
			BackgroundColor = Color.CornflowerBlue
		};
	}

	protected override GameSettings GetInitialGameSettings()
	{
		return new GameSettings
		{
			PreferredFramerate = 60,
			Vsync = false,
			WindowWidth = 1200,
			WindowHeight = 800,
			WindowMode = WindowMode.Windowed,
			IsMouseVisible = true
		};
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		
		if(Input.KeyPressed(Keys.Escape))
			Exit();
		
		if (Input.MouseButtonClicked(MouseButtons.LeftButton))
		{
			if (!Input.IsRecordingTypedChars)
				Input.IsRecordingTypedChars = true;
			else
				Console.WriteLine(Input.GetTypedChars());
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}