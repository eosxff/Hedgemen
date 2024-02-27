using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldDeepWater : Terrain
{
	public override Color GetMapPixelColor()
		=> new(52, 68, 149);

	public override TerrainType GetTerrainType()
		=> TerrainType.Water;
}
