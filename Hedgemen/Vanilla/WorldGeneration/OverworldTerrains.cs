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


public sealed class OverworldDeepWater : Terrain
{
	public override Color GetMapPixelColor()
		=> new(52, 68, 149);

	public override TerrainType GetTerrainType()
		=> TerrainType.Water;
}

public sealed class OverworldLand : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Green;

	public override TerrainType GetTerrainType()
		=> TerrainType.Land;
}

public sealed class OverworldMountain : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.Gray;

	public override TerrainType GetTerrainType()
		=> TerrainType.Mountain;
}

public sealed class OverworldTallMountain : Terrain
{
	public override Color GetMapPixelColor()
		=> Color.White;

	public override TerrainType GetTerrainType()
		=> TerrainType.Mountain;
}

