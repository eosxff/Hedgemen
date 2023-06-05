using System.Collections.Generic;
using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public class MapCell : IEntity<CellEvent>, IMutableEntity<MapCell, CellComponent, CellEvent>
{
	public EntityStatus Status { get; internal set; }

	public PropagateEventResult PropagateEvent(CellEvent e)
	{
		return PropagateEventResult.Success;
	}

	public void Destroy()
	{
		throw new System.NotImplementedException();
	}

	public IReadOnlyList<CellComponent> Components { get; } = new List<CellComponent>();

	public TComponentLocal? GetComponent<TComponentLocal>() where TComponentLocal : CellComponent
	{
		throw new System.NotImplementedException();
	}

	public bool GetComponent<TComponentLocal>(out TComponentLocal component) where TComponentLocal : CellComponent
	{
		throw new System.NotImplementedException();
	}

	public bool AddComponent(CellComponent component)
	{
		return false;
	}

	public bool RemoveComponent(CellComponent component)
	{
		return false;
	}

	public void RemoveAllComponents()
	{
		throw new System.NotImplementedException();
	}

	public SerializedRecord WriteObjectState()
	{
		throw new System.NotImplementedException();
	}

	public void ReadObjectState(SerializedRecord record)
	{
		throw new System.NotImplementedException();
	}
}