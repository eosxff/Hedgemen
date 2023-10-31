using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public interface IAuxiliaryComponent<in TEvent> : IPersistent where TEvent : IEntityEvent
{
	public void PropagateEvent(TEvent e);
}