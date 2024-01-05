using Petal.Framework.EC;

namespace Hgm.Game.EntityComponents;

public sealed class PartyMember : EntityComponent
{
	public Party Party
	{
		get;
		set;
	} = new();
}
