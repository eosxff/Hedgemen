using System;
using System.Collections.Generic;

namespace Petal.Framework.ECS;

public abstract class EntityComponent : IComponent<Entity, EntityEvent>
{
	public delegate void EventHandle<in TEvent>(TEvent e) where TEvent : EntityEvent;

	private delegate void EventHandleWrapped(EntityEvent e);

	private Dictionary<Type, EventHandleWrapped> _registeredEvents = new();
	
	public Entity Self
	{
		get;
		private set;
	}

	public void PropagateEvent(EntityEvent e)
	{
		bool found = _registeredEvents.TryGetValue(e.GetType(), out var handle);

		if (!found)
			return;

		handle(e);
	}

	public virtual void RegisterEvents() // todo make protected/internal
	{
		
	}

	protected void RegisterEvent<TEvent>(EventHandle<TEvent> handle) where TEvent : EntityEvent
	{
		_registeredEvents.Add(typeof(TEvent), args => handle((TEvent)args));
	}
}