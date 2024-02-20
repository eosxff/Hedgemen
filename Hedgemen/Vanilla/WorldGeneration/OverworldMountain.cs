using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldMountain : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Gray;

	public override TerrainType GetTerrainType()
		=> TerrainType.Mountain;
}
