using Petal.Framework.Persistence;

namespace Hgm.Game;

public sealed class HedgemenCampaign : Campaign
{
	public override void StartCampaign()
	{
		Hedgemen.Instance.Logger.Debug("Starting campaign!");
	}

	public override void LoadCampaign(PersistentData data)
	{

	}
}
