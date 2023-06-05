using System;
using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public interface IComponent<out TEntity, in TEvent> :
	IAuxiliaryComponent<TEvent>, ISerializableObject
	where TEvent : IEvent
	where TEntity : IEntity<TEvent>
{
	public TEntity Self { get; }

	public ComponentStatus Status { get; set; }

	public void PropagateEvent(TEvent e);
	public void Destroy();

	public ComponentInfo GetComponentInfo();
}

public struct ComponentInfo
{
	public required NamespacedString ContentIdentifier { get; init; }

	public required Type ComponentType { get; init; }
}