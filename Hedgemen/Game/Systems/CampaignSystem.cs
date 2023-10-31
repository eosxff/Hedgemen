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
			CampaignSessionDirectoryName = "new_campaign",
			Hedgemen = args.Hedgemen,
			ModList = args.ModList,
			StartingWorldCartographer = args.StartingWorldCartographer,
			StartingWorldCartographyOptions = new CartographyOptions
			{
				MapDimensions = new Vector2Int(384, 384),
				Seed = new Random().Next(int.MinValue, int.MaxValue)
			}
		};

		args.Hedgemen.GlobalEvents.OnCampaignGeneratorInitialization(generator);
		generator.GenerateCampaignScenic(campaign);
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

	public required Cartographer StartingWorldCartographer
	{
		get;
		set;
	}

	public required CartographyOptions StartingWorldCartographyOptions
	{
		get;
		set;
	}

	public async void GenerateCampaignAsync(Campaign campaign)
	{
		WorldMap startingWorldMap = null;

		await Task.Run(() =>
		{
			startingWorldMap = StartingWorldCartographer.Generate(new CartographyOptions
			{
				MapDimensions = new Vector2Int(384, 384),
				Seed = new Random().Next(int.MinValue, int.MaxValue)
			});
		});

		PetalExceptions.ThrowIfNull(startingWorldMap);

		campaign.Universe.AddWorld(startingWorldMap);
		campaign.Hedgemen.ChangeScenesAsync(() => new CampaignScene());
	}

	public void GenerateCampaignScenic(Campaign campaign)
	{
		campaign.Hedgemen.ChangeScenes(new CampaignGenerationScene(this));
	}
}


public struct CampaignStartArgs
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

public struct CampaignContinueArgs
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
