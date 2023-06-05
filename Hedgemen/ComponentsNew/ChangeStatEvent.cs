using Petal.Framework.ECS;

namespace Hgm.ComponentsNew;

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