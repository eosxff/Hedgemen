using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Petal.Framework;
using Petal.Framework.EC;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

public sealed class MapCell : IEntity<CellComponent, CellEvent>
{
	private readonly IDictionary<Type, CellComponent> _components = new Dictionary<Type, CellComponent>();
	private readonly IDictionary<Type, int> _componentEvents = new Dictionary<Type, int>();

	public IReadOnlyCollection<CellComponent> Components
		=> _components.Values as Dictionary<Type, CellComponent>.ValueCollection;

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

	private void RemoveRegisteredEventsFromComponent(CellComponent component)
	{
		var registeredEvents = component.GetRegisteredEvents();

		foreach (var registeredEvent in registeredEvents)
		{
			bool found = _componentEvents.TryGetValue(registeredEvent, out int eventCount);

			if (!found)
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

		bool found = _components.TryGetValue(typeof(T), out var comp);

		if (!found)
			return false;

		if (comp is T compAsT)
			component = compAsT;

		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // will this even inline?
	public T? GetComponent<T>() where T : CellComponent
	{
		bool found = GetComponent<T>(out var component);
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
		bool found = _components.TryGetValue(componentType, out var component);

		if (!found)
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
			bool found = element.InstantiateData<CellComponent>(out var component);

			if (found)
				AddComponent(component);
		}
	}
}
