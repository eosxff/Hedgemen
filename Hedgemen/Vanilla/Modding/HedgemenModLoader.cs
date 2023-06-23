using System;
using System.Collections.Generic;
using System.IO;
using Petal.Framework.IO;
using Petal.Framework.Modding;

namespace Hgm.Vanilla.Modding;

public class HedgemenModLoader : IModLoader<HedgemenMod>
{
	public ModLoaderSetupContext Setup()
	{
		var context = new ModLoaderSetupContext
		{
			EmbeddedMods = new List<IMod>(),
			ModsDirectory = new DirectoryInfo("mods"),
			ConfigFileName = "config.json"
		};

		return context;
	}

	public bool Start(ModLoaderSetupContext context)
	{
		var logger = Hedgemen.Instance.Logger;
		
		var modsQuery = QueryModsFromModDirectory(context);
		
		logger.Debug($"Number of mods recognized in folder '{context.ModsDirectory}': {modsQuery.Count}");

		return true;
	}

	private List<DirectoryInfo> QueryModsFromModDirectory(ModLoaderSetupContext context)
	{
		var directory = context.ModsDirectory;

		if (!directory.Exists)
			return new List<DirectoryInfo>();

		bool DirectoryFilter(DirectoryInfo d)
			=> d.FindFile(context.ConfigFileName).Exists;

		var validModDirectories = directory.GetDirectories(DirectoryFilter);
		
		var directories = new List<DirectoryInfo>(validModDirectories.Length);
		
		foreach (var dir in validModDirectories)
			directories.Add(dir);

		return directories;
	}
}