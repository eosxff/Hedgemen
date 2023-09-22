using Petal.Framework.EC;

namespace Hgm.Game;

public sealed class Campaign
{
	public ICampaignBehaviour Behaviour
	{
		get;
		private set;
	}

	public Campaign()
	{

	}

	public Campaign(ICampaignBehaviour behaviour)
	{
		Behaviour = behaviour;
		behaviour.AttachTo(this);
	}

	public void StartCampaign()
	{
		Hedgemen.Instance.Logger.Debug($"Starting campaign!");
		Behaviour.OnCampaignStarted();
	}
}
