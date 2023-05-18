using System.Collections.Generic;

namespace Petal.Framework.EntityComponent;

public class MapCell : IEntity<CellEvent>, IMutableEntity<MapCell, CellComponent, CellEvent>
{
    public EntityStatus Status
    {
        get;
        internal set;
    }

    public PropagateEventResult PropagateEvent(CellEvent e)
    {
        return PropagateEventResult.Success;
    }

    public IReadOnlyList<CellComponent> Components
    {
        get;
    } = new List<CellComponent>();

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

    public int RemoveAllPendingComponents()
    {
        throw new System.NotImplementedException();
    }
}