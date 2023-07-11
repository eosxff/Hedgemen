namespace Petal.Framework.Modding;

public abstract class PetalEmbeddedMod : PetalMod
{
	/// <summary>
	/// Get the <see cref="PetalModManifest"/> for a <see cref="PetalEmbeddedMod"/>. 
	/// </summary>
	/// <returns></returns>
	public abstract PetalModManifest GetEmbeddedManifest();
}