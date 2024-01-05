using Petal.Framework.EC;

namespace Hgm.Vanilla.EntityComponents;

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
