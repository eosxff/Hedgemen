using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla.WorldGeneration;

// todo fixme all of these BiomeSuppliers should reference its own BiomeDetails but it probably wouldn't work how i
// would want it to

public sealed class OverworldDesert : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Desert",
		TemperatureRange = new(10.0f, 30.0f),
		PrecipitationRange = new(0.0f, 125.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldDesert()
	};

	public override Color GetMapPixelColor()
		=> new(253, 209, 77);
}

public sealed class OverworldForest : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Forest",
		TemperatureRange = new(6.0f, 26.0f),
		PrecipitationRange = new(100.0f, 220.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldForest()
	};

	public override Color GetMapPixelColor()
		=> new(27, 76, 42);
}

public sealed class OverworldGrassland : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Grassland",
		TemperatureRange = new(-15.0f, 0.0f),
		PrecipitationRange = new(0.0f, 150.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldGrassland()
	};

	protected override void Awake()
	{
		Details = BiomeDetails; // todo
	}

	public override Color GetMapPixelColor()
	{
		return new Color(36, 120, 36);
	}

	public override void ReadData(PersistentData data)
	{
		base.ReadData(data);
		Details = BiomeDetails;
	}
}

public sealed class OverworldShrubland : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Shrubland",
		TemperatureRange = new(-5.0f, 30.0f),
		PrecipitationRange = new(0.0f, 60.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldShrubland()
	};

	public override Color GetMapPixelColor()
		=> new(155, 186, 61);
}

public sealed class OverworldTaiga : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Taiga",
		TemperatureRange = new(-15.0f, 10.0f),
		PrecipitationRange = new(20.0f, 220.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldTaiga()
	};

	public override Color GetMapPixelColor()
		=> Color.DarkGreen;
}

public sealed class OverworldTemperateRainforest : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Temperate Rainforest",
		TemperatureRange = new(8.0f, 20.0f),
		PrecipitationRange = new(220.0f, 300.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldTemperateRainforest()
	};

	public override Color GetMapPixelColor()
		=> new(81, 189, 59);
}

public sealed class OverworldTropicalRainforest : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Tropical Rainforest",
		TemperatureRange = new(20.0f, 28.0f),
		PrecipitationRange = new(320.0f, 420.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldTropicalRainforest()
	};

	public override Color GetMapPixelColor()
		=> new(72, 131, 47);
}

public sealed class OverworldTundra : Biome
{
    public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Tundra",
        TemperatureRange = new(-15.0f, 4.0f),
        PrecipitationRange = new(0.0f, 150.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldTundra()
    };

	protected override void Awake()
	{
		Details = BiomeDetails; // todo
	}

	public override Color GetMapPixelColor()
	{
		return new Color(79, 172, 178);
	}

	public override void ReadData(PersistentData data)
	{
		base.ReadData(data);
        Details = BiomeDetails;
	}
}
