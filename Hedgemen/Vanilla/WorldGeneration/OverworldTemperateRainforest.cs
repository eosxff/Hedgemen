using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

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
