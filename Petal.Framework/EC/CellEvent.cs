namespace Petal.Framework.EC;

public abstract class CellEvent : IEntityEvent
{
	public bool AllowAsync
	{
		get;
		protected set;
	} = false;

	public bool Async
	{
		get;
		internal set;
	} = false;
}
