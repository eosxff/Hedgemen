﻿using System.Collections.Generic;
using System.IO;

namespace Petal.Framework.Modding;

public sealed class ModLoaderSetupContext
{
	public required DirectoryInfo ModsDirectory
	{
		get;
		init;
	}

	public required string ConfigFileName
	{
		get;
		init;
	}
	
	public required List<IMod> EmbeddedMods
	{
		get;
		init;
	}
}

public interface IModLoader<TMod> where TMod : IMod
{
	public ModLoaderSetupContext Setup();
	public bool Start(ModLoaderSetupContext context);
}