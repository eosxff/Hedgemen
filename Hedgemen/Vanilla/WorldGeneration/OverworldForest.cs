using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldForest : Biome
{
	public static readonly BiomeDetails BiomeDetails = new()
    {
        Name = "Forest",
        TemperatureRange = new(5.0f, 25.0f),
        PrecipitationRange = new(150.0f, 220.0f),
        RequiredTerrainType = TerrainType.Land,
        BiomeSupplier = () => new OverworldForest()
    };

	public override Color GetMapPixelColor()
		=> new(27, 76, 42);
}
