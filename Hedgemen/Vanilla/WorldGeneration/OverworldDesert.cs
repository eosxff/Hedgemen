using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldDesert : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Desert",
        TemperatureRange = new(12.0f, 30.0f),
        PrecipitationRange = new(0.0f, 100.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldDesert()
    };

	public override Color GetMapPixelColor()
		=> new(253, 209, 77);
}
