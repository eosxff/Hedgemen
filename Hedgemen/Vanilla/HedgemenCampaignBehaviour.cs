using System;
using Hgm.Game;

namespace Hgm.Vanilla;

public sealed class HedgemenCampaignBehaviour : ICampaignBehaviour
{
	public Campaign Campaign
	{
		get;
		private set;
	}

	public void AttachTo(Campaign campaign)
	{
		Campaign = campaign;
	}

	public void OnCampaignStarted()
	{
		Hedgemen.Instance.Logger.Debug($"We made it!");
	}
}
