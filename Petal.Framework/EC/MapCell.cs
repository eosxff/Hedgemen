using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Petal.Framework.Persistence;
using Petal.Framework.Util.Extensions;

namespace Petal.Framework.EC;

public sealed class MapCell : IEntity<CellComponent, CellEvent>
{
	private readonly Dictionary<Type, CellComponent> _components = [];
	private readonly Dictionary<Type, int> _componentEvents = [];

	public IReadOnlyCollection<CellComponent> Components
		=> _components.Values;

	public bool HasAnyComponents()
		=> _components.Count > 0;

	public MapCell()
	{

	}

	public MapCell(params CellComponent[] components)
	{
		foreach (var component in components)
			AddComponent(component);
	}

	public void PropagateEvent(CellEvent e)
	{
		foreach (var component in Components)
		{
			component.PropagateEvent(e);
		}
	}

	public async Task PropagateEventAsync(CellEvent e)
	{
		if (!e.AllowAsync)
			throw new InvalidOperationException($"{e.GetType().Name} can not be ran asynchronously");

		e.Async = true;
		await Task.Run(RunAsync);
		return;

		void RunAsync()
		{
			PropagateEvent(e);
		}
	}

	public void PropagateEventIfResponsive(CellEvent e)
	{
		if (WillRespondToEvent(e.GetType()))
			PropagateEvent(e);
	}

	public int GetSubscriberCountForEvent<T>() where T : CellEvent
	{
		if (!_componentEvents.TryGetValue(typeof(T), out int subscriberCount))
			return 0;

		return subscriberCount;
	}

	public async Task PropagateEventIfResponsiveAsync(CellEvent e)
	{
		if (!e.AllowAsync)
			throw new InvalidOperationException($"{e.GetType().Name} can not be ran asynchronously");

		if (WillRespondToEvent(e.GetType()))
			await PropagateEventAsync(e);
	}

	public bool WillRespondToEvent(Type eventType)
	{
		return _componentEvents.ContainsKey(eventType);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool WillRespondToEvent<T>() where T : CellEvent
	{
		return WillRespondToEvent(typeof(T));
	}

	public bool AddComponent(CellComponent component)
	{
		if (component is null)
			return false;

		var componentType = component.GetType();

		if (_components.ContainsKey(componentType))
			return false;

		_components.Add(componentType, component);
		component.AddToMapCell(this);
		AddRegisteredEventsFromComponent(component);

		return true;
	}

	private void AddRegisteredEventsFromComponent(CellComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();

		foreach (var registeredEvent in registeredEvents)
		{
			if(_componentEvents.TryGetValue(registeredEvent, out int _))
				++_componentEvents[registeredEvent];
			else
				_componentEvents.Add(registeredEvent, 1);
		}
	}

	private void RemoveRegisteredEventsFromComponent(CellComponent component)
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
	public bool AddComponent<T>() where T : CellComponent, new()
	{
		return AddComponent(new T());
	}

	public bool GetComponent<T>([NotNullWhen(true)] out T? component) where T : CellComponent
	{
		component = default;

		if (!_components.TryGetValue(typeof(T), out var comp))
			return false;

		if (comp is T compT)
			component = compT;

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // will this even inline?
	public T? GetComponent<T>() where T : CellComponent
	{
		GetComponent<T>(out var component);
		return component;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent(CellComponent component)
	{
		return RemoveComponent(component, true);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool RemoveComponent<T>() where T : CellComponent
	{
		return RemoveComponent(typeof(T));
	}

	public bool RemoveComponent(Type componentType)
	{
		if (!_components.TryGetValue(componentType, out var component))
			return false;

		return RemoveComponent(component, true);
	}

	private bool RemoveComponent(CellComponent component, bool unregisterEvents)
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
			if(element.InstantiateData<CellComponent>(out var component))
				AddComponent(component);
		}
	}

	public bool HasComponent<T>() where T : CellComponent
		=> _components.ContainsKey(typeof(T));

	public bool HasComponents(params CellComponent[] components)
	{
		foreach(var component in components)
		{
			if(!_components.ContainsKey(component.GetType()))
				return false;
		}

		return true;
	}

	public bool HasComponentOf<T>()
	{
		// maybe cache results?
		foreach(var component in _components.Values)
		{
			if(component is T)
				return true;
		}

		return false;
	}
}
