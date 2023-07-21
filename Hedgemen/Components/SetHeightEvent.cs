using Petal.Framework.EC;

namespace Hgm.Components;

public class SetHeightEvent : CellEvent
{
	public required float Height
	{
		get;
		init;
	}
}