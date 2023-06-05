namespace Petal.Framework.EntityComponent;

public interface IAuxiliaryComponent<in TEvent> where TEvent : IEvent
{
	public void PropagateEvent(TEvent e);
}