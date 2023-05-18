using System.Collections.Generic;

namespace Petal.Framework.EntityComponent;

public interface IMutableEntity<TEntity, TComponent, TEvent>
    where TComponent : IComponent<TEntity, TEvent>
    where TEntity : IEntity<TEvent>
    where TEvent : IEvent
{
    public IReadOnlyList<TComponent> Components
    {
        get;
    }

    public TComponentLocal? GetComponent<TComponentLocal>() where TComponentLocal : TComponent;
    public bool GetComponent<TComponentLocal>(out TComponentLocal component) where TComponentLocal : TComponent;
    public bool AddComponent(TComponent component);
    public bool RemoveComponent(TComponent component);
    public int RemoveAllPendingComponents();
}