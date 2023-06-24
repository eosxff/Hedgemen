using System.IO;
using Petal.Framework.Modding;

namespace Hgm.Vanilla.Modding;

public abstract class ForgeMod : IMod
{
	public DirectoryInfo Directory
	{
		get;
		internal set;
	}
	
	public ForgeModManifest Manifest
	{
		get;
		internal set;
	}

	/// <summary>
	/// Get the <see cref="ForgeModManifest"/> for an embedded <see cref="ForgeMod"/>. 
	/// </summary>
	/// <returns></returns>
	public virtual ForgeModManifest GetEmbeddedManifest()
		=> new();
}