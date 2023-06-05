namespace Petal.Framework.ECS;

public interface IComponent<TEntity, TEvent>
	where TEntity : IEntity<TEvent>
	where TEvent : IEvent
{
	public TEntity Self
	{
		get;
	}

	public void PropagateEvent(TEvent e);
}