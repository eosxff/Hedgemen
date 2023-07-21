using Petal.Framework.EC;

namespace Hgm.Components;

public class ChangeStatEvent : EntityEvent
{
	public required string StatName
	{
		get;
		init;
	}

	public required int ChangeAmount
	{
		get;
		init;
	}
}