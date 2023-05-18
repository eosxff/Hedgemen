using System;
using System.Collections.Generic;

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

        for (int i = 0; i < _components.List.Count; ++i)
        {
            var component = _components.List[i];
            
            if((component.Status & ComponentStatus.Active) == ComponentStatus.Active)
                component.PropagateEvent(e);
        }

        return PropagateEventResult.Success;
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
        bool found = _components.Dictionary.TryGetValue(typeof(TComponentLocal), out var comp);
        component = comp as TComponentLocal;
        return found;
    }

    public bool AddComponent(Component component)
    {
        lock (_components)
        {
            if(_components.Dictionary.ContainsKey(component.GetType()))
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
            _components.Remove(component);

            return true;
        }
    }

    public int RemoveAllPendingComponents()
    {
        int removedCount = 0;
        
        for (int i = 0; i < _components.List.Count; ++i)
        {
            var component = _components.List[i];

            if ((component.Status & ComponentStatus.PendingRemoval) == ComponentStatus.PendingRemoval)
            {
                RemoveComponent(component);
                ++removedCount;
            }
        }

        return removedCount;
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