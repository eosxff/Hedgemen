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
		TemperatureRange = new(12.0f, 30.0f),
		PrecipitationRange = new(0.0f, 125.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldDesert()
	};

	public override Color GetMapPixelColor()
		=> new(253, 209, 77);
}

public sealed class OverworldGrassland : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Grassland",
		TemperatureRange = new(0.0f, 20.0f),
		PrecipitationRange = new(0.0f, 200.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldGrassland()
	};

	protected override void Awake()
	{
		Details = BiomeDetails; // todo
	}

	public override Color GetMapPixelColor()
		=> new(36, 120, 36);

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
		TemperatureRange = new(20.0f, 30.0f),
		PrecipitationRange = new(50.0f, 250.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldShrubland()
	};

	public override Color GetMapPixelColor()
		=> new(72, 131, 47);
}

public sealed class OverworldJungle : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Tropical Rainforest",
		TemperatureRange = new(15.0f, 30.0f),
		PrecipitationRange = new(300.0f, 455.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldJungle()
	};

	public override Color GetMapPixelColor()
		=> new(72, 131, 47);
}

public sealed class OverworldForest : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Forest",
		TemperatureRange = new(5.0f, 20.0f),
		PrecipitationRange = new(250.0f, 350.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldForest()
	};

	public override Color GetMapPixelColor()
		=> new(36, 120, 36);
}

public sealed class OverworldTaiga : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
	{
		Name = "Taiga",
		TemperatureRange = new(-5.0f, 5.0f),
		PrecipitationRange = new(50.0f, 250.0f),
		RequiredTerrainType = TerrainType.Land,
		BiomeSupplier = () => new OverworldTaiga()
	};

	public override Color GetMapPixelColor()
		=> new(20, 66, 22);
}

public sealed class OverworldTundra : Biome
{
    public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Tundra",
        TemperatureRange = new(-15.0f, 2.0f),
        PrecipitationRange = new(0.0f, 200.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldTundra()
    };

	protected override void Awake()
	{
		Details = BiomeDetails; // todo
	}

	public override Color GetMapPixelColor()
		=> new(168, 230, 226);

	public override void ReadData(PersistentData data)
	{
		base.ReadData(data);
        Details = BiomeDetails;
	}
}
