using System;
using System.IO;
using System.Text;
using Hgm.Vanilla;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.IO;
using Petal.Framework.Modding;
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

	private static Hedgemen _instance;

	public static Hedgemen Instance
	{
		get
		{
			ArgumentNullException.ThrowIfNull(_instance);
			return _instance;
		}
	}

	public ContentRegistry Registry
	{
		get;
		private set;
	}

	public Registry NRegistry // todo
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		_instance = this;
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
	}

	protected override void Initialize()
	{
		base.Initialize();

		var manifest = PetalModManifest.FromJson(
			new FileInfo("mods/example_mod/manifest.json").ReadString(Encoding.UTF8));
		
		Logger.LogLevel = LogLevel.Debug;
		NRegistry = new Registry(Logger);
		Registry = new ContentRegistry(Logger);
		ModLoader = new PetalModLoader(Logger);

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
			WindowMode = WindowMode.BorderlessFullscreen,
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