﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public interface IEntity<TComponent, in TEvent> : IPersistent
	where TEvent : IEntityEvent
	where TComponent : IComponent<TEvent>
{
	public IReadOnlyCollection<TComponent> Components
	{
		get;
	}

	public bool HasComponents();
	public void PropagateEvent(TEvent e);
	public Task PropagateEventAsync(TEvent e);
	public void PropagateEventIfResponsive(TEvent e);

	public bool WillRespondToEvent(Type eventType);
	public bool WillRespondToEvent<T>() where T : TEvent;

	public bool AddComponent(TComponent component);
	public bool AddComponent<T>() where T : TComponent, new();

	public bool GetComponent<T>([NotNullWhen(true)] out T? component) where T : TComponent;
	public T? GetComponent<T>() where T : TComponent;

	public bool RemoveComponent(TComponent component);
	public bool RemoveComponent<T>() where T : TComponent;
	public bool RemoveComponent(Type componentType);

	public void RemoveAllComponents();

	public void Destroy();
}
