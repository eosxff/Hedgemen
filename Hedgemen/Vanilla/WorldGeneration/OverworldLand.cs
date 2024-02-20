using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldLand : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Green;

	public override TerrainType GetTerrainType()
		=> TerrainType.Land;
}
