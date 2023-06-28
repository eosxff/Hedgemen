using System.IO;

namespace Petal.Framework.Modding;

/// <summary>
/// Base class for mods using the <see cref="PetalModLoader"/>.
/// </summary>
public abstract class PetalMod : IMod
{
	/// <summary>
	/// The working directory of the mod.
	/// </summary>
	public DirectoryInfo Directory
	{
		get;
		internal set;
	}
	
	/// <summary>
	/// The manifest data of the mod.
	/// </summary>
	public PetalModManifest Manifest
	{
		get;
		internal set;
	}

	/// <summary>
	/// Used as a callback for debugging purposes mostly.
	/// </summary>
	protected internal virtual void OnLoadedToPetalModLoader()
	{
		
	}

	/// <summary>
	/// Called before <see cref="PetalModLoader"/> calls <see cref="Setup"/> for all loaded <see cref="PetalMod"/>.
	/// </summary>
	/// /// <param name="context">Context for the <see cref="PetalModLoader"/> setup.</param>
	protected internal virtual void PrePetalModLoaderModSetupPhase(ModLoaderSetupContext context)
	{
		
	}
	
	/// <summary>
	/// All setup functionality for the mod.
	/// </summary>
	/// <param name="context">Context for the <see cref="PetalModLoader"/> setup.</param>
	protected internal virtual void Setup(ModLoaderSetupContext context)
	{
		
	}

	/// <summary>
	/// Called after<see cref="PetalModLoader"/> calls <see cref="Setup"/> for all loaded <see cref="PetalMod"/>.
	/// </summary>
	/// <param name="context">Context for the <see cref="PetalModLoader"/> setup.</param>
	protected internal virtual void PostPetalModLoaderSetupPhase(ModLoaderSetupContext context)
	{
		
	}

	/// <summary>
	/// Get the <see cref="PetalModManifest"/> for an embedded <see cref="PetalMod"/>. 
	/// </summary>
	/// <returns></returns>
	public virtual PetalModManifest GetEmbeddedManifest()
		=> new();
}