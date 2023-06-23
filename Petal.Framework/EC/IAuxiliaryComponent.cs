using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public interface IAuxiliaryComponent<in TEvent> : ISerializableObject where TEvent : IEvent
{
	public void PropagateEvent(TEvent e);
}