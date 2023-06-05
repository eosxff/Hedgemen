namespace Petal.Framework.ECS;

public abstract class EntityEvent : IEvent
{
	public required Entity Sender
	{
		get;
		init;
	}
}