using System.IO;

namespace Hgm.Game;

public sealed class CampaignDirectory
{
	public DirectoryInfo Info
	{
		get;
	}

	public CampaignDirectory(DirectoryInfo info)
	{
		Info = info;
	}
}
