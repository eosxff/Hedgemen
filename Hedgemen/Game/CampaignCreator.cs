using Hgm.Vanilla;
using Petal.Framework;

namespace Hgm.Game;

public sealed class CampaignCreator
{
	public CampaignSettings Settings
	{
		get;
		set;
	} = new();

	public NamespacedString CampaignBehaviourName
	{
		get;
		set;
	} = NamespacedString.Default;

	public Campaign Create()
	{
		var campaignBehaviours = HedgemenVanilla.Instance.Registers.CampaignBehaviours;
		var campaignBehaviourRO = campaignBehaviours.MakeReference(CampaignBehaviourName);
		var campaignBehaviour = campaignBehaviourRO.Supply<ICampaignBehaviour>();

		return new Campaign(campaignBehaviour);
	}
}
