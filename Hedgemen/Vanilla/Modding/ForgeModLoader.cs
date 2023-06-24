﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Petal.Framework;
using Petal.Framework.IO;
using Petal.Framework.Modding;
using Petal.Framework.Util.Logging;

namespace Hgm.Vanilla.Modding;

public class ForgeModLoader : IModLoader<ForgeMod>
{
	private readonly Dictionary<NamespacedString, ForgeMod> _mods = new();

	public IReadOnlyDictionary<NamespacedString, ForgeMod> Mods
		=> _mods;

	public ILogger Logger
	{
		get;
	}

	public ForgeModLoader(ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(logger);
		Logger = logger;
	}

	public ModLoaderSetupContext Setup()
	{
		var context = new ModLoaderSetupContext
		{
			EmbeddedMods = new List<IMod>(),
			ModsDirectory = new DirectoryInfo("mods"),
			ManifestFileName = "manifest.json"
		};

		return context;
	}

	public bool Start(ModLoaderSetupContext context)
	{
		var logger = Hedgemen.Instance.Logger;

		var modDirectoryQuery = QueryModsFromModDirectory(context);

		logger.Debug($"Number of mods recognized in folder '{context.ModsDirectory}': {modDirectoryQuery.Count}");

		var loadedMods = LoadModsFromEmbedAndDirectory(context, modDirectoryQuery);

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

	private List<ForgeMod> LoadModsFromEmbedAndDirectory(ModLoaderSetupContext context, List<DirectoryInfo> directories)
	{
		var list = new List<ForgeMod>(context.EmbeddedMods.Count + directories.Count);

		foreach (var embeddedMod in context.EmbeddedMods)
		{
			if (embeddedMod is ForgeMod hedgemenMod)
				list.Add(hedgemenMod);
			else
				Logger.Critical($"Failed to load embedded mod: {embeddedMod}. This should not happen.");
		}

		foreach (var directory in directories)
		{
			var loadSuccessful = LoadModFromDirectory(context, directory, out ForgeMod mod);

			if (!loadSuccessful)
				Logger.Error($"Could not load mod from '{directory.Name}'.");
			else
				Logger.Debug($"Successfully loaded '{mod.Manifest.NamespacedID}' of type '{mod.GetType()}'.");
		}
		
		return list;
	}
	
	private bool LoadModFromDirectory(ModLoaderSetupContext context, DirectoryInfo directory, out ForgeMod mod)
	{
		mod = null;
		
		var manifestFile = directory.FindFile(context.ManifestFileName);

		if (!manifestFile.Exists) // should never trip
		{
			Logger.Error($"File '{context.ManifestFileName}' could not be found in '{directory.Name}'");
			return false;
		}

		var manifestJson = manifestFile.ReadString(Encoding.UTF8);
		var manifest = JsonSerializer.Deserialize<ForgeModManifest?>(manifestJson);

		if (manifest is not null)
		{
			if (string.IsNullOrEmpty(manifest.ModFileDll))
			{
				mod = new DefaultForgeMod();
			}
			else
			{
				var dllFile = directory.FindFile(manifest.ModFileDll);

				if (dllFile.Exists)
				{
					var assembly = Assembly.LoadFile(dllFile.FullName);

					foreach (string dependencyFileName in manifest.Dependencies.Dlls)
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

					mod = (ForgeMod)Activator.CreateInstance(modMainType, true);

					if (mod is null)
					{
						Logger.Error($"Could not create instance from '{modMainType}'");
						return false;
					}
				}
				else
				{
					Logger.Error($"File '{dllFile.FullName}' does not exist.");
					return false;
				}
			}
			
			mod.Manifest = manifest;
			mod.Directory = directory;
			_mods.Add(new NamespacedString(manifest.NamespacedID), mod);
		}
		else
		{
			Logger.Error($"Manifest could not be created from:\n{manifestJson}");
			return false;
		}

		return true;
	}
}