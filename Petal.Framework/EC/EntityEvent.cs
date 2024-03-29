﻿namespace Petal.Framework.EC;

public abstract class EntityEvent : IEntityEvent
{
	public required Entity Sender
	{
		get;
		init;
	}

	public virtual bool AllowAsync
		=> false;

	public bool Async
	{
		get;
		internal set;
	} = false;
}
