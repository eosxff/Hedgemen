using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Petal.Framework.EC;

public sealed class Entity : IEntity<EntityComponent, EntityEvent>, ISerializableObject
{
	private IDictionary<Type, EntityComponent> _components = new Dictionary<Type, EntityComponent>();
	private HashSet<EventSet> _componentEvents = new();

	public IReadOnlyCollection<EntityComponent> Components
		=> _components.Values as Dictionary<Type, EntityComponent>.ValueCollection;

	public void PropagateEvent(EntityEvent e)
	{
		foreach (var component in Components)
		{
			component.PropagateEvent(e);
		}
	}

	public void PropagateEventIfResponsive(EntityEvent e)
	{
		if(WillRespondToEvent(e.GetType()))
			PropagateEvent(e);
	}

	public bool WillRespondToEvent(Type eventType)
	{
		return _componentEvents.Contains(new EventSet
		{
			EventType = eventType,
			Count = 0
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WillRespondToEvent<T>() where T : EntityEvent
		=> WillRespondToEvent(typeof(T));

	public void AddComponent(EntityComponent component)
	{
		if (component is null)
			return;
		
		var componentType = component.GetType();

		if (_components.ContainsKey(componentType))
			return;
		
		_components.Add(componentType, component);
		component.AddToEntity(this);
		AddRegisteredEventsFromComponent(component);
	}

	private void AddRegisteredEventsFromComponent(EntityComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();

		foreach (var registeredEvent in registeredEvents)
		{
			var eventSet = new EventSet
			{
				EventType = registeredEvent,
				Count = 0
			};
			
			bool found = _componentEvents.TryGetValue(eventSet, out var componentEvent);

			switch (found)
			{
				case true:
					componentEvent.Count++;
					break;
				case false:
					_componentEvents.Add(eventSet);
					break;
			}
		}
	}

	private void RemoveRegisteredEventsFromComponent(EntityComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();
		
		foreach (var registeredEvent in registeredEvents)
		{
			var eventSet = new EventSet
			{
				EventType = registeredEvent,
				Count = 0
			};
			
			bool found = _componentEvents.TryGetValue(eventSet, out var componentEvent);

			switch (found)
			{
				case true:
					componentEvent.Count++;

					if (--componentEvent.Count <= 0)
						_componentEvents.Remove(eventSet);
					
					break;
				case false:
					break;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddComponent<T>() where T : EntityComponent, new()
		=> AddComponent(new T());

	public bool GetComponent<T>([MaybeNullWhen(false)] out T component) where T : EntityComponent
	{
		component = default;
		
		bool found = _components.TryGetValue(typeof(T), out var comp);

		if (!found)
			return false;

		if(comp is T compAsT)
			component = compAsT;

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // will this even inline?
	public T GetComponent<T>() where T : EntityComponent
	{
		bool found = GetComponent<T>(out var component);
		return component;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent(EntityComponent component)
		=> RemoveComponent(component, true);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent<T>() where T : EntityComponent
		=> RemoveComponent(typeof(T));

	public bool RemoveComponent(Type componentType)
	{
		bool found = _components.TryGetValue(componentType, out var component);

		if (!found)
			return false;

		return RemoveComponent(component, true);
	}

	private bool RemoveComponent(EntityComponent component, bool unregisterEvents)
	{
		if(unregisterEvents)
			RemoveRegisteredEventsFromComponent(component);

		bool removed = _components.TryRemove(component.GetType());
		component.Destroy();
		return removed;
	}

	public void RemoveAllComponents()
	{
		foreach (var component in Components)
		{
			RemoveComponent(component, false);
		}
		
		_components.Clear();
		_componentEvents.Clear();
	}

	public void Destroy()
	{
		RemoveAllComponents();
	}

	public SerializedData WriteObjectState()
	{
		var data = new SerializedData(this);

		var components = new List<SerializedData>(_components.Count);

		foreach (var component in Components)
		{
			components.Add(component.WriteObjectState());
		}

		data.AddField(NamespacedString.FromDefaultNamespace("components"), components);
		
		return data;
	}

	public void ReadObjectState(SerializedData data)
	{
		if (data.GetField(
			    NamespacedString.FromDefaultNamespace("components"),
			    out List<SerializedData> dataList))
		{
			foreach (var element in dataList)
			{
				var found = element.GetSerializedObject<EntityComponent>(out var component);

				if (found)
					AddComponent(component);
			}
		}
	}

	private class EventSet
	{
		public required Type EventType
		{
			get;
			init;
		}

		public required int Count
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			if (obj is EventSet eventSet)
				return EventType == eventSet.EventType;

			if (obj is Type type)
				return EventType == type;

			return false;
		}

		public override int GetHashCode()
			=> EventType.GetHashCode();
	}
}