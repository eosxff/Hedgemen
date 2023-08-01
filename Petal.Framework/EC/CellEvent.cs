namespace Petal.Framework.EC;

public abstract class CellEvent : IEvent
{
	public required MapCell Sender
	{
		get;
		init;
	}

	public bool AllowAsync
	{
		get;
		internal set;
	}

	public bool Async
	{
		get;
		internal set;
	}
}
