namespace Petal.Framework.EntityComponent;

public abstract class EntityEvent : IEvent
{
    public Entity Sender
    {
        get;
        init;
    }
    
    public virtual bool Validate()
        => Sender is not null;
}