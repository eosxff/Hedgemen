using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

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
