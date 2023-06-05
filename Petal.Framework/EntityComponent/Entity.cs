using System;
using System.Collections.Generic;
using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public sealed class Entity : IEntity<EntityEvent>, IMutableEntity<Entity, Component, EntityEvent>
{
	private readonly ComponentGroup _components = new()
	{
		Dictionary = new Dictionary<Type, Component>(),
		List = new List<Component>()
	};

	public EntityStatus Status
	{
		get;
		set;
	} = EntityStatus.Active;

	public PropagateEventResult PropagateEvent(EntityEvent e)
	{
		if (Status == EntityStatus.Inactive)
			return PropagateEventResult.InactiveEntity;

		if (!e.Validate())
			return PropagateEventResult.InvalidEvent;

		for (var i = 0; i < _components.List.Count; ++i)
		{
			var component = _components.List[i];

			if (component.Status == ComponentStatus.Active)
				component.PropagateEvent(e);
		}

		return PropagateEventResult.Success;
	}

	public void Destroy()
	{
		RemoveAllComponents();
		Status = EntityStatus.Destroyed;
	}

	public IReadOnlyList<Component> Components
		=> _components.List;

	public TComponentLocal? GetComponent<TComponentLocal>() where TComponentLocal : Component
	{
		var found = _components.Dictionary.TryGetValue(typeof(TComponentLocal), out var component);
		return (TComponentLocal)component;
	}

	public bool GetComponent<TComponentLocal>(out TComponentLocal component) where TComponentLocal : Component
	{
		var found = _components.Dictionary.TryGetValue(typeof(TComponentLocal), out var comp);
		component = comp as TComponentLocal;
		return found;
	}

	public bool AddComponent(Component component)
	{
		// should we allow this to fail silently?
		if (component is null)
			return false;
		
		lock (_components)
		{
			if (_components.Dictionary.ContainsKey(component.GetType()))
				return false;

			_components.Add(component);

			return true;
		}
	}

	public bool RemoveComponent(Component component)
	{
		lock (_components)
		{
			if (!_components.Dictionary.ContainsKey(component.GetType()))
				return false;

			component.Destroy();
			component.Status = ComponentStatus.Inactive;
			_components.Remove(component);

			return true;
		}
	}

	public void RemoveAllComponents()
	{
		lock (_components)
		{
			for (var i = 0; i < _components.List.Count; ++i)
			{
				var component = _components.List[i];
				component.Destroy();
			}

			_components.List.Clear();
			_components.Dictionary.Clear();
		}
	}

	public SerializedRecord WriteObjectState()
	{
		var record = new SerializedRecord(this);

		var components = new List<SerializedRecord>(_components.List.Count);

		for (var i = 0; i < _components.List.Count; ++i)
		{
			var component = _components.List[i];
			components.Add(component.WriteObjectState());
		}

		record.AddField(NamespacedString.FromDefaultNamespace("components"), components);

		return record;
	}

	public void ReadObjectState(SerializedRecord record)
	{
		if (record.GetField(
			    NamespacedString.FromDefaultNamespace("components"),
			    out List<SerializedRecord> records))
		{
			foreach (var element in records)
			{
				var found = element.GetSerializedObject<Component>(out var component);

				if (found)
					_components.Add(component);
			}
		}
	}

	private class ComponentGroup
	{
		public required Dictionary<Type, Component> Dictionary
		{
			get;
			init;
		}

		public required List<Component> List
		{
			get;
			init;
		}

		public void Add(Component component)
		{
			Dictionary.Add(component.GetType(), component);
			List.Add(component);
		}

		public void Remove(Component component)
		{
			Dictionary.Remove(component.GetType());
			List.Remove(component);
		}
	}
}