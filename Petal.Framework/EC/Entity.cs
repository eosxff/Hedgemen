using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Petal.Framework.Persistence;
using Petal.Framework.Util;
using Petal.Framework.Util.Extensions;

namespace Petal.Framework.EC;

public sealed class Entity : IEntity<EntityComponent, EntityEvent>
{
	private readonly IDictionary<Type, EntityComponent> _components = new Dictionary<Type, EntityComponent>();
	private readonly IDictionary<Type, int> _componentEvents = new Dictionary<Type, int>();

	public IReadOnlyCollection<EntityComponent> Components
		=> _components.Values as Dictionary<Type, EntityComponent>.ValueCollection;

	public bool HasComponents()
		=> _components.Count > 0;

	public void PropagateEvent(EntityEvent e)
	{
		foreach (var component in Components)
		{
			component.PropagateEvent(e);
		}
	}

	public async Task PropagateEventAsync(EntityEvent e)
	{
		if (!e.AllowAsync)
			throw new InvalidOperationException($"{e.GetType().Name} can not be ran asynchronously.");

		e.Async = true;
		await Task.Run(RunAsync);
		return;

		void RunAsync()
		{
			PropagateEvent(e);
		}
	}

	public void PropagateEventIfResponsive(EntityEvent e)
	{
		if (WillRespondToEvent(e.GetType()))
			PropagateEvent(e);
	}

	public async Task PropagateEventIfResponsiveAsync(EntityEvent e)
	{
		if (!e.AllowAsync)
			throw new InvalidOperationException($"{e.GetType().Name} can not be ran asynchronously.");

		if (WillRespondToEvent(e.GetType()))
			await PropagateEventAsync(e);
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

	public bool AddComponent(EntityComponent component)
	{
		if (component is null)
			return true;

		var componentType = component.GetType();

		if (_components.ContainsKey(componentType))
			return true;

		_components.Add(componentType, component);
		component.AddToEntity(this);
		AddRegisteredEventsFromComponent(component);

		return true;
	}

	private void AddRegisteredEventsFromComponent(EntityComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();

		foreach (var registeredEvent in registeredEvents)
		{
			bool found = _componentEvents.TryGetValue(registeredEvent, out int eventCount);

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
			if (!_componentEvents.TryGetValue(registeredEvent, out int eventCount))
				continue;

			if (eventCount - 1 <= 0)
				_componentEvents.Remove(registeredEvent);
			else
				_componentEvents[registeredEvent]--;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool AddComponent<T>() where T : EntityComponent, new()
	{
		return AddComponent(new T());
	}

	public bool GetComponent<T>([NotNullWhen(true)] out T? component) where T : EntityComponent
	{
		component = default;

		if (!_components.TryGetValue(typeof(T), out var comp))
			return false;

		if (comp is T compT)
			component = compT;

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // will this even inline?
	public T? GetComponent<T>() where T : EntityComponent
	{
		GetComponent<T>(out var component);
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
		if (!_components.TryGetValue(componentType, out var component))
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

	public PersistentData WriteData()
	{
		var data = new PersistentData(this);

		var components = new List<PersistentData>(_components.Count);

		foreach (var component in Components)
			components.Add(component.WriteData());

		data.WriteField("components", components);

		return data;
	}

	public void ReadData(PersistentData data)
	{
		if (!data.ReadField("components", out List<PersistentData> dataList))
			return;

		foreach (var element in dataList)
		{
			if(element.InstantiateData<EntityComponent>(out var component))
				AddComponent(component);
		}
	}
}
