using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Petal.Framework.EC;

public sealed class Entity : IEntity<EntityComponent, EntityEvent>
{
	private readonly IDictionary<Type, EntityComponent> _components = new Dictionary<Type, EntityComponent>();
	private readonly IDictionary<Type, int> _componentEvents = new Dictionary<Type, int>();

	public IReadOnlyCollection<EntityComponent> Components
		=> _components.Values as Dictionary<Type, EntityComponent>.ValueCollection;

	public void PropagateEvent(EntityEvent e)
	{
		foreach (var component in Components)
		{
			component.PropagateEvent(e);
		}
	}

	public async Task PropagateEventAsync(EntityEvent e)
	{
		await Task.Run(() => PropagateEvent(e));
	}

	public void PropagateEventIfResponsive(EntityEvent e)
	{
		if (WillRespondToEvent(e.GetType()))
			PropagateEvent(e);
	}

	public bool WillRespondToEvent(Type eventType)
	{
		return _componentEvents.ContainsKey(eventType);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WillRespondToEvent<T>() where T : EntityEvent
	{
		return WillRespondToEvent(typeof(T));
	}

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
			bool found = _componentEvents.TryGetValue(registeredEvent, out var eventCount);

			switch (found)
			{
				case true:
					_componentEvents[registeredEvent]++;
					break;

				case false:
					_componentEvents.Add(registeredEvent, 1);
					break;
			}
		}
	}

	private void RemoveRegisteredEventsFromComponent(EntityComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();

		foreach (var registeredEvent in registeredEvents)
		{
			bool found = _componentEvents.TryGetValue(registeredEvent, out int eventCount);

			switch (found)
			{
				case true:
					if (eventCount - 1 <= 0)
						_componentEvents.Remove(registeredEvent);
					else
						_componentEvents[registeredEvent]--;
					break;

				case false:
					break;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddComponent<T>() where T : EntityComponent, new()
	{
		AddComponent(new T());
	}

	public bool GetComponent<T>([NotNullWhen(true)] out T? component) where T : EntityComponent
	{
		component = default;

		bool found = _components.TryGetValue(typeof(T), out var comp);

		if (!found)
			return false;

		if (comp is T compAsT)
			component = compAsT;

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // will this even inline?
	public T? GetComponent<T>() where T : EntityComponent
	{
		bool found = GetComponent<T>(out var component);
		return component;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent(EntityComponent component)
	{
		return RemoveComponent(component, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent<T>() where T : EntityComponent
	{
		return RemoveComponent(typeof(T));
	}

	public bool RemoveComponent(Type componentType)
	{
		bool found = _components.TryGetValue(componentType, out var component);

		if (!found)
			return false;

		return RemoveComponent(component, true);
	}

	private bool RemoveComponent(EntityComponent component, bool unregisterEvents)
	{
		if (unregisterEvents)
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

	public DataStorage WriteStorage()
	{
		var data = new DataStorage(this);

		var components = new List<DataStorage>(_components.Count);

		foreach (var component in Components)
		{
			components.Add(component.WriteStorage());
		}

		data.WriteData(NamespacedString.FromDefaultNamespace("components"), components);

		return data;
	}

	public void ReadStorage(DataStorage storage)
	{
		if (storage.ReadData(
			    NamespacedString.FromDefaultNamespace("components"),
			    out List<DataStorage> dataList))
		{
			foreach (var element in dataList)
			{
				bool found = element.ReadData<EntityComponent>(out var component);

				if (found)
					AddComponent(component);
			}
		}
	}
}
