using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
	public static string Version => VersionStringify(typeof(Hedgemen).Assembly.GetName().Version!);

	private static Hedgemen HedgemenInstance;

	public static Hedgemen Instance
	{
		get
		{
			PetalExceptions.ThrowIfNull(HedgemenInstance);
			return HedgemenInstance;
		}
	}

	public Registry Registry
	{
		get;
		private set;
	}

	public PetalModLoader ModLoader
	{
		get;
		private set;
	}

	public Hedgemen()
	{
		HedgemenInstance = this;

		OnDebugChanged += DebugChangedCallback;
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
	}

	protected override void Setup()
	{
		var embeddedMods = new List<PetalEmbeddedMod>(2)
		{
			new HedgemenVanilla()
		};

		if(IsDebug)
			embeddedMods.Add(new HedgemenDebugMod());

		var context = ModLoader.Setup(new ModLoaderSetupArgs
		{
			EmbedOnlyMode = IsEmbedOnlyMode(),
			Game = this,
			EmbeddedMods = embeddedMods
		});

		Logger.Info($"Starting {nameof(PetalModLoader)}.");

		const LogLevel modLoaderSuccessfulLevel = LogLevel.Info;
		const LogLevel modLoaderUnsuccessfulLevel = LogLevel.Critical;

		var logLevel = ModLoader.Start(context) ? modLoaderSuccessfulLevel : modLoaderUnsuccessfulLevel;

		if(logLevel == modLoaderSuccessfulLevel)
			Logger.Add($"Successfully started {nameof(PetalModLoader)}", modLoaderSuccessfulLevel);
		else
		{
			Logger.Add($"{nameof(PetalModLoader)} could not be started. Aborting.", modLoaderUnsuccessfulLevel);
			throw new PetalException();
		}
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		Logger.Info($"Exiting Hedgemen.");
		WriteLogFile();

		AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
	}

	protected override void Initialize()
	{
		base.Initialize();

		Registry = new Registry(Logger);
		ModLoader = new PetalModLoader(Logger);

		Setup();
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
			LogLevel = logLevel,
			LogInvalidLevelsSilently = true,
		};

		logger.Info($"Hedgemen:{Version}");

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
			IsWindowUserResizable = false,
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
			Logger.Info($"Using settings from {fileName}.");
			Logger.LogLevel = oldLogLevel;
			return GameSettings.FromJson(json);
		}

		catch (Exception e)
		{
			Logger.Warn($"Using fallback game settings. Exception was raised:\n{e}");
			Logger.LogLevel = oldLogLevel;
			return fallbackSettings;
		}
	}

	private void WriteLogFile()
	{
		var logFile = new FileInfo($"log-{DateTime.Now:yyyy-MM-dd-hh:mm:ss}.txt");

		if(logFile.Exists)
			logFile.Delete();

		logFile.WriteString(Logger.ToString(), Encoding.UTF8, FileMode.OpenOrCreate);
	}

	private void DebugChangedCallback(object? sender, DebugChangedArgs args)
	{
		if (sender is PetalGame game)
		{
			Logger.LogLevel = args.IsDebug ? DebugLogLevel : ReleaseLogLevel;
			Logger.Info($"Logger now set to {game.Logger.LogLevel.ToString()}.");
		}
	}

	private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs args)
	{
		Logger.Critical($"Unhandled exception:\n{args.ExceptionObject}");
		WriteLogFile();
	}

	private static bool IsEmbedOnlyMode()
	{
#if EMBED_ONLY_MODE
		return true;
#else
		return false;
#endif
	}

	private static string VersionStringify(Version version)
		=> $"indev-{version.Major}.{version.Minor}.{version.Build}";
}
