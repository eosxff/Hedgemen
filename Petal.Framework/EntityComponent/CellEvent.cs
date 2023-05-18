namespace Petal.Framework.EntityComponent;

public abstract class CellEvent : IEvent
{
    public MapCell Sender
    {
        get;
        init;
    }

    public virtual bool Validate()
        => Sender is not null;
}