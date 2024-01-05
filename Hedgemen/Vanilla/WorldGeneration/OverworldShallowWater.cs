using Hgm.Game.WorldGeneration;
using Microsoft.Xna.Framework;

namespace Hgm.Vanilla.WorldGeneration;

public sealed class OverworldShallowWater : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Aqua;
}
