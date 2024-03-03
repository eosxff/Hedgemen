using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public abstract class EntityComponent : IComponent<EntityEvent>
{
	protected delegate void EventHandle<in TEvent>(TEvent e) where TEvent : EntityEvent;

	private delegate void EventHandleWrapped(EntityEvent e);

	private readonly Dictionary<Type, EventHandleWrapped> _registeredEvents = [];

	public Entity Self
	{
		get;
		private set;
	}

	public IReadOnlyCollection<Type> GetRegisteredEvents()
		=> _registeredEvents.Keys;

	public void PropagateEvent(EntityEvent e)
	{
		if(!_registeredEvents.TryGetValue(e.GetType(), out var handle))
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
		Awake();
		RegisterEvents();
	}

	protected virtual void Awake()
	{

	}

	protected virtual void RegisterEvents()
	{

	}

	protected void RegisterEvent<TEvent>(EventHandle<TEvent> handle) where TEvent : EntityEvent
	{
		_registeredEvents.TryAdd(typeof(TEvent), args => handle((TEvent)args));
	}

	public virtual PersistentData WriteData()
	{
		return new PersistentData(this);
	}

	public virtual void ReadData(PersistentData data)
	{

	}
}
