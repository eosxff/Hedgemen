namespace Hgm.Game;

public interface ICampaignBehaviour
{
	public Campaign? Campaign
	{
		get;
	}

	public void AttachTo(Campaign campaign);

	public void OnCampaignStarted();
}
