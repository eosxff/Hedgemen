using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public abstract class EntityComponent : IComponent<EntityEvent>
{
	protected delegate void EventHandle<in TEvent>(TEvent e) where TEvent : EntityEvent;

	private delegate void EventHandleWrapped(EntityEvent e);

	private Dictionary<Type, EventHandleWrapped> _registeredEvents = new();

	public Entity Self
	{
		get;
		private set;
	}

	public IReadOnlyCollection<Type> GetRegisteredEvents()
	{
		var list = new List<Type>(_registeredEvents.Count);

		foreach (var registeredEvent in _registeredEvents.Keys)
		{
			list.Add(registeredEvent);
		}

		return list;
	}

	public void PropagateEvent(EntityEvent e)
	{
		bool found = _registeredEvents.TryGetValue(e.GetType(), out var handle);

		if (!found)
			return;

		handle(e);
	}

	public bool WillRespondToEvent(Type eventType)
	{
		return _registeredEvents.ContainsKey(eventType);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WillRespondToEvent<T>() where T : EntityEvent
	{
		return WillRespondToEvent(typeof(T));
	}

	public void Destroy()
	{
		OnDestroy();
		Self = null;
	}

	protected virtual void OnDestroy()
	{

	}

	internal void AddToEntity(Entity entity)
	{
		Self = entity;
		Initialize();
		RegisterEvents();
	}

	protected virtual void Initialize()
	{

	}

	protected virtual void RegisterEvents()
	{

	}

	protected void RegisterEvent<TEvent>(EventHandle<TEvent> handle) where TEvent : EntityEvent
	{
		_registeredEvents.Add(typeof(TEvent), args => handle((TEvent)args));
	}

	public virtual DataStorage WriteStorage()
	{
		return new DataStorage(this);
	}

	public virtual void ReadStorage(DataStorage storage)
	{

	}
}