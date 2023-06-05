using Petal.Framework.EntityComponent.Persistence;

namespace Petal.Framework.EntityComponent;

public abstract class CellComponent : IComponent<MapCell, CellEvent>
{
	public MapCell Self { get; private set; }

	public ComponentStatus Status { get; set; }

	public void PropagateEvent(CellEvent e)
	{
	}

	public void Destroy()
	{
	}

	public abstract ComponentInfo GetComponentInfo();

	public SerializedRecord WriteObjectState()
	{
		throw new System.NotImplementedException();
	}

	public void ReadObjectState(SerializedRecord record)
	{
		throw new System.NotImplementedException();
	}
}