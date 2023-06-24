using System;
using System.IO;
using System.Text;
using Hgm.Vanilla;
using Hgm.Vanilla.Modding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.Graphics;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Scenery;
using Petal.Framework.Scenery.Nodes;
using Petal.Framework.Util.Logging;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	public static Version Version
		=> new(0, 0, 1);
	
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

	public ForgeModLoader ModLoader
	{
		get;
		private set;
	}

	protected override void Setup()
	{
		var context = ModLoader.Setup();
		context.EmbeddedMods.Add(new HedgemenVanilla());
		
		Logger.Debug($"Starting {nameof(ForgeModLoader)}.");

		var logLevel = ModLoader.Start(context) ? LogLevel.Debug : LogLevel.Error;
		
		Logger.Add(
			logLevel == LogLevel.Debug ?
				$"Successfully started {nameof(ForgeModLoader)}" :
				$"Unsuccessfully started {nameof(ForgeModLoader)}.",
			logLevel);
	}

	protected override void Initialize()
	{
		base.Initialize();

		ModLoader = new ForgeModLoader(Logger);

		Logger.Debug("I");
		Logger.Warn("Love");
		Logger.Error("These");
		Logger.Critical("Colours");
		
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
		Setup();
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