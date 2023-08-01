using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public interface IEntity<TComponent, in TEvent> : IDataStorageHandler
	where TEvent : IEvent
	where TComponent : IComponent<TEvent>
{
	public IReadOnlyCollection<TComponent> Components
	{
		get;
	}

	public void PropagateEvent(TEvent e);
	public Task PropagateEventAsync(TEvent e);
	public void PropagateEventIfResponsive(TEvent e);

	public bool WillRespondToEvent(Type eventType);
	public bool WillRespondToEvent<T>() where T : TEvent;

	public void AddComponent(TComponent component);
	public void AddComponent<T>() where T : TComponent, new();

	public bool GetComponent<T>([NotNullWhen(true)] out T? component) where T : TComponent;
	public T? GetComponent<T>() where T : TComponent;

	public bool RemoveComponent(TComponent component);
	public bool RemoveComponent<T>() where T : TComponent;
	public bool RemoveComponent(Type componentType);

	public void RemoveAllComponents();

	public void Destroy();
}
