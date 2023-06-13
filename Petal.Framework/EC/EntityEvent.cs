namespace Petal.Framework.EC;

public abstract class EntityEvent : IEvent
{
	public required Entity Sender { get; init; }
}