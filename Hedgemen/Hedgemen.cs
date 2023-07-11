using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Hgm.Components;
using Hgm.Vanilla;
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
#if EMBED_ONLY_MODE
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
		private set;
	}

	public Hedgemen()
	{
		Instance = this;
		OnDebugChanged += DebugChangedCallback;
	}

	public PetalModLoader ModLoader
	{
		get;
		private set;
	}

	protected override void Setup()
	{
		var context = ModLoader.Setup(new ModLoaderSetupArgs
		{
			EmbedOnlyMode = IsEmbedOnlyMode(),
			Game = this,
			EmbeddedMods = new IMod[] { new HedgemenVanilla() }
		});

		Logger.Debug($"Starting {nameof(PetalModLoader)}.");

		var logLevel = ModLoader.Start(context) ? LogLevel.Debug : LogLevel.Error;
		
		Logger.Add(
			logLevel == LogLevel.Debug ?
				$"Successfully started {nameof(PetalModLoader)}" :
				$"Unsuccessfully started {nameof(PetalModLoader)}.",
			logLevel);
		
		Logger.Debug($"We can access hgm:mod from {nameof(PetalModLoader)}: " +
		             $"{ModLoader.GetMod("hgm:mod", out HedgemenVanilla vanilla)}");
		
		Logger.Debug($"We can access example:mod from {nameof(PetalModLoader)}: " +
		             $"{ModLoader.GetMod("example:mod", out PetalMod example)}");
		
		Logger.Debug($"We can access no_code:mod from {nameof(PetalModLoader)}: " +
		             $"{ModLoader.GetMod("no_code:mod", out PetalMod noCode)}");
		
		Logger.Debug(example.Manifest.Contact.Homepage);
		Logger.Debug(example.Manifest);
		Logger.Debug(example.Manifest.Dependencies.IncompatibleMods[0]);
	}

	protected override void Initialize()
	{
		base.Initialize();

		var manifest = PetalModManifest.FromJson(new FileInfo("mods/example_mod/manifest.json").ReadString(Encoding.UTF8));
		Logger.Critical($"{manifest?.Description}");
		
		ContentRegistry = new ContentRegistry(Logger);
		Logger.LogLevel = LogLevel.Debug;

		ModLoader = new PetalModLoader(Logger);

		Logger.Debug("I");
		Logger.Warn("Love");
		Logger.Error("These");
		Logger.Critical("Colours");
		
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
			Logger.Warn("Using fallback game settings. Exception was raised.");
			Logger.LogLevel = oldLogLevel;
			return fallbackSettings;
		}
	}
}