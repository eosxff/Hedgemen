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
using Petal.Framework.Util.Logging;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Hedgemen Instance
	{
		get;
		private set;
	}

	public ContentRegistry ContentRegistry
	{
		get;
	} = new();

	public Hedgemen()
	{
		Instance = this;
		OnDebugChanged += DebugChangedCallback;
	}

	protected override void Initialize()
	{
		base.Initialize();

		ContentRegistry.Register(
			"hedgemen:ui/skin/button_hover_texture",
			Assets.LoadAsset<Texture2D>(new FileInfo("button_hover.png").Open(FileMode.Open)));
		
		ContentRegistry.Register(
			"hedgemen:ui/skin/button_normal_texture",
			Assets.LoadAsset<Texture2D>(new FileInfo("button_normal.png").Open(FileMode.Open)));
		
		ContentRegistry.Register(
			"hedgemen:ui/skin/button_input_texture",
			Assets.LoadAsset<Texture2D>(new FileInfo("button_input.png").Open(FileMode.Open)));

		var scene = new Scene(
			new Stage(),
			Skin.FromJson(
				new FileInfo("skin.json").ReadString(Encoding.UTF8),
				ContentRegistry))
		{
			BackgroundColor = Color.Green,
			ViewportAdapter = new BoxingViewportAdapter(GraphicsDevice, Window, new Vector2Int(640, 360))
		};

		ChangeScenes(scene);
	}

	private void DebugChangedCallback(object? sender, DebugChangedArgs args)
	{
		if (sender is PetalGame game)
		{
			Logger.Debug($"Logger now set to {game.Logger.LogLevel.ToString()}.");
		}
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