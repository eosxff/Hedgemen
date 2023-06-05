namespace Petal.Framework.EntityComponent;

public abstract class CellEvent : IEvent
{
	public MapCell Sender { get; init; }

	public virtual bool Validate()
	{
		return Sender is not null;
	}
}