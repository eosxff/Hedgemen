using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public interface IEntity<in TEvent> : ISerializableObject where TEvent : IEvent
{
	public EntityStatus Status
	{
		get;
	}

	public PropagateEventResult PropagateEvent(TEvent e);
	public void Destroy();
}