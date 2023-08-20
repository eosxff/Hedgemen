using Petal.Framework.EC;

namespace Hgm.Vanilla.WorldGeneration;

public abstract class Terrain : CellComponent
{
	public abstract float GetMinimumHeightRequirement();
}
