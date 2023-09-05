using Petal.Framework.Persistence;

namespace Hgm;

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
