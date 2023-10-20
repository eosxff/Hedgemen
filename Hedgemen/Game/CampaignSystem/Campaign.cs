using System.IO;
using System.Threading.Tasks;
using Hgm.Game.Scenes;
using Hgm.Game.WorldGeneration;
using Petal.Framework.Modding;

namespace Hgm.Game.CampaignSystem;

public sealed class Campaign
{
	public static void StartCampaign(CampaignStartArgs args)
	{
		var campaign = new Campaign(new CampaignSession(args.SessionDirectory, args.Hedgemen), args.Hedgemen);

		var generator = new CampaignGenerator
		{
			CampaignSessionDirectoryName = "new_campaign",
			Hedgemen = args.Hedgemen,
			ModList = args.ModList,
			StartingWorldCartographer = args.StartingWorldCartographer
		};

		args.Hedgemen.GlobalEvents.OnCampaignGeneratorInitialization(generator);
		generator.GenerateCampaignScenic(campaign);
	}

	public static Campaign ContinueCampaign(CampaignContinueArgs args)
	{
		var campaign = new Campaign(new CampaignSession(args.SessionDirectory, args.Hedgemen), args.Hedgemen);
		campaign.LoadCampaignFromDisk();
		return campaign;
	}

	public Hedgemen Hedgemen
	{
		get;
		private set;
	}

	public CampaignSession Session
	{
		get;
	}

	public CampaignUniverse Universe
	{
		get;
	}

	private Campaign(CampaignSession session, Hedgemen hedgemen)
	{
		Hedgemen = hedgemen;
		Session = session;
		Universe = new CampaignUniverse(this);
	}

	private void LoadCampaignFromDisk()
	{
		// stub
	}
}

public sealed class CampaignStartArgs
{
	public required PetalModList ModList
	{
		get;
		init;
	}

	public required Hedgemen Hedgemen
	{
		get;
		init;
	}

	public required DirectoryInfo SessionDirectory
	{
		get;
		init;
	}

	public required Cartographer StartingWorldCartographer
	{
		get;
		init;
	}
}

public sealed class CampaignContinueArgs
{
	public required Hedgemen Hedgemen
	{
		get;
		init;
	}

	public DirectoryInfo SessionDirectory
	{
		get;
		set;
	}
}
