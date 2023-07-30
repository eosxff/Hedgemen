namespace Petal.Framework.Modding;

/// <summary>
/// Used for first class mods. <see cref="PetalModLoader"/> will throw an exception if any first class mod
/// does not inherit from this.
/// </summary>
public abstract class PetalEmbeddedMod : PetalMod
{
	/// <summary>
	/// Get the <see cref="PetalModManifest"/> for the embedded mod. This usage should typically be used
	/// for mod initialization. Recommended to use <see cref="PetalMod.Manifest"/> for subsequent queries.
	/// </summary>
	public abstract PetalModManifest GetEmbeddedManifest();
}
