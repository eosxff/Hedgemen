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

	public virtual ForgeModManifest GetEmbeddedManifest()
		=> new();
}