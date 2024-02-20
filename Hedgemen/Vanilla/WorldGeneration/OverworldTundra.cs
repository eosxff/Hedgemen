using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTundra : Biome
{
    public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Tundra",
        TemperatureRange = new(-10.0f, 0.0f),
        PrecipitationRange = new(0.0f, 455.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldTundra()
    };

	protected override void Awake()
	{
		Details = BiomeDetails; // todo
	}

	public override Color GetMapPixelColor()
	{
		return Color.BurlyWood;
	}

	public override void ReadData(PersistentData data)
	{
		base.ReadData(data);
        Details = BiomeDetails;
	}
}