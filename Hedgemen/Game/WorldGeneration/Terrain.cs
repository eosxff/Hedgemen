using Hgm.Game.CellComponents;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;

namespace Hgm.Game.WorldGeneration;

public abstract class Terrain : CellComponent
{
	public abstract Color GetMapPixelColor();
	public abstract TerrainType GetTerrainType();

	protected override void RegisterEvents()
	{
		RegisterEvent<MapPixelColorQuery>(QueryMapPixelColor);
		RegisterEvent<WorldCellInfoQuery>(QueryWorldCellInfo);
	}

	private void QueryMapPixelColor(MapPixelColorQuery e)
	{
		if(e.Priority > MapPixelColorQuery.DisplayPriority.Terrain)
            return;

		e.MapPixelColor = GetMapPixelColor();
        e.Priority = MapPixelColorQuery.DisplayPriority.Terrain;
	}

	private void QueryWorldCellInfo(WorldCellInfoQuery e)
	{
		e.TerrainType = GetTerrainType();
	}
}

public enum TerrainType
{
	None,
	Water,
	Land,
	Mountain
}
