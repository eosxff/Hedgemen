﻿using System;
using System.Collections.Generic;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public interface IComponent<in TEvent> : IPersistent where TEvent : IEntityEvent
{
	public IReadOnlyCollection<Type> GetRegisteredEvents();

	public void PropagateEvent(TEvent e);

	public bool WillRespondToEvent(Type eventType);
	public bool WillRespondToEvent<T>() where T : TEvent;

	public void Destroy();
}