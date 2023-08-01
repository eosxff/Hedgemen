using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Petal.Framework.IO;
using Petal.Framework.Util;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Modding;

/// <summary>
/// A somewhat general purpose mod loader. Obviously designed with Hedgemen in mind.
/// </summary>
public class PetalModLoader
{
	private readonly Dictionary<NamespacedString, PetalMod> _mods = new();

	public IReadOnlyDictionary<NamespacedString, PetalMod> Mods
		=> _mods;

	public ILogger Logger
	{
		get;
	}

	public PetalModLoader(ILogger logger)
	{
		Logger = logger;
	}

	/// <summary>
	/// Readies the mod loader to be able to start with a <see cref="ModLoaderSetupContext"/>.
	/// </summary>
	/// <param name="args">the setup arguments.</param>
	/// <returns></returns>
	public ModLoaderSetupContext Setup(ModLoaderSetupArgs args)
	{
		string modsDirectory = args.ModsDirectoryName ?? "mods";
		string manifestFileName = args.ManifestFileName ?? "manifest.json";

		var context = new ModLoaderSetupContext
		{
			EmbeddedMods = args.EmbeddedMods,
			ModsDirectory = new DirectoryInfo(modsDirectory),
			ManifestFileName = manifestFileName,
			EmbedOnlyMode = args.EmbedOnlyMode,
			Game = args.Game,
			ModLoader = this
		};

		return context;
	}

	/// <summary>
	/// Starts the mod loader. Embedded mods will be loaded before regular mods. Regular mods will not be loaded
	/// if <see cref="ModLoaderSetupContext.EmbedOnlyMode"/> is true.
	/// </summary>
	/// <param name="context">the current mod loader context.</param>
	/// <returns>value indicating whether or not the mod loader was able to start correctly.</returns>
	public bool Start(ModLoaderSetupContext context)
	{
		var logger = PetalGame.Petal.Logger;
		var modDirectoryQuery = new List<DirectoryInfo>();

		if (!context.EmbedOnlyMode)
			modDirectoryQuery = QueryModsFromModDirectory(context);
		else
			Logger.Warn("Embed only mode is on. Only loading embedded mods.");

		logger.Debug($"Number of mods recognized in folder '{context.ModsDirectory}': {modDirectoryQuery.Count}");

		var loadedMods = LoadModsFromEmbedAndDirectory(context, modDirectoryQuery);

		foreach (var mod in loadedMods)
		{
			_mods.Add(mod.Manifest.ModID, mod);
			mod.OnLoadedToPetalModLoader();
		}

		logger.Debug($"Loaded {_mods.Count} mods.");
		logger.Debug($"Initializing {_mods.Count} mods.");

		ICollection<PetalMod> modsCollection = _mods.Values;

		foreach (var mod in modsCollection)
		{
			mod.PrePetalModLoaderModSetupPhase(context);
		}

		foreach (var mod in modsCollection)
		{
			mod.Setup(context);
		}

		foreach (var mod in modsCollection)
		{
			mod.PostPetalModLoaderSetupPhase(context);
		}

		return true;
	}

	/// <summary>
	/// Attempts to retrieve a mod based on the given <paramref name="modID"/>. The retrieved <paramref name="mod"/>
	/// will be a valid reference if true is returned, otherwise it will be null.
	/// </summary>
	/// <param name="modID">the mod namespaced identifier.</param>
	/// <param name="mod">the retrieved mod.</param>
	/// <typeparam name="T">the type of the mod being retrieved.</typeparam>
	/// <returns>if the operation succeeded.</returns>
	public bool GetMod<T>(NamespacedString modID, [NotNullWhen(true)] out T? mod) where T : PetalMod
	{
		mod = default;

		bool found = _mods.TryGetValue(modID, out var petalMod);

		if (!found)
		{
			Logger.Error($"{modID} could not be found in the mod loader.");
			return false;
		}

		if (petalMod is T tMod)
			mod = tMod;
		else
		{
			Logger.Error($"Mod {petalMod.GetType()} can not be cast to {typeof(T)}.");
			return false;
		}

		return true;
	}

	private List<DirectoryInfo> QueryModsFromModDirectory(ModLoaderSetupContext context)
	{
		var directory = context.ModsDirectory;

		if (!directory.Exists)
		{
			Logger.Warn($"Could not find directory '{directory.FullName}'");
			return new List<DirectoryInfo>();
		}

		bool DirectoryFilter(DirectoryInfo d)
			=> d.FindFile(context.ManifestFileName).Exists;

		var validModDirectories = directory.GetDirectories(DirectoryFilter);

		var directories = new List<DirectoryInfo>(validModDirectories.Length);

		foreach (var dir in validModDirectories)
			directories.Add(dir);

		return directories;
	}

