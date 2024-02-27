using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldShallowWater : Terrain
{
	public override Color GetMapPixelColor()
		=> new(85, 136, 212);

	public override TerrainType GetTerrainType()
		=> TerrainType.Water;
}
