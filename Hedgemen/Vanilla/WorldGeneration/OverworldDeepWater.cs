using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldDeepWater : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Blue;
}