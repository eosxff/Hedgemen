using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;
using Petal.Framework.Persistence;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTundra : Biome
{
    public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Tundra",
        TemperatureRange = new(-15.0f, 0.0f),
        PrecipitationRange = new(0.0f, 150.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldTundra() // fixme should reference BiomeDetails but would cause infinite loop
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