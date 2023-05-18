namespace Petal.Framework.EntityComponent;

public interface IEntity<in TEvent> where TEvent : IEvent
{
    public EntityStatus Status
    {
        get;
    }
    
    public PropagateEventResult PropagateEvent(TEvent e);
}