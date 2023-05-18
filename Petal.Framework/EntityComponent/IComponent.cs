using System;

namespace Petal.Framework.EntityComponent;

public interface IComponent<out TEntity, TEvent> : IAuxiliaryComponent<TEvent>
    where TEvent: IEvent
    where TEntity : IEntity<TEvent>
{
    public TEntity Self
    {
        get;
    }

    public ComponentStatus Status
    {
        get;
        set;
    }

    public void PropagateEvent(TEvent e);
    public void Destroy();

    public ComponentInfo GetComponentInfo();
}

public struct ComponentInfo
{
    public required NamespacedString ContentIdentifier
    {
        get;
        init;
    }

    public required Type ComponentType
    {
        get;
        init;
    }
}