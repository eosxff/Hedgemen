namespace Petal.Framework.EC;

public interface IEntityEvent
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
