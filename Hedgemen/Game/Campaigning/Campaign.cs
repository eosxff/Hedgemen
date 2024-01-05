namespace Hgm.Game.Campaigning;

public sealed class Campaign
{
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

	public Campaign(CampaignSession session, Hedgemen hedgemen)
	{
		Hedgemen = hedgemen;
		Session = session;
		Universe = new CampaignUniverse(this);
	}
}
