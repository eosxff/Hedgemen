using Petal.Framework.EC;

namespace Hgm.Components;

public class StatChangeEvent : EntityEvent
{
	public required string StatName
	{
		get;
		init;
	}

	public required int Amount
	{
		get;
		init;
	}
}