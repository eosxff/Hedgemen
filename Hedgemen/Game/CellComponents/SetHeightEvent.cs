using Petal.Framework.EC;

namespace Hgm.Game.CellComponents;

public sealed class SetHeightEvent : CellEvent
{
	public required float Height
	{
		get;
		init;
	}
}
