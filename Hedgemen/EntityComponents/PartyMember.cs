using Petal.Framework.EC;

namespace Hgm.EntityComponents;

public sealed class PartyMember : EntityComponent
{
	public Party Party
	{
		get;
		set;
	} = new();
}
