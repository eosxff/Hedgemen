using Petal.Framework.EntityComponent;

namespace Hgm.Components;

public class StatChangeEvent : EntityEvent
{
	public required string StatName { get; init; }

	public required int Amount { get; init; }
}