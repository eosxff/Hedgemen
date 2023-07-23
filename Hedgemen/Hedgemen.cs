using System;
using System.IO;
using System.Text;
using Hgm.Vanilla;
using Petal.Framework;
using Petal.Framework.Content;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Util;
using Petal.Framework.Util.Logging;
using Petal.Framework.Windowing;

namespace Hgm;

public class Hedgemen : PetalGame
{
	private const LogLevel DebugLogLevel = LogLevel.Debug;
	private const LogLevel ReleaseLogLevel = LogLevel.Warn;
	public static readonly Version HedgemenVersion = typeof(Hedgemen).Assembly.GetName().Version!;

	private static bool IsEmbedOnlyMode()
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
			PetalExceptions.ThrowIfNull(_instance);
			return _instance;
		}
	}

	public Registry Registry
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		_instance = this;

		OnDebugChanged += DebugChangedCallback;
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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

	protected override void OnExiting(object sender, EventArgs args)
	{
		Logger.Debug($"Exiting Hedgemen.");
		WriteLogFile();

		AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
	}

	private void WriteLogFile()
	{
		var logFile = new FileInfo("log.txt");
		logFile.WriteString(Logger.ToString(), Encoding.UTF8, FileMode.OpenOrCreate);
	}

	protected override void Initialize()
	{
		base.Initialize();

		Logger.LogLevel = LogLevel.Debug;
		Registry = new Registry(Logger);
		ModLoader = new PetalModLoader(Logger);

		Setup();
	}

	private void DebugChangedCallback(object? sender, DebugChangedArgs args)
	{
		if (sender is PetalGame game)
		{
			Logger.LogLevel = args.IsDebug ? DebugLogLevel : ReleaseLogLevel;
			Logger.Debug($"Logger now set to {game.Logger.LogLevel.ToString()}.");
		}
	}

	protected override ILogger GetInitialLogger()
	{
#if DEBUG
		var logLevel = DebugLogLevel;
#else
		var logLevel = ReleaseLogLevel;
#endif

		var logger = new PetalLogger
		{
			LogLevel = logLevel
		};

		string hedgemenVersion = typeof(Hedgemen).Assembly.GetName().Version!.ToString(3);
		string petalVersion = typeof(PetalGame).Assembly.GetName().Version!.ToString(3);

		logger.Debug($"Hedgemen-{hedgemenVersion}, Petal-{petalVersion}");

		return logger;
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

	private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
	{
		Logger.Critical($"Unhandled exception:\n{args.ExceptionObject}");
		WriteLogFile();
	}
}
