using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Petal.Framework.IO;
using Petal.Framework.Util.Logging;

namespace Petal.Framework.Modding;

public class PetalModLoader : IModLoader<PetalMod>
{
	internal const string PetalRepositoryLink = "https://github.com/eosxff/Hedgemen";

	private readonly Dictionary<NamespacedString, PetalMod> _mods = new();

	public IReadOnlyDictionary<NamespacedString, PetalMod> Mods
		=> _mods;

	public ILogger Logger
	{
		get;
	}

	public PetalModLoader(ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(logger);
		Logger = logger;
	}

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

		ICollection<PetalMod> allMods = _mods.Values;

		foreach (var mod in allMods)
		{
			mod.PrePetalModLoaderModSetupPhase(context);
		}

		foreach (var mod in allMods)
		{
			mod.Setup(context);
		}

		foreach (var mod in allMods)
		{
			mod.PostPetalModLoaderSetupPhase(context);
		}

		return true;
	}

	public bool GetMod<T>(NamespacedString modID, out T mod) where T : PetalMod
	{
		mod = default;

		bool found = _mods.TryGetValue(modID, out var petalMod);

		if (found && petalMod is T tMod)
		{
			mod = tMod;
		}

		return found;
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

		foreach (var embeddedModsElement in context.EmbeddedMods)
		{
			if (embeddedModsElement is PetalEmbeddedMod embeddedMod)
			{
				SetModManifestAndDirectory(embeddedMod, embeddedMod.GetEmbeddedManifest(), new DirectoryInfo("."));
				list.Add(embeddedMod);
				Logger.Debug($"Loaded Embedded mod '{embeddedModsElement}'.");
			}
			else
				Logger.Critical($"Failed to load embedded mod '{embeddedModsElement}' This should not happen.");
		}

		foreach (var directory in directories)
		{
			var loadSuccessful = LoadModFromDirectory(context, directory, out var mod);

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

		var manifestJson = manifestFile.ReadString(Encoding.UTF8);
		var manifest = JsonSerializer.Deserialize<PetalModManifest?>(manifestJson);

		if (manifest is null)
		{
			Logger.Error($"Manifest could not be created from:\n{manifestJson}");
			return false;
		}

		if (string.IsNullOrEmpty(manifest.ModFileDll))
		{
			mod = new DefaultPetalMod();
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
	private void SetModManifestAndDirectory(PetalMod mod, PetalModManifest manifest, DirectoryInfo directory)
	{
		mod.Manifest = manifest;
		mod.Directory = directory;
	}
}
