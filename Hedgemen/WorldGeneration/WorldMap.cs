using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.WorldGeneration;

public class WorldMap
{
	public Map<MapCell> Cells
	{
		get;
	}

	public WorldMap(Map<MapCell> cells)
	{
		Cells = cells;
	}
}
