namespace Petal.Framework.EC;

public interface IEvent
{
	public bool AllowAsync
	{
		get;
	}

	public bool Async
	{
		get;
	}
}
