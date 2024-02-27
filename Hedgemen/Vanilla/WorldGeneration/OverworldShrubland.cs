using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

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
