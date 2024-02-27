using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTaiga : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Taiga",
        TemperatureRange = new(-4.0f, 8.0f),
        PrecipitationRange = new(20.0f, 220.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldTaiga()
    };

	public override Color GetMapPixelColor()
		=> Color.DarkGreen;
}
