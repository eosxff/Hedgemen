﻿using System;
using System.IO;
using System.Text;
using Hgm.Components;
using Hgm.Vanilla;
using Hgm.Vanilla.Modding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Petal.Framework;
using Petal.Framework.EC;
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

	private bool IsEmbedOnlyMode()
	{
#if EMBEDONLYMODE
		return true;
#else
		return false;
#endif
	}
	
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

	public ForgeModLoader Forge
	{
		get;
		private set;
	}

	protected override void Setup()
	{
		var context = Forge.Setup(new ModLoaderSetupArgs
		{
			EmbedOnlyMode = IsEmbedOnlyMode()
		});
		
		context.EmbeddedMods.Add(new HedgemenVanilla());
		
		Logger.Debug($"Starting {nameof(ForgeModLoader)}.");

		var logLevel = Forge.Start(context) ? LogLevel.Debug : LogLevel.Error;
		
		Logger.Add(
			logLevel == LogLevel.Debug ?
				$"Successfully started {nameof(ForgeModLoader)}" :
				$"Unsuccessfully started {nameof(ForgeModLoader)}.",
			logLevel);
		
		Logger.Debug($"Can we access hedgemen:mod from Forge: " +
		             $"{Forge.GetMod("hedgemen:mod", out HedgemenVanilla vanilla)}");
		
		Logger.Debug($"Can we access example:mod from Forge: " +
		             $"{Forge.GetMod("example:mod", out ForgeMod example)}");
		
		Logger.Debug($"Can we access no_code:mod from Forge: " +
		             $"{Forge.GetMod("no_code:mod", out ForgeMod noCode)}");
	}

	protected override void Initialize()
	{
		base.Initialize();
		Logger.LogLevel = LogLevel.Debug;

		Forge = new ForgeModLoader(Logger);

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

		if (IsDebug)
		{
			Test();
		}
	}

	private void Test()
	{
		var mapCell = new MapCell();
		mapCell.AddComponent<PerlinGeneration>();
		
		if(mapCell.WillRespondToEvent<SetHeightEvent>())
		{
			mapCell.PropagateEvent(new SetHeightEvent
			{
				Sender = mapCell,
				Height = 0.95f
			});
			
			Logger.Debug($"Cell height is now: {mapCell.GetComponent<PerlinGeneration>().Height}");
		}
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
		var oldLogLevel = Logger.LogLevel;

		Logger.LogLevel = LogLevel.Debug;
		
		const string fileName = "petal.json";
		var file = new FileInfo(fileName);
		var fallbackSettings = new GameSettings
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

		if (!file.Exists)
		{
			Logger.Warn("Using fallback game settings.");
			Logger.LogLevel = oldLogLevel;
			return fallbackSettings;
		}
		
		string json = file.ReadStringSilently(Encoding.UTF8);

		if (string.IsNullOrEmpty(json))
		{
			Logger.Warn("Using fallback game settings.");
			Logger.LogLevel = oldLogLevel;
			return fallbackSettings;
		}

		try
		{
			Logger.Debug($"Using settings from {fileName}.");
			Logger.LogLevel = oldLogLevel;
			return GameSettings.FromJson(json);
		}
		
		catch (Exception e)
		{
			Logger.Warn("Using fallback game settings.");
			Logger.LogLevel = oldLogLevel;
			return fallbackSettings;
		}
	}
}