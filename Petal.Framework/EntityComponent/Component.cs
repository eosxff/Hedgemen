using System;
using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public abstract class Component : IComponent<Entity, EntityEvent>
{
	public Entity? Self
	{
		get;
		private set;
	}

	public ComponentStatus Status
	{
		get;
		set;
	} = ComponentStatus.Active;

	public virtual void PropagateEvent(EntityEvent e)
	{
	}

	public void Destroy()
	{
		if (Self is null)
			return;

		OnDestroyed();
		Self = null;
	}

	protected virtual void OnDestroyed()
	{
	}

	public abstract ComponentInfo GetComponentInfo();

	public virtual SerializedRecord WriteObjectState()
	{
		var record = new SerializedRecord(this);
		return record;
	}

	public virtual void ReadObjectState(SerializedRecord record)
	{
	}
}