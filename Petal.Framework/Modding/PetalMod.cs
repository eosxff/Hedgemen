using System.IO;
using Petal.Framework.Modding;

namespace Petal.Framework.Modding;

public abstract class PetalMod : IMod
{
	public DirectoryInfo Directory
	{
		get;
		internal set;
	}
	
	public PetalModManifest Manifest
	{
		get;
		internal set;
	}

	/// <summary>
	/// Get the <see cref="PetalModManifest"/> for an embedded <see cref="PetalMod"/>. 
	/// </summary>
	/// <returns></returns>
	public virtual PetalModManifest GetEmbeddedManifest()
		=> new();
}