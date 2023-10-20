using System;
using System.Threading.Tasks;
using Hgm.Game.Scenes;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Modding;

namespace Hgm.Game.CampaignSystem;

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

	public async void GenerateCampaignScenic(Campaign campaign)
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

		campaign.Universe.AddWorld(startingWorldMap);
		campaign.Hedgemen.ChangeScenesAsync(() => new CampaignScene());
	}

	public void GenerateCampaign(Campaign campaign)
	{

	}
}