	private List<PetalMod> LoadModsFromEmbedAndDirectory(ModLoaderSetupContext context, List<DirectoryInfo> directories)
	{
		var list = new List<PetalMod>(context.EmbeddedMods.Count + directories.Count);

		// load first class mods
		foreach (var embeddedModsElement in context.EmbeddedMods)
		{
			if (embeddedModsElement is PetalEmbeddedMod embeddedMod)
			{
				SetModManifestAndDirectory(embeddedMod, embeddedMod.GetEmbeddedManifest(), new DirectoryInfo("."));
				list.Add(embeddedMod);
				Logger.Debug($"Loaded Embedded mod '{embeddedModsElement}'.");
			}
			else
			{
				Logger.Critical($"First class/embedded mod {embeddedModsElement.GetType()} " +
				                $"is not of type {typeof(PetalEmbeddedMod)}.");
				throw new PetalException();
			}
		}

		// load third party mods
		foreach (var directory in directories)
		{
			bool loadSuccessful = LoadModFromDirectory(context, directory, out var mod);

			if (!loadSuccessful)
				Logger.Error($"Could not load mod from '{directory.Name}'.");
			else
			{
				Logger.Debug($"Loaded '{mod.Manifest.ModID}' of type '{mod.GetType()}'.");
				list.Add(mod);
			}
		}

		return list;
	}

	private bool LoadModFromDirectory(ModLoaderSetupContext context, DirectoryInfo directory, out PetalMod mod)
	{
		mod = null;
		var manifestFile = directory.FindFile(context.ManifestFileName);

		if (!manifestFile.Exists) // should never trip
		{
			Logger.Error($"File '{context.ManifestFileName}' could not be found in '{directory.Name}'");
			return false;
		}

		string manifestJson = manifestFile.ReadString(Encoding.UTF8);
		var manifest = JsonSerializer.Deserialize<PetalModManifest?>(manifestJson);

		if (manifest is null)
		{
			Logger.Error($"Manifest could not be created from:\n{manifestJson}");
			return false;
		}

		if (string.IsNullOrEmpty(manifest.ModFileDll))
		{
			mod = new DefaultPetalMod(); // todo
		}

		else
		{
			var dllFile = directory.FindFile(manifest.ModFileDll);

			if (!dllFile.Exists)
			{
				Logger.Error($"File '{dllFile.FullName}' does not exist.");
				return false;
			}

			var assembly = Assembly.LoadFile(dllFile.FullName);

			foreach (string dependencyFileName in manifest.Dependencies.ReferencedDlls)
			{
				var dllDependencyFile = directory.FindFile(dependencyFileName);

				if (dllDependencyFile.Exists)
				{
					Assembly.LoadFile(dllDependencyFile.FullName);
				}
				else
				{
					Logger.Error($"File {dependencyFileName} does not exist.");
					return false;
				}
			}

			var modMainType = assembly.GetType(manifest.ModMain);

			if (modMainType is null)
			{
				Logger.Error($"Mod entry point '{manifest.ModMain}' could not be found.");
				return false;
			}

			mod = (PetalMod)Activator.CreateInstance(modMainType, true);

			if (mod is null)
			{
				Logger.Error($"Could not create instance from '{modMainType}'");
				return false;
			}
		}

		SetModManifestAndDirectory(mod, manifest, directory);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SetModManifestAndDirectory(PetalMod mod, PetalModManifest manifest, DirectoryInfo directory)
	{
		mod.Manifest = manifest;
		mod.Directory = directory;
	}
}

/// <summary>
/// Initialization context for <see cref="PetalModLoader"/>.
/// </summary>
public sealed class ModLoaderSetupContext
{
	public required DirectoryInfo ModsDirectory
	{
		get;
		init;
	}

	/// <summary>
	/// The <see cref="PetalModManifest"/> file name for when <see cref="PetalModLoader"/>
	/// scans directories for valid mods.
	/// </summary>
	public required string ManifestFileName
	{
		get;
		init;
	}

	/// <summary>
	/// First party mods.
	/// </summary>
	public required IReadOnlyList<PetalMod> EmbeddedMods
	{
		get;
		init;
	}

	/// <summary>
	/// Whether or not the mod loader should load third-party mods on <see cref="PetalModLoader.Start"/>.
	/// First party mods will always be loaded.
	/// </summary>
	public required bool EmbedOnlyMode
	{
		get;
		init;
	}

	/// <summary>
	/// The <see cref="PetalGame"/> the mod loader is attached to.
	/// </summary>
	public required PetalGame Game
	{
		get;
		init;
	}

	/// <summary>
	/// The
	/// </summary>
	public required PetalModLoader ModLoader
	{
		get;
		init;
	}
}

/// <summary>
/// <see cref="PetalModLoader"/> custom arguments for <see cref="PetalModLoader.Setup"/>.
/// </summary>
public struct ModLoaderSetupArgs
{
	/// <summary>
	/// The game instance the mod loader uses.
	/// </summary>
	public required PetalGame Game
	{
		get;
		init;
	}

	/// <summary>
	/// First party mods.
	/// </summary>
	public required IReadOnlyList<PetalMod> EmbeddedMods
	{
		get;
		init;
	}

	/// <summary>
	/// Whether or not the mod loader should load third-party mods on <see cref="PetalModLoader.Start"/>.
	/// First party mods will always be loaded.
	/// </summary>
	public required bool EmbedOnlyMode
	{
		get;
		init;
	}

	/// <summary>
	/// The local directory path for <see cref="PetalModLoader"/> to scan from.
	/// </summary>
	public string? ModsDirectoryName
	{
		get;
		init;
	}

	/// <summary>
	/// The <see cref="PetalModManifest"/> file name for when <see cref="PetalModLoader"/>
	/// scans directories for valid mods.
	/// </summary>
	public string? ManifestFileName
	{
		get;
		init;
	}
}
