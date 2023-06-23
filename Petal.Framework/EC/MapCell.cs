using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Petal.Framework.Persistence;

namespace Petal.Framework.EC;

public class MapCell : IEntity<CellComponent, CellEvent>
{

	public SerializedData WriteObjectState()
	{
		throw new NotImplementedException();
	}

	public void ReadObjectState(SerializedData data)
	{
		throw new NotImplementedException();
	}

	public IReadOnlyCollection<CellComponent> Components
	{
		get;
	}
	public void PropagateEvent(CellEvent e)
	{
		throw new NotImplementedException();
	}

	public Task PropagateEventAsync(CellEvent e)
	{
		throw new NotImplementedException();
	}

	public void PropagateEventIfResponsive(CellEvent e)
	{
		throw new NotImplementedException();
	}

	public bool WillRespondToEvent(Type eventType)
	{
		throw new NotImplementedException();
	}

	public bool WillRespondToEvent<T>() where T : CellEvent
	{
		throw new NotImplementedException();
	}

	public void AddComponent(CellComponent component)
	{
		throw new NotImplementedException();
	}

	public void AddComponent<T>() where T : CellComponent, new()
	{
		throw new NotImplementedException();
	}

	public bool GetComponent<T>(out T component) where T : CellComponent
	{
		throw new NotImplementedException();
	}

	public T GetComponent<T>() where T : CellComponent
	{
		throw new NotImplementedException();
	}

	public bool RemoveComponent(CellComponent component)
	{
		throw new NotImplementedException();
	}

	public bool RemoveComponent<T>() where T : CellComponent
	{
		throw new NotImplementedException();
	}

	public bool RemoveComponent(Type componentType)
	{
		throw new NotImplementedException();
	}

	public void RemoveAllComponents()
	{
		throw new NotImplementedException();
	}

	public void Destroy()
	{
		throw new NotImplementedException();
	}
}