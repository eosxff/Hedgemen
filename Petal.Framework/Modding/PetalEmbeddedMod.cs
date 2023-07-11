namespace Petal.Framework.Modding;

public abstract class PetalEmbeddedMod : PetalMod
{
	/// <summary>
	/// Get the <see cref="PetalModManifest"/> for an embedded <see cref="PetalEmbeddedMod"/>. 
	/// </summary>
	/// <returns></returns>
	public abstract PetalModManifest GetEmbeddedManifest();
}