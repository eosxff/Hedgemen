using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldTallMountain : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.White;
	
	public override TerrainType GetTerrainType()
		=> TerrainType.Mountain;
}
