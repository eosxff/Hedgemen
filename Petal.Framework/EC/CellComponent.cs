using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public abstract class CellComponent : IComponent<CellEvent>
{
	protected delegate void EventHandle<in TEvent>(TEvent e) where TEvent : CellEvent;
	private delegate void EventHandleWrapped(CellEvent e);

	private readonly Dictionary<Type, EventHandleWrapped> _registeredEvents = new();

	public MapCell Self
	{
		get;
		private set;
	}

	public IReadOnlyCollection<Type> GetRegisteredEvents()
		=> _registeredEvents.Keys;

	public void PropagateEvent(CellEvent e)
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
	public bool WillRespondToEvent<T>() where T : CellEvent
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

	internal void AddToMapCell(MapCell cell)
	{
		Self = cell;
		Initialize();
		RegisterEvents();
	}

	protected virtual void Initialize()
	{

	}

	protected virtual void RegisterEvents()
	{

	}

	protected void RegisterEvent<TEvent>(EventHandle<TEvent> handle) where TEvent : CellEvent
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
