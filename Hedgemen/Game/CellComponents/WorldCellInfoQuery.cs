using Hgm.Game.WorldGeneration;
using Petal.Framework.EC;
using Petal.Framework.Util;

namespace Hgm.Game.CellComponents;

public sealed class WorldCellInfoQuery : CellEvent, IResettable
{
    public float Temperature
    {
        get;
        set;
    } = float.MinValue;

    public float Precipitation
    {
        get;
        set;
    } = float.MinValue;

    public float NoiseHeight
    {
        get;
        set;
    } = float.MinValue;

    public TerrainType TerrainType
    {
        get;
        set;
    } = TerrainType.None;

	public void Reset()
	{
		Temperature = float.MinValue;
        Precipitation = float.MinValue;
        NoiseHeight = float.MinValue;
	}
}