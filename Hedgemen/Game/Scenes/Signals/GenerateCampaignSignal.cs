using Hgm.Game.CampaignSystem;
using Petal.Framework.Scenery;

namespace Hgm.Game.Scenes.Signals;

public sealed class GenerateCampaignSignal : ImmediateSignal
{
	public CampaignGenerator Generator
	{
		get;
	}

	public GenerateCampaignSignal(CampaignGenerator generator)
	{
		Generator = generator;
	}

	public override void Fire(Scene scene)
	{

	}
}
