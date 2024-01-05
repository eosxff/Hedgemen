using Petal.Framework.EC;

namespace Hgm.Game.CellComponents;

public class SetHeightEvent : CellEvent
{
	public required float Height
	{
		get;
		init;
	}
}
