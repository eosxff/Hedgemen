namespace Petal.Framework.ECS;

public interface IEntity<in TEvent>
	where TEvent : IEvent
{
	public void PropagateEvent(TEvent e);
}