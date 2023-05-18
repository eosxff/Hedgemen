namespace Petal.Framework.EntityComponent;

public abstract class Component : IComponent<Entity, EntityEvent>
{
    public Entity? Self
    {
        get;
        private set;
    }

    public ComponentStatus Status
    {
        get;
        set;
    } = ComponentStatus.Active;

    public virtual void PropagateEvent(EntityEvent e)
    {
        
    }

    public void Destroy()
    {
        Self = null;
        OnDestroy();
    }

    protected virtual void OnDestroy()
    {
        
    }

    public abstract ComponentInfo GetComponentInfo();
}