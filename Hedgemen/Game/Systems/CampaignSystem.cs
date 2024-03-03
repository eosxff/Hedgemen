using System;
using System.IO;
using System.Threading.Tasks;
using Hgm.Game.Campaigning;
using Hgm.Game.Scenes;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Modding;
using Petal.Framework.Util;

namespace Hgm.Game.Systems;

public static class CampaignSystem
{
	public static void StartCampaign(CampaignStartArgs args)
	{
		var campaign = new Campaign(new CampaignSession(args.SessionDirectory, args.Hedgemen), args.Hedgemen);

		var generator = new CampaignGenerator
		{
			StartingWorldCartographer = args.StartingWorldCartographer,
			CampaignSessionDirectoryName = "new_campaign",
			Hedgemen = args.Hedgemen,
			ModList = args.ModList,
		};

		args.Hedgemen.GlobalEvents.OnCampaignGeneratorInitialization(generator);
		generator.GenerateCampaignScenic(campaign);
	}

	public static void StartCampaignNew(CampaignStartArgs args)
	{

	}

	public static void ContinueCampaign(CampaignContinueArgs args)
	{
		var campaign = new Campaign(new CampaignSession(args.SessionDirectory, args.Hedgemen), args.Hedgemen);
	}

	public static Campaign GenerateCampaign()
	{
		return null;
	}
}

public sealed class CampaignGenerator
{
	public Cartographer StartingWorldCartographer
	{
		get;
		set;
	}

	public required Hedgemen Hedgemen
	{
		get;
		set;
	}

	public required string CampaignSessionDirectoryName
	{
		get;
		set;
	} = string.Empty;

	public required PetalModList ModList
	{
		get;
		set;
	}

	public async void GenerateCampaignAsync(Campaign campaign)
	{
		var startingWorldMap = await Task.Run(GenerateStartingWorld);
		PetalExceptions.ThrowIfNull(startingWorldMap);

		campaign.Universe.AddWorld(startingWorldMap);
		campaign.Hedgemen.ChangeScenesAsync(() => new CampaignScene());
	}

	public void GenerateCampaignScenic(Campaign campaign)
	{
		campaign.Hedgemen.ChangeScenes(new CampaignGenerationScene(this));
	}

	private WorldMap GenerateStartingWorld()
	{
		return StartingWorldCartographer.Generate();
	}
}


public readonly struct CampaignStartArgs
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

public readonly struct CampaignContinueArgs
{
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
}
