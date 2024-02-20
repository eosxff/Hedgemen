using Hgm.Game.CellComponents;
using Hgm.Vanilla.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.EC;
using Petal.Framework.Persistence;
using Petal.Framework.Util;

namespace Hgm.Game.WorldGeneration;

public abstract class Biome : CellComponent
{
    public float Temperature
    {
        get;
        set;
    } = 0.0f;

    public float Precipitation
    {
        get;
        set;
    } = 0.0f;

    public BiomeDetails? Details
    {
        get;
        set;
    }

	protected override void RegisterEvents()
	{
		RegisterEvent<MapPixelColorQuery>(QueryMapPixelColor);
        RegisterEvent<WorldCellInfoQuery>(QueryWorldCellInfo);
	}

	private void QueryMapPixelColor(MapPixelColorQuery e)
    {
        if(e.Priority > Cartographer.DisplayPriority.Biome)
            return;

        e.MapPixelColor = GetMapPixelColor();
        e.Priority = Cartographer.DisplayPriority.Biome;
    }

    private void QueryWorldCellInfo(WorldCellInfoQuery e)
	{
		e.Temperature = Temperature;
        e.Precipitation = Precipitation;
	}

    public abstract Color GetMapPixelColor();

    public override PersistentData WriteData()
	{
		var data = base.WriteData();
        data.WriteField("hgm:temperature", Temperature);
        data.WriteField("hgm:precipitation", Precipitation);

        return data;
	}

	public override void ReadData(PersistentData data)
	{
		Temperature = data.ReadField("hgm:temperature", 0.0f);
        Precipitation = data.ReadField("hgm:precipitation", 0.0f);
	}
}

public sealed class BiomeDetails
{
    public required string Name
    {
        get;
        init;
    }

    public required Vector2 TemperatureRange
    {
        get;
        init;
    }

    public required Vector2 PrecipitationRange
    {
        get;
        init;
    }

    public required TerrainType RequiredTerrainType
    {
        get;
        init;
    }

    public required Supplier<Biome> BiomeSupplier
    {
        get;
        init;
    }
}
