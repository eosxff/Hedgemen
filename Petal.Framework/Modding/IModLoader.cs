using System.Collections.Generic;
using System.IO;

namespace Petal.Framework.Modding;

public sealed class ModLoaderSetupContext
{
	public required DirectoryInfo ModsDirectory
	{
		get;
		init;
	}

	public required string ManifestFileName
	{
		get;
		init;
	}
	
	public required IReadOnlyList<IMod> EmbeddedMods
	{
		get;
		init;
	}

	public required bool EmbedOnlyMode
	{
		get;
		init;
	}

	public required PetalGame Game
	{
		get;
		init;
	}

	public required PetalModLoader ModLoader
	{
		get;
		init;
	}
}

public struct ModLoaderSetupArgs
{
	public required PetalGame Game
	{
		get;
		init;
	}

	public required IReadOnlyList<IMod> EmbeddedMods
	{
		get;
		init;
	}

	public required bool EmbedOnlyMode
	{
		get;
		init;
	}

	public string? ModsDirectoryName
	{
		get;
		init;
	}

	public string? ManifestFileName
	{
		get;
		init;
	}
}

public interface IModLoader<in TMod> where TMod : IMod
{
	public ModLoaderSetupContext Setup(ModLoaderSetupArgs args);
	public bool Start(ModLoaderSetupContext context);

	public bool GetMod<T>(NamespacedString modID, out T mod) where T : TMod;
}