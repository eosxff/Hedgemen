using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla.WorldGeneration;

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