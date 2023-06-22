namespace Petal.Framework.EC;

public abstract class CellEvent : IEvent
{
	public required MapCell Sender
	{
		get;
		init;
	}
}