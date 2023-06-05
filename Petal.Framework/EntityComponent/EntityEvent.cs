namespace Petal.Framework.EntityComponent;

public abstract class EntityEvent : IEvent
{
	public Entity Sender
	{
		get;
		init;
	}

	public virtual bool Validate()
	{
		return Sender is not null;
	}
}